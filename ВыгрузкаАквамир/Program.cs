using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using NSettings;
using NParser;


using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System.Reflection;
using System.IO;

namespace ВыгрузкаАквамир
{
    class Program
    {
        static void Main(string[] args)
        {
            CLog log = new CLog(AppDomain.CurrentDomain.BaseDirectory + "log\\");
            log.WriteDelimiter();
            CSettings settings;
            CParcer_akvamir parser;

            //
            //
            // Если всего одна страница раздела, то page.goods == null
            // решить эту проблему
            //
           // try
            {
                //-------------------------------------------
                log.WriteMessage("Десериализирую настройки");
                settings = CSettings.Deserialize();
                log.WriteMessage("ok");

                log.WriteMessage("Открываю браузер");
                ChromeOptions options = new ChromeOptions();
                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                options.AddArgument("--window-size=1920,1080");
                options.BinaryLocation = settings.chrome_path;
                ChromeDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
                Uri site_url = new Uri("https://akvamir23.ru/");

                //parcer = new CParser.CParser(site_url, settings, driver);

                //rcer.Start();
                log.WriteMessage("ok");


                parser = CParcer_akvamir.LoadXML(settings.local_directory_data + "akvamir.xml", settings);
                parser.SetDriver(driver);
                parser.SetChromeOptions(options);
                //parser = new CParcer_akvamir("https://akvamir23.ru/", settings, driver, options);
                //parser.catalog = new CCatalog_akvamir("https://akvamir23.ru/catalog/");
                parser.ResetDriver();
                bool parce_catalog = false;
                parser.Parce(parce_catalog);
                parser.SaveXML(settings.local_directory_data + "akvamir.xml");
                Console.WriteLine("SaveXML");
                DateTime d = parser.catalog.chapters[0].goods[0].datetime;
                Console.WriteLine(d.Month);
                Console.WriteLine(d.Day);
                //parser.SaveXML(settings.local_directory_data + "akvamir.xml");

                //parser = CParcer_akvamir.LoadXML(settings.local_directory_data + "akvamir.xml", settings);
                
                //CGood_akvamir good = new CGood_akvamir(new Uri("https://akvamir23.ru/product/vanna-orli-1695kh745kh665-radomir/"));
                //driver.Navigate().GoToUrl(good.url);
                //good.Parce(driver);





                parser.Quit();

                Console.ReadKey();
            }
            /*catch (Exception exc)
            {
                log.WriteError(exc.Message);
                Console.WriteLine();
                Console.ReadLine();
                return;
            }*/



            //-------------------------------------------
        }
    }
}
