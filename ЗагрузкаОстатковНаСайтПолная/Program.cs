using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using NFtp;
using NCsv;
using NLog;
using NSettings;


namespace ЗагрузкаОстатковНаСайтПолная
{
    class Program
    {
        static void Main(string[] args)
        {
            CLog log = new CLog(AppDomain.CurrentDomain.BaseDirectory + "log\\");
            log.WriteDelimiter();
            bool b = false;
            CSettings settings = null;
            CFtp ftp = null;
            CCsv vodoparad = null;
            CCsv bitrix = null;
            CCsv for_save = null;
            Dictionary<string, string> bild = new Dictionary<string, string>();            
            



            try
            {
                //-------------------------------------------
                log.WriteMessage("Десериализирую настройки");
                settings = CSettings.Deserialize();
                log.WriteMessage("ok");

                //-------------------------------------------                
                log.WriteMessage("Ищу файл с полной выгрузкой остатков в папке " + settings.local_directory_bild_ostatki);
                string[] allfiles = Directory.GetFiles(settings.local_directory_bild_ostatki);
                //для каждого файла в папке
                foreach (string file in allfiles)
                {
                    //ищем end
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension.ToUpper() == ".END")
                    {
                        FileInfo f_xml = new FileInfo(fileInfo.FullName.ToUpper().Replace(".END", ".XML"));
                        //если нашли end, ищем xml
                        if (f_xml.Exists == true)
                        {                                                        
                            //читаем xml
                            XmlDocument xmlDoc = new XmlDocument(); ;
                            try
                            {
                                xmlDoc.Load(f_xml.FullName);
                            }
                            catch (Exception ex)
                            {
                                log.WriteMessage(" xml файл не открыт: " + ex.Message);
                            }

                            //Берем главый узел
                            XmlNodeList main_node_list = xmlDoc.GetElementsByTagName("Документ");
                            string s = "";
                            //обходим данные в xml файле
                            foreach (XmlElement element in main_node_list)
                            {
                                //Смотрим тип выгрузки
                                s = element.GetAttribute("ОбъемВыгрузки");

                                //нашли файл с полной выгрузкой
                                if (s == "Полная выгрузка")
                                {
                                    //-------------------------------------------
                                    b = true;
                                    log.WriteMessage("Нашел " + f_xml.Name);
                                    log.WriteMessage("Собираю из него информацию");

                                    //обходим строки                
                                    XmlNodeList list = xmlDoc.GetElementsByTagName("Строка");
                                    foreach (XmlElement element2 in list)
                                    {
                                        //берем каждую строку xml
                                        string code = element2.GetAttribute("Код");
                                        string count = element2.GetAttribute("Остаток");
                                        //пишем ее в Dictionary bild
                                        bild.Add(code, count);
                                    }
                                    log.WriteMessage("ok");

                                    //-------------------------------------------
                                    log.WriteMessage("Загружаю файл csv водопарада " + settings.local_directory_data + settings.local_vodoparad_csv);
                                    vodoparad = new CCsv(settings.local_directory_data + settings.local_vodoparad_csv, ';', Encoding.UTF8);
                                    vodoparad.ReadDataToCSV();
                                    log.WriteMessage("ok");

                                    //-------------------------------------------
                                    log.WriteMessage("Загружаю файл csv с сайта " + settings.local_directory_data + settings.local_bitrix_filename);
                                    bitrix = new CCsv(settings.local_directory_data + settings.local_bitrix_filename, ';', Encoding.UTF8);
                                    bitrix.ReadDataToCSV();
                                    log.WriteMessage("ok");

                                    //-------------------------------------------
                                    log.WriteMessage("Формирую файл csv с остатками для загрузки на фтп");
                                    for_save = new CCsv(settings.local_directory_data + settings.local_ostatki_full_filename, ';', Encoding.UTF8);

                                    //формируем заголовок
                                    Dictionary<string, int> headers = new Dictionary<string, int>();
                                    headers.Add("Артикул", 0);
                                    headers.Add("Наличие (служебное)", 1);
                                    for_save.headers = headers;

                                    Dictionary<int, string> data = null;

                                    //для каждой позиции из битрикса
                                    foreach (Dictionary<int, string> row_bitrix in bitrix.data)
                                    {


                                    }//конец для каждой позиции из битрикса


                                    log.WriteMessage("ok");


                                } // конец нашли файл с полной выгрузкой

                            } // конец обходим данные в xml файле

                        } //конец если нашли end, ищем xml

                    } //конец ищем end

                } //конец для каждого файла в папке

                if (b == false)
                {
                    log.WriteMessage("Не нашел");
                }
            }
            catch (Exception exc)
            {
                log.WriteError(exc.Message);
                return;
            }

            //-------------------------------------------
        }
    }
}
