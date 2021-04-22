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

            var responseStringTask = Task.Run(async () => await result.Content.ReadAsStringAsync());


            XmlTextReader reader = new XmlTextReader(text + "sitemap.xml");
            List<string> xmlurls = new List<string>();
            xmlurls.Add(reader.GetAttribute("loc")); // парсинг xml

            Console.WriteLine(responseStringTask.Result);
            Console.ReadKey();

        }

        public static async Task<HttpResponseMessage> sendRequest(string text)
        {
            // Расчет производительности
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(text);
            request1.Accept = "text/html";
            request1.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36";
            Stopwatch timer = new Stopwatch();

            timer.Start();

            HttpWebResponse response = (HttpWebResponse)request1.GetResponse();
            response.Close();

            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            Console.WriteLine(timer);
            // вывод результата

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

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("ms");

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

                    TimeSpan timeTaken1 = timer1.Elapsed;
                    Console.WriteLine(i);
                    Console.WriteLine(timeTaken1);
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
