using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            Console.WriteLine("Введите адрес URL: ");
            string text = Console.ReadLine();
            var task = Task.Run(async () => await sendRequest(text));
            var result = task.Result;

            


            XmlTextReader reader = new XmlTextReader(text + "sitemap.xml");
            List<string> xmlurls = new List<string>();
            xmlurls.Add(reader.GetAttribute("loc")); // парсинг xml

            foreach (string i in xmlurls)
            { if (i == null) continue;
                Console.WriteLine("Ссылки в файле XML");
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

                    Console.Write(i + "|           ");
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
 
            Console.ReadKey();

        }

        public static async Task<HttpResponseMessage> sendRequest(string text)
        {


            // парсинг начальной страницы

            List<string> siteLinks = new List<string>();
            IWebDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            driver.Url = @text;
            var links = driver.FindElements(By.XPath("//a"));
            foreach (IWebElement link in links)
            {
                siteLinks.Add(link.GetAttribute("href"));

            }
            // список ссылок


            // производительность внутренних ссылок

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("     URL                                                      |   ms       ");
            Console.WriteLine("Ссылки на самой странице");
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

            Console.WriteLine("-------------------------------------------------------");
            // end
            
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
