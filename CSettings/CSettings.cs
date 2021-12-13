using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSettings
{
    public class CSettings
    {        
        private static string filename_settings = "C:\\Bild\\ИнтернетМагазин\\Data\\Settings.xml";

        /// <summary>
        /// IP адрес FTP сервера интернет-магазина
        /// </summary>
        public string ftp_ip { get; set; }

        /// <summary>
        /// Логин к FTP серверу
        /// </summary>
        public string ftp_login { get; set; }

        /// <summary>
        /// Пароль к FTP серверу
        /// </summary>
        public string ftp_password { get; set; }

        /// <summary>
        /// Локальная директория всех файлов для работы
        /// </summary>
        public string bild_directory { get; set; }

        /// <summary>
        /// Директория на FTP сервере куда кидать и откуда брать файлы для работы
        /// </summary>
        public string ftp_directory { get; set; }

        /// <summary>
        /// Имя файла с ценами для сайта, который загружается на FTP сервер
        /// </summary>
        public string price_csv { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ftp_filename { get; set; }

        /// <summary>
        /// Имя файла, который формируется парсером Водопарада, хранит в себе информацию о карточках товаров
        /// с водопарада
        /// </summary>
        public string vodoparad_csv { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string directory_bild_price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string bitrix_data { get; set; }

        /// <summary>
        /// Файл с допущенными брендами на сайт, если при формировании цен бренд товара не находится в этом списке,
        /// то для него ставится цена 0, все позиции на сайте с ценами 0 становятся неактивными
        /// </summary>
        public string brands_filename { get; set; }

        /// <summary>
        /// Десериализация настроек из файла, путь к которому указан был в конструкторе
        /// </summary>
        /// <param name="settings">объект класса CSettings куда будут десирализованны настройки из файла</param>
        public static void Deserialize(CSettings settings)
        {            
            XmlSerializer formatter = new XmlSerializer(typeof(CSettings));
            using (FileStream fs = new FileStream(filename_settings, FileMode.Open))
            {
                settings = (CSettings)formatter.Deserialize(fs);                
            }
        }
    }
}
