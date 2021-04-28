using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите адрес URL: ");
            var text = Console.ReadLine();
          
            string XmlURL = text + "sitemap.xml";

            List<string> xmlurls = new List<string>();
            try
            {
                XmlTextReader reader = new XmlTextReader(XmlURL);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text) { 
                       
                                
                                xmlurls.Add(reader.Value); // парсинг xml
                                Console.WriteLine(reader.Value);
                               
                           
                    }
                }
            }
            catch (XmlException e)
            {
                Console.WriteLine("###error: " + e.Message);
            }
            catch (Exception generalException)
            {
                Console.WriteLine("Нет xml файла");
            }



            var task = Task.Run(async () => await sendRequest(text,xmlurls));
            var result = task.Result; 
            Console.ReadKey();
        }


        public static async Task<HttpResponseMessage> sendRequest(string text,List <string> xmlurls)
        {


            // парсинг начальной страницы
            int countxml=0;
            int counturls=0;
            List<string> siteLinks = new List<string>();
            List<string> webLinks = new List<string>();
            
            IWebDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            driver.Url = @text;
            var links = driver.FindElements(By.XPath("//a"));
            foreach (IWebElement link in links)
            {
                webLinks.Add(link.GetAttribute("href"));

            }

            // список ссылок








            if (xmlurls.Count() < 1) 
            {
                siteLinks = webLinks;
                Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
                Console.WriteLine("URL");
                foreach (string u in siteLinks)
                {
                    if (u == null) continue;
                    Console.WriteLine(u);
                    counturls++;
                }
            }
            else if (xmlurls.Count()>1)
            {
                Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");
                Console.WriteLine("URL");
                foreach (string u in xmlurls.Except(webLinks))
                {
                    if (u == null) continue;
                    Console.WriteLine(u);
                    countxml++;
                }
                Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
                Console.WriteLine("URL");
                foreach (string u in webLinks.Except(xmlurls))
                {
                    if (u == null) continue;
                    Console.WriteLine(u);
                    counturls++;
                }
                siteLinks = new List<string>(xmlurls.Union(webLinks));
            }
            // производительность внутренних ссылок

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("     URL                                                      |   ms       ");
            foreach (string i in siteLinks)
            {
                if (i == null) continue;

                try
                {
                    HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(i);
                    request2.Accept = "text/html";
                    request2.UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36";

                    Stopwatch timer1 = new Stopwatch();

                    timer1.Start();
                    HttpWebResponse response1 = (HttpWebResponse)request2.GetResponse();
                    response1.Close();

                    timer1.Stop();


                    Console.Write(i+"|           ");
                    Console.WriteLine(timer1.ElapsedMilliseconds);
                }
                catch (WebException e)
                {
                }
                catch (Exception generalException)
                {
                    Console.WriteLine(generalException);
                }
            }


            Console.WriteLine("----------------------------------------------------------------------------------------");
            // end
       
            Console.WriteLine("Urls found in sitemap: " + countxml);
            Console.WriteLine("Urls found after crawling a website: "+ counturls);
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(text);
            request.Method = HttpMethod.Get;
            request.Headers.Add("accept", "text/html");                  
            request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36");

            return await httpClient.SendAsync(request);
        }
    }
}
