using Fiddler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using HtmlAgilityPack;
using System.Diagnostics;

namespace HeadlessWebCrawler
{
    class Program
    {
        public static string chromeDriverDirectory = @"C:\CS\Playground\WebCrawler";
        public static string phantomDriverDirectory = @"C:\CS\Playground\WebCrawler\packages\PhantomJS.1.9.2\tools\phantomjs";
        public static IWebDriver Driver;
        public static int timeoutLoad = 15;

        public static void Reset()
        {
            Driver = new ChromeDriver(chromeDriverDirectory, new ChromeOptions(), TimeSpan.FromSeconds(20));
            //Driver = new PhantomJSDriver(phantomDriverDirectory, new PhantomJSOptions(), TimeSpan.FromSeconds(timeout));
        }

        public static void TryStop()
        {
            try
            {
                // stop 
                Program.Driver.FindElement(By.TagName("body")).SendKeys("Keys.ESCAPE");
            }
            catch (Exception)
            {
            }

        }

        static void Main(string[] args)
        {
            Setup();
            Reset();


            //CLeanData();

            //RunPhase0();
            RunPhase1();
            //RunPhase2();
            //while (true)
            {
                //var p3 = RunPhase3();
                //ExportData();

                //Console.ReadKey();
                //try
                //{
                //Driver.Close();
                //Driver.Dispose();
                //}
                //catch (Exception)
                //{

                //}
            }
            try
            {
                Console.ReadKey();
                Driver.Close();
                Driver.Dispose();
            }
            catch (Exception)
            {
            }
           
        }

        static void RunPhase0()
        {
            // 
            var engine = new P0Engine();
            engine.BaseUrl = @"http://www.tripadvisor.cn/Hotels-g308272-Shanghai-Hotels.html";
            engine.MaxPage = 20;
            engine.Start();
        }

        static void RunPhase1()
        {
            // 
            var engine = new P1Engine();
            engine.Start();
        }

        static void RunPhase2()
        {
            // 
            var engine = new P2Engine();
            engine.Start();
        }

        static P3Engine RunPhase3()
        {
            // 
            var engine = new P3Engine();
            engine.Start();
            return engine;
        }

