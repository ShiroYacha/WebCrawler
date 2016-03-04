using Fiddler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebCrawler
{
    public partial class MainForm : Form
    {
        delegate void updateUI();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
            FiddlerApplication.Startup(8888, FiddlerCoreStartupFlags.RegisterAsSystemProxy);
            ctripBrowser.ScriptErrorsSuppressed = true;
            qunarBrowser.ScriptErrorsSuppressed = true;
            WebProxy myProxy = new WebProxy();
            Uri newUri = new Uri("http://localhost:8888");
            myProxy.Address = newUri;
            FiddlerApplication.Prefs.SetStringPref("fiddler.config.path.makecert", @"c:\Program Files (x86)\Fiddler2\Makecert.exe");
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                {
                    throw new Exception("Unable to create cert for FiddlerCore.");
                }
                X509Store certStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadWrite);
                try
                {
                    certStore.Add(CertMaker.GetRootCertificate());
                }
                finally
                {
                    certStore.Close();
                }
            }
        }

        private List<HotelInfo> ctripResults = new List<HotelInfo>();
        private List<HotelInfo> qunarResults = new List<HotelInfo>();

        private object locker = new object();
        void FiddlerApplication_AfterSessionComplete(Session oSession)
        {

            string responseBody = oSession.GetResponseBodyAsString();

            try
            {
                if (responseBody.IndexOf("<div class=\"hotel_pic\">") > -1 && responseBody.IndexOf("http://hotels.ctrip.com/hotel/shanghai2") > -1)
                {
                    lock (locker)
                    {

                        // update counter
                        ctripPageCounter++;

                        // get content node
                        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                        htmlDocument.LoadHtml(responseBody);
                        var contentNode = htmlDocument.GetElementbyId("hotel_list");

                        int nextStartIndex;
                        foreach (var node in contentNode.ChildNodes.Where(n => n.Name == "div"))
                        {
                            nextStartIndex = 0;

                            var hotelInfo = new HotelInfo();

                            // scan results
                            var inner = node.InnerHtml;
                            if (inner.Length == 0)
                                continue;
                            hotelInfo.Name = ExtractContent(inner, "<a title=\"", "\"", nextStartIndex, out nextStartIndex);
                            hotelInfo.Rate = ExtractContent(inner, "<span class=\"hotel_value\">", "<", nextStartIndex, out nextStartIndex);
                            ExtractContent(inner, "<span class=\"total_judgement_score\">", "<", nextStartIndex, out nextStartIndex);
                            hotelInfo.RecommendPercentage = ExtractContent(inner, "<span class=\"total_judgement_score\">", "用户推荐", nextStartIndex, out nextStartIndex);
                            hotelInfo.NumberOfRate = ExtractContent(inner, "<span class=\"hotel_judgement\">源自", "位住客点评", nextStartIndex, out nextStartIndex);
                            hotelInfo.Price = ExtractContent(inner, "&yen;</dfn>", "<", nextStartIndex, out nextStartIndex);
                            // save result
                            ctripResults.Add(hotelInfo);
                        }

                        // go to next
                        NavigateToNextCtripAddress();
                    }

                }
                else if (responseBody.IndexOf("hotellist_inner") > -1)
                {
                    lock (locker)
                    {
                        // update counter
                        qunarPageCounter++;

                        // get content node
                        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                        htmlDocument.LoadHtml(responseBody);
                        var contentNode = htmlDocument.GetElementbyId("hotellist_inner");

                        int nextStartIndex;
                        foreach (var node in contentNode.ChildNodes.Where(n => n.Name == "div"))
                        {
                            nextStartIndex = 0;

                            if (node.OuterHtml.IndexOf("sr_item sr_item_new") < 0)
                                continue;

                            var hotelInfo = new HotelInfo();

                            // scan results
                            var inner = node.InnerHtml;
                            if (inner.Length == 0)
                                continue;
                            hotelInfo.Name = ExtractContent(inner, "alt=\"", "\"", nextStartIndex, out nextStartIndex);
                            hotelInfo.Stars = ExtractContent(inner, "ratings_stars_", "  star_track", 0, out nextStartIndex);
                            hotelInfo.Rate = ExtractContent(inner, "average js--hp-scorecard-scoreval\">", "<", 0, out nextStartIndex);
                            hotelInfo.NumberOfRate = ExtractContent(inner, "<span class=\"score_from_number_of_reviews\" style=\"margin-bottom: 5px;\">\n", "条", 0, out nextStartIndex);
                            hotelInfo.RecommendPercentage = "N/A";
                            //hotelInfo.Price = ExtractContent(inner, "&yen;</cite><b>", "<", nextStartIndex, out nextStartIndex);
                            // save result
                            qunarResults.Add(hotelInfo);
                        }

                        // go to next
                        if (contentNode.ChildNodes.Any())
                        {
                            NavigateToNextQunarAddress();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        private string ExtractContent(string rawData, string prefix, string suffix, int startIndex, out int nameNodeEnd)
        {
            var nameNodeStart = rawData.IndexOf(prefix, startIndex);
            if (nameNodeStart == -1)
            {
                nameNodeEnd = startIndex;
                return "";
            }
            nameNodeEnd = rawData.IndexOf(suffix, nameNodeStart + prefix.Length);
            if (nameNodeEnd == -1)
                return "";
            return rawData.Substring(nameNodeStart + prefix.Length, nameNodeEnd - nameNodeStart - prefix.Length);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Fiddler.FiddlerApplication.Shutdown();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            Task.Run(() => NavigateToNextCtripAddress());
            //Task.Run(() => NavigateToNextQunarAddress());
        }

        private int ctripPageCounter = 0;
        private int qunarPageCounter = 0;

        private void NavigateToNextCtripAddress()
        {
            lock (locker)
            {
                if (ctripPageCounter < scanPagesSlider.Value)
                {
                    var page = "";
                    if (ctripPageCounter > 0)
                    {
                        page = "/p" + (ctripPageCounter + 1).ToString();
                    }
                    var url = "http://www.mafengwo.cn/hotel/10099/#scope=city%2C0%2C&price=0%2C&q=&p=1&sort=order_desc&sales=0&avl=0&tag=19548";//"http://hotels.ctrip.com/hotel/shanghai2" + page + "#ctm_ref=ctr_hp_sb_lst";
                    ctripBrowser.Invoke(new Action(() =>
                    {
                        ctripBrowser.Navigate(url);
                    }));
                    ctripCounterLabel.Invoke(new updateUI(() =>
                    {
                        ctripCounterLabel.Text = (ctripPageCounter + 1).ToString();
                    }));
                }
            }
        }

        private void NavigateToNextQunarAddress()
        {
            lock (locker)
            {
                if (qunarPageCounter < scanPagesSlider.Value)
                {
                    if (qunarPageCounter == 0)
                    {
                        var url = "http://www.booking.com/searchresults.zh-cn.html?aid=349128;label=cn-V9Ch_mxYkujP2dZPNLFLPAS69844110988%3Apl%3Ata%3Ap1%3Ap2%3Aac%3Aap1t1%3Aneg%3Afi%3Atikwd-1967728583%3Alp1003215%3Ali%3Adec%3Adm;sid=002c37404f8f8b5d5f3fa6a77c9504cb;dcid=1;class_interval=1;csflt=%7B%7D;dest_id=-1924465;dest_type=city;dtdisc=0;group_adults=2;group_children=0;hlrd=0;hyb_red=0;inac=0;label_click=undef;nha_red=0;no_rooms=1;offset=0;offset_unavail=1;redirected_from_city=0;redirected_from_landmark=0;redirected_from_region=0;review_score_group=empty;room1=A%2CA;sb_price_type=total;score_min=0;si=ai%2Cco%2Cci%2Cre%2Cdi;src=country;ss=%E4%B8%8A%E6%B5%B7;ss_all=0;ss_raw=%E4%B8%8A%E6%B5%B7;ssb=empty;sshis=0;origin=search;srpos=1&tfl_cwh=1";
                        qunarBrowser.Invoke(new Action(() =>
                        {
                            qunarBrowser.Navigate(url);
                        }));
                    }
                    else
                    {
                        qunarBrowser.Invoke(new Action(() =>
                        {
                            var elementCollection = qunarBrowser.Document.GetElementsByTagName("div");
                            foreach (HtmlElement curElement in elementCollection)
                            {
                                if (curElement.GetAttribute("classname").ToString() == "results-paging")
                                {
                                    var lastElement = curElement.Children[curElement.Children.Count - 1];
                                    var href = lastElement.GetAttribute("href");
                                    qunarBrowser.Navigate(href);
                                }
                            }
                        }));
                    }
                    qunarCounterLabel.Invoke(new updateUI(() =>
                    {
                        qunarCounterLabel.Text = (qunarPageCounter + 1).ToString();
                    }));
                }
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            var path = @"C:\Users\sebzh\Desktop\ctrip.csv";
            var path2 = @"C:\Users\sebzh\Desktop\qunar.csv";
            ExtractToFile(path, ctripResults);
            ExtractToFile(path2, qunarResults);
        }

        private void ExtractToFile(string path, List<HotelInfo> infos)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            //Create the file.
            var separator = ";";
            using (FileStream fs = File.Create(path))
            {
                AddText(fs, "Name;Price;Rate;NumberOfRate;RecommendPercentage;Stars;\r\n");
                foreach (var hotelInfo in infos)
                {
                    AddText(fs, hotelInfo.Name + separator);
                    AddText(fs, hotelInfo.Price + separator);
                    AddText(fs, hotelInfo.Rate + separator);
                    AddText(fs, hotelInfo.NumberOfRate + separator);
                    AddText(fs, hotelInfo.RecommendPercentage + separator);
                    AddText(fs, hotelInfo.Stars + separator);
                    AddText(fs, "\r\n");
                }
            }
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }
}
