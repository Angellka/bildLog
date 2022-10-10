using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

using NSettings;

using System.IO;
using System.Reflection;
using System.Xml.Serialization;


namespace NParser
{    
    [Serializable]
    public abstract class CParser
    {
        [XmlIgnore]
        public WebDriver driver;
        [XmlIgnore]
        public ChromeOptions chrome_options;

        public CCatalog catalog;
        public CSettings settings;
        public string site_url;

        public CParser() { }

        public CParser(string site_url, CSettings settings, WebDriver driver, ChromeOptions chrome_options)
        {
            this.site_url = site_url;
            this.settings = settings;
            this.driver = driver;
            this.chrome_options = chrome_options;
        }

        public abstract void Parce(bool parce_catalog);

        public void SetDriver(WebDriver driver)
        {
            this.driver = driver;
        }

        public void SetChromeOptions(ChromeOptions options)
        {
            this.chrome_options = options;
        }


        public void ResetDriver()
        {
            if (driver != null)
                driver.Quit();
            driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chrome_options);
        }

        public void Quit()
        {
            driver.Quit();
        }
    }
}
