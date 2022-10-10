using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NSettings
{
    public class CSettings
    {        
        private static string filename_settings = "C:\\Bild\\ИнтернетМагазин\\Data\\Settings.xml";

        public string ftp_ip { get; set; }

        public string ftp_login { get; set; }

        public string ftp_password { get; set; }
        
        public string ftp_directory { get; set; }

        public string ftp_bitrix_filename { get; set; }

        public string ftp_price_filename { get; set; }

        public string ftp_ostatki_full_filename { get; set; }

        public string ftp_ostatki_filename { get; set; }

        public string local_directory_data { get; set; }

        public string local_bitrix_filename { get; set; }

        public string local_price_filename { get; set; }

        public string local_ostatki_full_filename { get; set; }

        public string local_ostatki_filename { get; set; }

        public string local_directory_bild_price { get; set; }

        public string local_directory_bild_ostatki { get; set; }

        public string local_vodoparad_csv { get; set; }

        public string local_brands_filename { get; set; }

        public string chrome_path { get; set; }

        /// <summary>
        /// Десериализация настроек из файла, путь к которому указан был в конструкторе
        /// </summary>
        /// <param name="settings">объект класса CSettings куда будут десирализованны настройки из файла</param>
        public static CSettings Deserialize()
        {            
            XmlSerializer formatter = new XmlSerializer(typeof(CSettings));
            using (FileStream fs = new FileStream(filename_settings, FileMode.Open))
            {
                return (CSettings)formatter.Deserialize(fs);                
            }
        }
    }
}
