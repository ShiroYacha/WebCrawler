﻿#define CHROME

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
        public static string chromeDriverDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static string phantomDriverDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static IWebDriver Driver;
        public static int timeoutLoad = 360;

        public static void Reset()
        {
#if CHROME
            Driver = new ChromeDriver(chromeDriverDirectory, new ChromeOptions(), TimeSpan.FromSeconds(20));
#else
            Driver = new ChromeDriver(phantomDriverDirectory, new ChromeOptions(), TimeSpan.FromSeconds(20));
#endif
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

        static void StaticRunner()
        {
            //FilterAllHotelsFromList();

            // start 
            //Setup();
            //Reset();
            //CLeanData();

            //RunPhase0();
            //RunPhase1();
            //RunPhase2();
            //while (true)
            {
                RunPhase3();
                ExportData();

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

        static void StartRunner()
        {
            int mode = 0;
            try
            {
                Console.WriteLine($"Enter 1 to extract links from CTrip, 2 to extract data from links, 3 to do both, 4 to export xml to excel data");
                string line = Console.ReadLine()?.Trim();
                while ((!int.TryParse(line, out mode) || (mode != 1 && mode != 2 && mode != 3 && mode !=4)))
                {
                    Console.WriteLine($"The entered number not 1, 2, 3 or 4，please try again:");
                    line = Console.ReadLine()?.Trim();
                }
                if (mode == 1)
                {
                    RunStep1();
                }
                else if (mode == 2)
                {
                    RunStep2();
                }
                else if (mode == 3)
                {
                    RunStep3();
                }
                else if (mode == 4)
                {
                    ExportData2();
                    Console.WriteLine("Done");
                }
                Console.ReadKey();
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            finally
            {
                if (mode != 4)
                {
                    Driver.Close();
                    Driver.Dispose();
                }
            }

        }

        private static void ExportData2()
        {
            FileStream fsi2 = new FileStream(@"p2.txt", FileMode.Open);
            var serializer2 = new XmlSerializer(typeof(List<ForeignCommentData>));
            var data2 = serializer2.Deserialize(fsi2) as List<ForeignCommentData>;
            fsi2.Close();
            var separator = ";";

            using (FileStream fs = File.Create(@"p3.txt"))
            {
                AddText(fs, "ForeignId;ForeignName;ForeignKey;ForeignNameId;ForeignLocation；Star;CCommentCount;CRecommendCommentCount;CNeedToImproveCommentCount;CPrice;COpenYear;RoomCount;\r\n");
                foreach (var d in data2)
                {

                    if (d != null)
                    {
                        AddText(fs, d.ForeignId + separator);
                        AddText(fs, d.ForeignName + separator);
                        AddText(fs, d.ForeignKey + separator);
                        AddText(fs, d.ForeignNameId + separator);
                        AddText(fs, d.ForeignLocation + separator);
                        AddText(fs, d.Star + separator);
                        AddText(fs, d.CommentCount + separator);
                        AddText(fs, d.RecommendCommentCount + separator);
                        AddText(fs, d.NeedToImproveCommentCount + separator);
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
                        AddText(fs, separator);
                        AddText(fs, separator);
                        AddText(fs, separator);
                    }
                    AddText(fs, "\r\n");
                }
            }
        }

        static void RunStep4()
        {
            
        }

        private double ToRad(double degree)
        {
            return degree*Math.PI/180.0;
        }

        private double GetDistanceInKmFromCoordinates(double lon1d, double lat1d, double lon2d, double lat2d)
        {
            var R = 6371; // km
            var dLat = ToRad(lat2d - lat1d);
            var dLon = ToRad(lon2d - lon1d);
            var lat1 = ToRad(lat1d);
            var lat2 = ToRad(lat2d);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        static void RunStep3()
        {
            RunStep1();
            Reset();
            RunPhase3();
        }

        static void RunStep2()
        {
            Setup();
            Reset();
            RunPhase3();
        }

        static void RunStep1()
        {
            // get todos
            string[] todos;
            using (var filestream = new FileStream("todo.txt", FileMode.Open))
            using (var reader = new StreamReader(filestream))
            {
                todos = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            // get start point
            Console.WriteLine("Please enter the index of the starting hotel (empty = 1 to start from first):");
            int startingHotelIndex = 0;
            string line = Console.ReadLine()?.Trim();
            while (!string.IsNullOrWhiteSpace(line) && (!int.TryParse(line, out startingHotelIndex) || startingHotelIndex - 1 < 0 || startingHotelIndex - 1 >= todos.Length))
            {
                Console.WriteLine($"The entered number is not correct (too big or too small or not a number)，please try again:");
                line = Console.ReadLine()?.Trim();
            }
            startingHotelIndex--;
            if (startingHotelIndex < 0)
                startingHotelIndex = 0;

            // get end point
            Console.WriteLine("Please enter the index of the ending hotel (empty = end):");
            int endingHotelIndex = todos.Length;
            line = Console.ReadLine()?.Trim();
            while (!string.IsNullOrWhiteSpace(line) && (!int.TryParse(line, out endingHotelIndex) || endingHotelIndex < startingHotelIndex || endingHotelIndex >= todos.Length))
            {
                Console.WriteLine($"The entered number is not correct (too big or too small or not a number)，please try again:");
                line = Console.ReadLine()?.Trim();
            }
            endingHotelIndex--;

            // display
            Console.WriteLine($"Starting from {startingHotelIndex+1} until {endingHotelIndex+1} ...");


            // start 
            Setup();
            Reset();
            var engine = new P2Engine()
            {
                StartIndex = - 1,
                ListOfTargets = todos.Skip(startingHotelIndex).Take(endingHotelIndex - startingHotelIndex + 1).ToArray()
            };
            engine.Start();

        }

        static void FilterAllHotelsFromList()
        {
            var targets = FindHotelsFromList();

            FileStream fs = new FileStream("p0.txt", FileMode.OpenOrCreate);
            var serializer = new XmlSerializer(typeof(List<HotelData>));
            var data = serializer.Deserialize(fs) as List<HotelData>;
            fs.Close();

            var newData = data.Where(d => targets.Contains(d.Name)).ToList();
            Console.WriteLine("Serializing");
            TextWriter writer = new StreamWriter("pgps.txt");
            serializer.Serialize(writer, newData);
            writer.Close();
        }

        static string[] FindHotelsFromList()
        {
            using (var filestream = new FileStream("todogps.txt", FileMode.Open))
            using (var reader = new StreamReader(filestream))
            {
                return reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        static void Main(string[] args)
        {
            StaticRunner();
        }
        
        static void RunPhase0()
        {
            // 
            var engine = new P0Engine();
            engine.BaseUrl = @"http://www.tripadvisor.cn/Hotels-g308272-Shanghai-Hotels.html";
            engine.MaxPage = 50;
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

        static void RunPhase3()
        {
            // 
            var engine = new P3Engine();
            engine.Start();
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
            NextIndex = -1;
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
        public int StartIndex = -1;
        public string[] ListOfTargets;
        public List<ForeignCommentData> ForeignComments = new List<ForeignCommentData>();

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
            ForeignComments = ListOfTargets.Select(l=>new ForeignCommentData() { ForeignId = l}).ToList();
            NextIndex = StartIndex;
            NavigateToNext(null);
        }

        private object locker = new object();
        protected override void AfterSessionComplete(Session oSession)
        {

            string responseBody = oSession.GetResponseBodyAsString();
            if (responseBody.Contains("很抱歉，暂时无法找到符合您要求的酒店") && !responseBody.Contains("//    textNoresult.innerHTML = \"<strong>很抱歉，暂时无法找到符合您要求的酒店。</strong>\""))
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
                    var fc = ForeignComments[NextIndex];
                    fc.ForeignKey = commentsLink;
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
            if (++NextIndex == ForeignComments.Count+1)
            {
                Console.WriteLine("Done");
                SerializeToFile2();
                return;
            }
            if(NextIndex>0)
                Console.WriteLine(">> Working on = " + NextIndex);

            if (NextIndex >= StartIndex + 2)
            {
                SerializeToFile2();
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

            var next = $"http://hotels.ctrip.com/hotel/shanghai2/k1{ForeignComments[NextIndex].ForeignId}#ctm_ref=hod_hp_sb_lst";
            try
            {

                Program.Driver.Url = HttpUtility.UrlPathEncode(next);
                Program.Driver.Navigate();
                Console.WriteLine("Moving to next...");
            }
            catch (Exception)
            {
            }
            //if(NextIndex>1)
        }

        protected override void DeserializeFromFile()
        {
            base.DeserializeFromFile();
            FileStream fs = new FileStream(ToLocation, FileMode.OpenOrCreate);
            var serializer = new XmlSerializer(typeof(List<ForeignCommentData>));
            ForeignComments = serializer.Deserialize(fs) as List<ForeignCommentData>;
            StartIndex = ForeignComments.Count - 1;
        }

        private void SerializeToFile2()
        {
            Console.WriteLine("Serializing");
            var serializer = new XmlSerializer(typeof(List<ForeignCommentData>));
            TextWriter writer = new StreamWriter(ToLocation);
            serializer.Serialize(writer, ForeignComments);
            writer.Close();
        }
    }

    class P3Engine : Engine
    {
        private int NextIndex;
        private int StartIndex = -1;
        private int CurrentIndex;
        private bool locked = false;
        private const int TIME_INTERVAL_IN_MILLISECONDS = 15000;
        private static P3Engine _instance;

        private static Timer _timer;

        public P3Engine()
        {
            _instance = this;
        }

        private static void Callback(object state)
        {
            if (_instance.CurrentIndex == _instance.NextIndex)
            {
                try
                {
                    Console.WriteLine("Renavigating...");
                    _instance.NavigateAgain();
                    Console.WriteLine("Renavigated...");
                }
                catch (Exception)
                {
                    Console.WriteLine("Renavigate failed...");
                }
            }
            else
            {
                _instance.CurrentIndex = _instance.NextIndex;
            }
        }

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
            CurrentIndex = NextIndex;
            _timer = new Timer(Callback, null, TIME_INTERVAL_IN_MILLISECONDS, TIME_INTERVAL_IN_MILLISECONDS);
        }

        protected override void DeserializeFromFile()
        {
            FileStream fs = new FileStream(ToLocation, FileMode.OpenOrCreate);
            var serializer = new XmlSerializer(typeof(List<ForeignCommentData>));
            ForeignComments = serializer.Deserialize(fs) as List<ForeignCommentData>;
            StartIndex = -1;
            fs.Close();
        }

        private object locker = new object();
        public List<ForeignCommentData> ForeignComments = new List<ForeignCommentData>();
        protected override void AfterSessionComplete(Session oSession)
        {
            if (locked)
                return;

            string responseBody = oSession.GetResponseBodyAsString();
            ForeignCommentData fd = ForeignComments[NextIndex];
            
            if (responseBody.Contains("<link rel=\"canonical\" href=\""))
            {
                int lastIndex;
                fd.ForeignNameId = ExtractContent(responseBody, "<link rel=\"canonical\" href=\"", "\"", 0, out lastIndex);
            }
            if (responseBody.Contains("_commonHead_lnkMapZone"))
            {
                int lastIndex;
                ExtractContent(responseBody, "_commonHead_lnkMapZone", "\"", 0, out lastIndex);
                fd.ForeignLocation = ExtractContent(responseBody, "target=\"_blank\">", "<", lastIndex, out lastIndex);
            }
            if (responseBody.Contains("<span id=\"ctl00_MainContentPlaceHolder_commonHead_imgStar\" class=\""))
            {
                //<span id="ctl00_MainContentPlaceHolder_commonHead_imgStar" class="
                int lastIndex;
                fd.Star = ExtractContent(responseBody, "<span id=\"ctl00_MainContentPlaceHolder_commonHead_imgStar\" class=\"", "\"", 0, out lastIndex);
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
            if (responseBody.Contains("<span class=\\\"price\\\">"))
            {

                int lastIndex;
                fd.Price = ExtractContent(responseBody, "<span class=\\\"price\\\">", "<", 0, out lastIndex);
                if (!string.IsNullOrEmpty(fd.CommentCount))
                {
                    locked = true;
                    NavigateToNext(null);
                    locked = false;
                    return;
                }


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
                if (string.IsNullOrEmpty(fd.ForeignNameId) || string.IsNullOrEmpty(fd.ForeignName))
                    return;

                if(string.IsNullOrEmpty(fd.Price))
                {
                    if (++noPriceCount < 3)
                    {
                        return;
                    }
                    else
                    {
                        fd.Price = "N/A";
                    }
                }

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
            else if(responseBody.Contains("此酒店暂无点评"))
            {
                // navigate to next
                Console.WriteLine("No comments!");
                locked = true;
                NavigateToNext(null);
                locked = false;
            }


        }

        private static int noPriceCount = 0;

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

        private void NavigateAgain()
        {
            Program.Driver.Url = ForeignComments[NextIndex].ForeignKey;
            Program.Driver.Navigate();
        }

        protected override void NavigateToNext(HtmlAgilityPack.HtmlDocument document)
        {
            if (++NextIndex == ForeignComments.Count+1)
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
            var foreignKey = ForeignComments[NextIndex].ForeignKey;
            if (foreignKey != null)
            {
                try
                {
                    Program.Driver.Url = foreignKey;
                    Program.Driver.Navigate();
                    Console.WriteLine("Detail...");
                    noPriceCount = 0;

                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("Skipped!");
                noPriceCount = 0;

                NavigateToNext(null);
            }

            //if(NextIndex>1)

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
            fs.Close();
        }
    }
}
