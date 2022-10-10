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

namespace ВыгрузкаВодопарад
{
    class Program
    {
        static void Main(string[] args)
        {
            CLog log = new CLog(AppDomain.CurrentDomain.BaseDirectory + "log\\");
            log.WriteDelimiter();
            CSettings settings;

            CParcer_vodoparad parser;

            //try
            {
                //109.194.101.128	3128
                
                //-------------------------------------------
                log.WriteMessage("Десериализирую настройки");
                settings = CSettings.Deserialize();
                log.WriteMessage("ok");

                log.WriteMessage("Открываю браузер");
                ChromeOptions options = new ChromeOptions();
                //proxy--------------------
                /*Proxy p = new Proxy();
                var url = "109.194.101.128:3128";
                p.HttpProxy = url;
                p.SslProxy = url;
                options.Proxy = p;
                options.AddArgument("ignore-certificate-errors");*/
                //-------------------------

                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                options.AddArgument("--window-size=1920,1080");
                options.BinaryLocation = settings.chrome_path;
                ChromeDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
                Uri site_url = new Uri("https://vodoparad.ru/");

                //----------------ручная система обхода антиддос
                driver.Navigate().GoToUrl(site_url);
                log.WriteMessage("Открываю браузер");
                log.WriteMessage("Обойдите антиддос и нажмите enter");
                Console.ReadLine();

                
                log.WriteMessage("ok");
                parser = new CParcer_vodoparad("https://vodoparad.ru/", settings, driver);
                parser.catalog = new CCatalog_vodoparad("https://www.vodoparad.ru/catalog/santekhnika.html");
                parser.Parce();
                parser.SaveXML(settings.local_directory_data + "vodoparad2.xml");

                driver.Quit();

                Console.ReadKey();
            }
            /*catch (Exception exc)
            {
                log.WriteError(exc.Message);
                Console.WriteLine();
                Console.ReadLine();
                return;
            }*/
        }
    }
}
