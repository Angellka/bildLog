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

namespace ВыгрузкаВодопарад
{
    [Serializable]
    public class CParcer_vodoparad : CParser
    {
        public CParcer_vodoparad() { }
        public CParcer_vodoparad(string site_url, CSettings settings, WebDriver driver) : base(site_url, settings, driver)
        {
        }

        public override void Parce()
        {
            this.catalog.Parce(driver);
        }

        public void SaveXML(string filename)
        {
            XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(CParser), new Type[] { typeof (CParcer_vodoparad),
                                                                                          typeof (CCatalog_vodoparad),
                                                                                          typeof(CChapter_vodoparad),
                                                                                          typeof(CPage_vodoparad),
                                                                                          typeof(CGood_vodoparad)});
            FileStream fw = new FileStream(filename, FileMode.Create);
            xmlSerialaizer.Serialize(fw, this);
            fw.Close();
        }

        public static CParcer_vodoparad LoadXML(string filename, CSettings settings)
        {
            //XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(CParser));
            XmlSerializer xmlSerialaizer = new XmlSerializer(typeof(CParser), new Type[] { typeof (CParcer_vodoparad),
                                                                                          typeof (CCatalog_vodoparad),
                                                                                          typeof(CChapter_vodoparad),
                                                                                          typeof(CPage_vodoparad),
                                                                                          typeof(CGood_vodoparad)});
            FileStream fr = new FileStream(filename, FileMode.Open);
            CParcer_vodoparad parcer = (CParcer_vodoparad)xmlSerialaizer.Deserialize(fr);
            parcer.settings = settings;
            fr.Close();
            return parcer;
        }
    }

}
