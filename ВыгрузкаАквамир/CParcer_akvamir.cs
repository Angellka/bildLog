using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NParser;
using NSettings;
using OpenQA.Selenium;
using System.Xml.Serialization;
using System.IO;
using OpenQA.Selenium.Chrome;
using System.Timers;


namespace ВыгрузкаАквамир
{
    [Serializable]
    public class CParcer_akvamir : CParser
    {
        public CParcer_akvamir() { }
        public CParcer_akvamir(string site_url, CSettings settings, WebDriver driver, ChromeOptions chrome_options) : base(site_url, settings, driver, chrome_options)
        {
        }


        private Timer aTimer;

        private bool isStoped = false;

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(300000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (isStoped == true)
            {
                Console.WriteLine("Driver stoped! Reset!");
                this.ResetDriver();
            }
            else
            {
                Console.WriteLine("Is no stopped. All right!");
            }

            isStoped = true;            
        }

        public override void Parce(bool parce_catalog)
        {
            
            int i = 0;
            if (parce_catalog == true)
                this.catalog.Parce(driver);
            
            //List<CGood> g = this.Goods();
            //chapters[1].Parce(driver);

            SetTimer();
            List<CGood> goods = catalog.Goods();
            foreach (CGood good in goods)
            {
                if (good.datetime.Month != DateTime.Now.Month
                    &&
                    good.datetime.Day != DateTime.Now.Day)
                {
                    good.Parce(driver);                    

                    Console.WriteLine(good.ToString());
                    i++;
                    if (i > 10)
                    {
                        i = 0;
                        this.SaveXML(settings.local_directory_data + "akvamir.xml");
                        Console.WriteLine("SaveXML");
                    }
                }

                isStoped = false;
            }

            aTimer.Stop();
            aTimer.Dispose();
        }

        public void SaveXML(string filename)
        {
            XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(CParser),new Type[] { typeof (CParcer_akvamir),
                                                                                          typeof (CCatalog_akvamir),
                                                                                          typeof(CChapter_akvamir),
                                                                                          typeof(CPage_akvamir),
                                                                                          typeof(CGood_akvamir)});
            FileStream fw = new FileStream(filename, FileMode.Create);
            xmlSerialaizer.Serialize(fw, this);
            fw.Close();
        }

        public static CParcer_akvamir LoadXML(string filename, CSettings settings)
        {
            //XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(CParser));
            XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(CParser), new Type[] { typeof (CParcer_akvamir),
                                                                                          typeof (CCatalog_akvamir),
                                                                                          typeof(CChapter_akvamir),
                                                                                          typeof(CPage_akvamir),
                                                                                          typeof(CGood_akvamir)});
            FileStream fr = new FileStream(filename, FileMode.Open);
            CParcer_akvamir parcer = (CParcer_akvamir)xmlSerialaizer.Deserialize(fr);
            parcer.settings = settings;
            fr.Close();
            return parcer;
        }
    }
}