        static void CLeanData()
        {
            string text = System.IO.File.ReadAllText(@"filter.txt");
            var filterList = text.Split(new string[] { "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            FileStream fsi = new FileStream(@"p0.txt", FileMode.OpenOrCreate);
            var serializer = new XmlSerializer(typeof(List<HotelData>));
            var data = serializer.Deserialize(fsi) as List<HotelData>;
            fsi.Close();

            var cleanData = new List<HotelData>();
            foreach (var d in data)
            {
                if (!cleanData.Any(x => x.Name == d.Name) && !filterList.Contains(d.Name))
                {
                    cleanData.Add(d);
                }
            }

            FileStream fsi2 = new FileStream(@"p0c.txt", FileMode.OpenOrCreate);
            serializer.Serialize(fsi2, cleanData);
            fsi2.Close();
        }

        static void ExportData()
        {
            FileStream fsi = new FileStream(@"p2.txt", FileMode.OpenOrCreate);
            var serializer = new XmlSerializer(typeof(List<HotelData>));
            var data = serializer.Deserialize(fsi) as List<HotelData>;
            fsi.Close();
            var path1 = @"p3-1.txt";
            var path2 = @"p3-2.txt";
            if (File.Exists(path1))
            {
                File.Delete(path1);
            }

            //Create the file.
            var separator = ";";
            using (FileStream fs = File.Create(path1))
            {
                AddText(fs, "Name;ForeignKey;Star;Ranking;Locality;Location;TCommentCount;TExcellentCount;TGoodCount;TNormalCount;TBadCount;TVeryBadCount;TFamilyCat;TCoupleCat;TSoloCat;TBusinessCat;\r\n");
                foreach (var hotelData in data)
                {
                    if (hotelData.PrimaryCommentData == null)
                        continue;

                    AddText(fs, hotelData.Name + separator);
                    AddText(fs, hotelData.ForeignKey + separator);
                    //AddText(fs, $"http://hotels.ctrip.com/hotel/shanghai2/k1{hotelData.Name}#ctm_ref=hod_hp_sb_lst" + separator);
                    double star;
                    if (double.TryParse(hotelData.Star, out star))
                    {
                        AddText(fs, hotelData.Star + separator);
                    }
                    else
                    {
                        AddText(fs, separator);
                    }
                    AddText(fs, hotelData.Ranking + separator);
                    AddText(fs, hotelData.Locality + separator);
                    AddText(fs, hotelData.Location + separator);
                    AddText(fs, hotelData.PrimaryCommentData.CommentCount + separator);
                    AddText(fs, hotelData.PrimaryCommentData.ExcellentCount + separator);
                    AddText(fs, hotelData.PrimaryCommentData.GoodCount + separator);
                    AddText(fs, hotelData.PrimaryCommentData.NormalCount + separator);
                    AddText(fs, hotelData.PrimaryCommentData.BadCount + separator);
                    AddText(fs, hotelData.PrimaryCommentData.VeryBadCount + separator);
                    AddText(fs, hotelData.PrimaryCommentData.FamilyCat + separator);
                    AddText(fs, hotelData.PrimaryCommentData.CoupleCat + separator);
                    AddText(fs, hotelData.PrimaryCommentData.SoloCat + separator);
                    AddText(fs, hotelData.PrimaryCommentData.BusinessCat + separator);
                    AddText(fs, "\r\n");
                }
            }

            if (File.Exists(path2))
            {
                File.Delete(path2);
            }

            FileStream fsi2 = new FileStream(@"p3.txt", FileMode.Open);
            var serializer2 = new XmlSerializer(typeof(List<ForeignCommentData>));
            var data2 = serializer2.Deserialize(fsi2) as List<ForeignCommentData>;
            fsi2.Close();

            using (FileStream fs = File.Create(path2))
            {
                AddText(fs, "ForeignName;ForeignKey;Star;CCommentCount;CRecommendCommentCount;CNeedToImproveCommentCount;CScore;CPrice;COpenYear;RoomCount;\r\n");
                foreach (var d in data2)
                {

                    if (data != null)
                    {
                        AddText(fs, d.ForeignName + separator);
                        AddText(fs, d.ForeignKey + separator);
                        AddText(fs, d.Star + separator);
                        AddText(fs, d.CommentCount + separator);
                        AddText(fs, d.RecommendCommentCount + separator);
                        AddText(fs, d.NeedToImproveCommentCount + separator);
                        AddText(fs, d.Score + separator);
                        AddText(fs, d.Price + separator);
                        AddText(fs, d.OpenYear + separator);
                        AddText(fs, d.RoomCount + separator);
                    }
                    else
                    {
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                    }
                    AddText(fs, "\r\n");
                }
            }
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        static void Setup()
        {
            FiddlerApplication.Startup(8888, FiddlerCoreStartupFlags.RegisterAsSystemProxy);
            //WebProxy myProxy = new WebProxy();
            //Uri newUri = new Uri("http://localhost:8888");
            //myProxy.Address = newUri;
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
    }

    class P0Engine : Engine
    {
        public int MaxPage { get; set; }

        public override void Start()
        {
            Program.Driver.Url = BaseUrl;
            Program.Driver.Navigate();
        }

        protected override string ToLocation
        {
            get
            {
                return @"p0.txt";
            }
        }

        protected override string FromLocation
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        private bool first = true;
        protected override void AfterSessionComplete(Session oSession)
        {
            string responseBody = oSession.GetResponseBodyAsString();

            if (responseBody.Contains("<div id=\"ACCOM_OVERVIEW\""))
            {
                //if(first)
                //{
                //    Console.WriteLine("Enter to continue");
                //    Console.ReadKey();
                //    Program.Driver.FindElement(By.TagName("body")).SendKeys(Keys.F5);
                //    first = false;
                //    return;
                //}

                // get content node
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(responseBody);


                foreach (var node in htmlDocument.DocumentNode.SelectNodes("//div[@class='meta_listing easyClear metaCacheKey msk ']"))
                {
                    var rawData = node.InnerHtml;
                    if (!rawData.Contains("listing_info popIndexValidation"))
                        return;

                    var hotelData = new HotelData();
                    int lastIndex;
                    hotelData.Name = ExtractContent(rawData, "dir=\"ltr\">", "<", 0, out lastIndex);
                    hotelData.PrimaryKey = "http://www.tripadvisor.cn" + ExtractContent(rawData, "<a target=\"_blank\" href=\"", "\"", 0, out lastIndex);
                    Data.Add(hotelData);

                }


                Console.WriteLine($"Processed data: {Data.Count}");
                NavigateToNext(htmlDocument);
            }
        }

        protected override void NavigateToNext(HtmlAgilityPack.HtmlDocument document)
        {
            try
            {
                var num1 = document.DocumentNode.SelectSingleNode("//span[@class='pageNum current']")?.GetAttributeValue("data-page-number", "");
                var num2 = document.DocumentNode.SelectSingleNode("//span[@class='pageNum first current']")?.GetAttributeValue("data-page-number", "");
                var numString = string.IsNullOrEmpty(num1) ? num2 : num1;
                var thisPage = int.Parse(numString);
                if (thisPage >= MaxPage)
                {
                    Console.WriteLine("DONE!");
                    SerializeToFile();
                }
                else
                {
                    var next = Program.Driver.FindElement(By.XPath("//a[@class ='nav next ui_button primary taLnk']"));
                    if (next != null)
                    {
                        next.SendKeys(Keys.Enter);
                        //SerializeToFile();
                    }
                    else
                    {
                        Console.WriteLine("DONE!");
                        SerializeToFile();
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.Write(e.Message);
            }

        }
    }

    class P1Engine : Engine
    {
        private int NextIndex;

        protected override string ToLocation
        {
            get
            {
                return @"p1.txt";
            }
        }

        protected override string FromLocation
        {
            get
            {
                return @"p0.txt";
            }
        }

        public override void Start()
        {
            DeserializeFromFile();
            NextIndex = 48;
            NavigateToNext(null);
        }

        protected override void AfterSessionComplete(Session oSession)
        {
            string responseBody = oSession.GetResponseBodyAsString();
            if (responseBody.Contains("<div id=\"PAGE\""))
            {
                var hotelData = Data[NextIndex];

                // get content node
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(responseBody);

                // crawler
                int lastIndex;
                hotelData.PrimaryCommentData = new PrimaryCommentData();

                hotelData.Ranking = ExtractContent(responseBody, "排名第", "（", 0, out lastIndex);
                hotelData.Star = ExtractContent(responseBody, "酒店星级：</span>", "星级", 0, out lastIndex);
                hotelData.PrimaryCommentData.CommentCount = ExtractContent(responseBody, "tabs_header reviews_header\">", "条来自猫途鹰", 0, out lastIndex);
                hotelData.PrimaryCommentData.ExcellentCount = GetCommentCount(responseBody, 5);
                hotelData.PrimaryCommentData.GoodCount = GetCommentCount(responseBody, 4);
                hotelData.PrimaryCommentData.NormalCount = GetCommentCount(responseBody, 3);
                hotelData.PrimaryCommentData.BadCount = GetCommentCount(responseBody, 2);
                hotelData.PrimaryCommentData.VeryBadCount = GetCommentCount(responseBody, 1);
                hotelData.PrimaryCommentData.FamilyCat = GetCommentCat(responseBody, "家庭");
                hotelData.PrimaryCommentData.CoupleCat = GetCommentCat(responseBody, "夫妻");
                hotelData.PrimaryCommentData.SoloCat = GetCommentCat(responseBody, "独自旅游");
                hotelData.PrimaryCommentData.BusinessCat = GetCommentCat(responseBody, "商务");

                int relatedStartIndex;
                ExtractContent(responseBody, "hr_tabs content_block hr_tabs_block", "\"", 0, out relatedStartIndex); //
                ExtractContent(responseBody, "region_title region_title_nomargin", "\"", 0, out lastIndex);
                hotelData.Locality = ExtractContent(responseBody, " ", "</", lastIndex, out lastIndex);
                var lat = ExtractContent(responseBody, "<div class=\"mapContainer\" data-lat=\"", "\"", relatedStartIndex, out lastIndex);
                var lon = ExtractContent(responseBody, "data-lng=\"", "\"", lastIndex, out lastIndex);
                hotelData.Location = $"{lat},{lon}"; // 
                hotelData.NumberOfRooms = ExtractContent(responseBody, "<span class=\"tabs_num_rooms\"> ", "</", relatedStartIndex, out lastIndex);

                // navigate to next
                NavigateToNext(null);
            }
        }

        private string GetCommentCount(string responseBody, int number)
        {
            int lastIndex;
            ExtractContent(responseBody, $"<label class=\"row_label\" for=\"com{number}\"", ">", 0, out lastIndex);
            return ExtractContent(responseBody, "row_count\">", "<", lastIndex, out lastIndex);
        }

        private string GetCommentCat(string responseBody, string cat)
        {
            int lastIndex;
            ExtractContent(responseBody, $"{cat}</div>", "<", 0, out lastIndex);
            return ExtractContent(responseBody, "<div class=\"value\">", "<", lastIndex, out lastIndex);
        }

        protected override void NavigateToNext(HtmlAgilityPack.HtmlDocument document)
        {
            if (++NextIndex == Data.Count)
            {
                Console.WriteLine("Done");
                SerializeToFile();
                return;
            }
            Console.WriteLine(">> Working on = " + NextIndex);
            SerializeToFile();

            //Program.Driver.Close();
            //Program.Reset();
            //Program.Driver.FindElement(By.TagName("body")).SendKeys(Keys.Escape);
            var next = Data[NextIndex].PrimaryKey;
            Program.Driver.Url = next;
            Program.Driver.Navigate();

        }
    }

    class P2Engine : Engine
    {
        private int NextIndex;
        private const int StartIndex = -1;
        protected override string ToLocation
        {
            get
            {
                return @"p2.txt";
            }
        }

        protected override string FromLocation
        {
            get
            {
                return @"p2.txt";
            }
        }

        public override void Start()
        {
            DeserializeFromFile();
            NextIndex = StartIndex;
            NavigateToNext(null);
        }

        private object locker = new object();
        protected override void AfterSessionComplete(Session oSession)
        {

            string responseBody = oSession.GetResponseBodyAsString();
            if (responseBody.Contains("很抱歉，暂时无法找到符合您要求的酒店"))
            {
                try
                {
                    // navigate to next
                    NavigateToNext(null);

                }
                catch (Exception)
                {
                }
            }
            else if (responseBody.Contains("searchresult_info"))
            {
                //hotelPositionJSON
                int lastIndex;
                var link = ExtractContent(responseBody, "<a data-dopost=\"T\" class=\"hotel_judge\" href=\"", "\"", 0, out lastIndex);
                if (link == "")
                    return;

                var commentsLink = "http://hotels.ctrip.com" + link + "#ctm_ref=hod_sr_lst_dl_c_1_1";

                Console.WriteLine("To detail page...");

                try
                {
                    var hotelData = Data[NextIndex];
                    hotelData.ForeignKey = commentsLink;
                    //Program.Driver.Navigate().GoToUrl(commentsLink);


                    // navigate to next
                    NavigateToNext(null);
                }
                catch (Exception)
                {

                }

            }

        }

        protected override void NavigateToNext(HtmlDocument document)
        {
            if (++NextIndex == Data.Count)
            {
                Console.WriteLine("Done");
                SerializeToFile();
                return;
            }
            Console.WriteLine(">> Working on = " + NextIndex);

            if (NextIndex >= StartIndex + 2)
            {
                SerializeToFile();
                try
                {
                    Program.Driver.FindElement(By.TagName("body")).SendKeys(Keys.Escape);
                    Console.WriteLine("Esc...");
                    //Program.Reset();
                    //Program.Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(15));
                }
                catch (Exception)
                {
                }
            }

            var next = $"http://hotels.ctrip.com/hotel/shanghai2/k1{Data[NextIndex].Name}#ctm_ref=hod_hp_sb_lst";
            try
            {

                Program.Driver.Url = HttpUtility.UrlPathEncode(next);
                Program.Driver.Navigate();
                Console.WriteLine("Detail...");
            }
            catch (Exception)
            {
            }
            //if(NextIndex>1)
        }
    }

    class P3Engine : Engine
    {
        private int NextIndex;
        private int StartIndex = -1;
        private bool locked = false;
        protected override string ToLocation
        {
            get
            {
                return @"p3.txt";
            }
        }

        protected override string FromLocation
        {
            get
            {
                return @"p2.txt";
            }
        }

        public override void Start()
        {
            DeserializeFromFile();
            NextIndex = StartIndex;
            NavigateToNext(null);
        }


        protected override void DeserializeFromFile()
        {
            base.DeserializeFromFile();
            FileStream fs = new FileStream(ToLocation, FileMode.OpenOrCreate);
            var serializer = new XmlSerializer(typeof(List<ForeignCommentData>));
            ForeignComments = serializer.Deserialize(fs) as List<ForeignCommentData>;
            StartIndex = ForeignComments.Count-1;
        }

        private object locker = new object();
        public List<ForeignCommentData> ForeignComments = new List<ForeignCommentData>();
        protected override void AfterSessionComplete(Session oSession)
        {
            if (locked)
                return;

            string responseBody = oSession.GetResponseBodyAsString();
            ForeignCommentData fd;
            if (ForeignComments.Count <= NextIndex)
            {
                fd = new ForeignCommentData();
                ForeignComments.Add(fd);
                fd.ForeignKey = Data[NextIndex].ForeignKey;
            }
            else
            {
                fd = ForeignComments[NextIndex];
            }
            if (responseBody.Contains("<link rel=\"canonical\" href=\""))
            {
                int lastIndex;
                fd.ForeignId = ExtractContent(responseBody, "<link rel=\"canonical\" href=\"", "\"", 0, out lastIndex);
            }
            if (responseBody.Contains("<span id=\"ctl00_MainContentPlaceHolder_commonHead_imgStar\" class=\""))
            {
                //<span id="ctl00_MainContentPlaceHolder_commonHead_imgStar" class="
                int lastIndex;
                fd.Star = ExtractContent(responseBody, "<span id=\"ctl00_MainContentPlaceHolder_commonHead_imgStar\" class=\"", "\"", 0, out lastIndex);
            }
            if (responseBody.Contains("<h2 class=\"cn_n\" itemprop=\"name\">"))
            {

                int lastIndex;
                fd.ForeignName = ExtractContent(responseBody, "<h2 class=\"cn_n\" itemprop=\"name\">", "<", 0, out lastIndex);
            }
            if (responseBody.Contains("年开业"))
            {
                var details = responseBody;
                var openIndexEnd = details.IndexOf("年开业");
                var roomIndexEnd = details.IndexOf("间房");

                if (openIndexEnd > -1)
                {
                    var start = details.LastIndexOf(">", openIndexEnd) + 2;
                    fd.OpenYear = details.Substring(start, openIndexEnd - start).Trim();
                }
                if (roomIndexEnd > -1)
                {
                    var start = details.LastIndexOf(";", roomIndexEnd) + 1;
                    fd.RoomCount = details.Substring(start, roomIndexEnd - start);
                }

                Console.WriteLine("Got rooms!");

            }
            if (responseBody.Contains(">全部(") && responseBody.Contains(">值得推荐("))
            {
                int lastIndex;
                var basicData = ExtractContent(responseBody, ">全部(", "\"", 0, out lastIndex);
                if (basicData == "")
                    return;


                // crawler
                fd.CommentCount = ExtractContent(responseBody, ">全部(", ")", 0, out lastIndex);
                fd.RecommendCommentCount = ExtractContent(responseBody, ">值得推荐(", ")", 0, out lastIndex);
                fd.NeedToImproveCommentCount = ExtractContent(responseBody, ">有待改善(", ")", 0, out lastIndex);

                var test = ExtractContent(responseBody, "htl_room_txt text_3l ", "<div class=\"introduce_all layoutfix \">", 0, out lastIndex);

                Console.WriteLine("Got comments!");

                // navigate to next
                locked = true;
                NavigateToNext(null);
                locked = false;
            }


        }

        private string GetCommentCount(string responseBody, int number)
        {
            int lastIndex;
            ExtractContent(responseBody, $"<label class=\"row_label\" for=\"com{number}\"", ">", 0, out lastIndex);
            return ExtractContent(responseBody, "row_count\">", "<", lastIndex, out lastIndex);
        }

        private string GetCommentCat(string responseBody, string cat)
        {
            int lastIndex;
            ExtractContent(responseBody, $"{cat}</div>", "<", 0, out lastIndex);
            return ExtractContent(responseBody, "<div class=\"value\">", "<", lastIndex, out lastIndex);
        }

        private void SerializeToFile2()
        {
            Console.WriteLine("Serializing");
            var serializer = new XmlSerializer(typeof(List<ForeignCommentData>));
            TextWriter writer = new StreamWriter(ToLocation);
            serializer.Serialize(writer, ForeignComments);
            writer.Close();
        }

        protected override void NavigateToNext(HtmlAgilityPack.HtmlDocument document)
        {
            if (++NextIndex == Data.Count)
            {
                Console.WriteLine("Done");
                SerializeToFile2();
                return;
            }
            Console.WriteLine(">> Working on = " + NextIndex);

            if (NextIndex >= StartIndex + 2)
            {
                SerializeToFile2();
           
                    //Program.Driver.FindElement(By.TagName("body")).SendKeys(Keys.Escape);
                    //Console.WriteLine("Esc...");
                    //Program.Reset();
                    //Program.Driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(15));
           
            }
            var foreignKey = Data[NextIndex].ForeignKey;
            if (foreignKey != null)
            {
                try
                {
                    Program.Driver.Url = foreignKey;
                    Program.Driver.Navigate();
                    Console.WriteLine("Detail...");
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("Skipped!");
                NavigateToNext(null);
            }

            //if(NextIndex>1)

        }

        internal void Navigate()
        {
            var foreignKey = Data[++NextIndex].ForeignKey;
            SerializeToFile2();
            if (foreignKey != null)
            {
                try
                {
                    Program.Driver.Url = foreignKey;
                    Program.Driver.Navigate();
                    Console.WriteLine("Detail...");
                }
                catch (Exception)
                {
                }
            }
        }
    }


    abstract class Engine
    {
        public List<HotelData> Data { get; set; } = new List<HotelData>();

        public int Type { get; set; }

        public string BaseUrl { get; set; }

        public Engine()
        {
            FiddlerApplication.AfterSessionComplete += AfterSessionComplete;
        }

        public virtual void Start()
        {
            Program.Driver.Url = BaseUrl;
            Program.Driver.Navigate();
        }

        protected string ExtractContent(string rawData, string prefix, string suffix, int startIndex, out int nameNodeEnd)
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

        protected abstract void NavigateToNext(HtmlAgilityPack.HtmlDocument document);

        protected abstract void AfterSessionComplete(Session oSession);

        protected abstract string ToLocation { get; }
        protected abstract string FromLocation { get; }

        protected void SerializeToFile()
        {
            Console.WriteLine("Serializing");
            var serializer = new XmlSerializer(typeof(List<HotelData>));
            TextWriter writer = new StreamWriter(ToLocation);
            serializer.Serialize(writer, Data);
            writer.Close();
        }

        protected virtual void DeserializeFromFile()
        {
            Console.WriteLine("Deserializing");
            FileStream fs = new FileStream(FromLocation, FileMode.OpenOrCreate);
            var serializer = new XmlSerializer(typeof(List<HotelData>));
            Data = serializer.Deserialize(fs) as List<HotelData>;
        }
    }
}
