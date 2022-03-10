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
using NFileSystem;

namespace ЗагрузкаОстатковНаСайтПолная
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory_backup = AppDomain.CurrentDomain.BaseDirectory + "backup\\";
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
                bool bFind_Full_File = false;
                //для каждого файла в папке
                foreach (string file in allfiles)
                {
                    //ищем end
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension.ToUpper() == ".END")
                    {
                        FileInfo f_end = fileInfo;
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
                                    bFind_Full_File = true;
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
                                    // заголовки
                                    // 0 - артикул
                                    // 1 - цена
                                    // 2 - наличие
                                    // 3 - наименование
                                    // 4 - url водопарада

                                    //-------------------------------------------
                                    log.WriteMessage("Загружаю файл csv с сайта " + settings.local_directory_data + settings.local_bitrix_filename);
                                    bitrix = new CCsv(settings.local_directory_data + settings.local_bitrix_filename, ';', Encoding.UTF8);
                                    bitrix.ReadDataToCSV();
                                    log.WriteMessage("ok");
                                    // заголовки
                                    // 0 - ID элемента
                                    // 1 - Обновлять цены
                                    // 2 - Наименование элемента
                                    // 3 - Производитель
                                    // 4 - Код товара
                                    // 5 - url водопарада
                                    // 6 - Розничная цена
                                    // 7 - Цена товара со скидкой
                                    // 8 - Пометка на удаление

                                    //-------------------------------------------
                                    log.WriteMessage("Формирую файл csv с остатками для загрузки на фтп");
                                    for_save = new CCsv(settings.local_directory_data + settings.local_ostatki_full_filename, ';', Encoding.UTF8);

                                    //формируем заголовок
                                    Dictionary<string, int> headers = new Dictionary<string, int>();
                                    headers.Add("ID", 0);
                                    headers.Add("Наличие (служебное)", 1);
                                    headers.Add("Наличие", 2);
                                    for_save.headers = headers;

                                    Dictionary<int, string> data = null;
                                    
                                    //для каждой позиции из битрикса
                                    foreach (Dictionary<int, string> row_bitrix in bitrix.data)
                                    {
                                        bool bFindBild = false;
                                        bool bFindVodoparad = false;

                                        //если есть код товара
                                        if (row_bitrix[4] != "")
                                        {
                                            bFindBild = true; //Запущен алгоритм поиска по коду товара
                                            //пытаемся по коду товара найти наличие в xml файле из 1С
                                            if (bild.ContainsKey(row_bitrix[4]) == true)
                                            {                                                 
                                                string count = bild[row_bitrix[4]];
                                                //если наличие не равно 0
                                                if (count != "0")
                                                {
                                                    Dictionary<int, string> d = new Dictionary<int, string>();
                                                    d.Add(0, row_bitrix[0]); // ID
                                                    d.Add(1, "В наличии");   // Наличие(служебное)
                                                    d.Add(2, "В наличии");   // Наличие
                                                    for_save.data.Add(d);                                                    
                                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Код товара: " + row_bitrix[4] + " Наличие 1С: " + count + " Статус: В наличии");
                                                }
                                                else
                                                //если наличие равно 0
                                                {
                                                    Dictionary<int, string> d = new Dictionary<int, string>();
                                                    d.Add(0, row_bitrix[0]); // ID
                                                    d.Add(1, "Под заказ");   // Наличие(служебное)
                                                    d.Add(2, "Под заказ");   // Наличие
                                                    for_save.data.Add(d);                                                    
                                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Код товара: " + row_bitrix[4] + " Наличие 1С: " + count + " Статус: Под заказ");
                                                }
                                            }
                                            //если по коду товара не нашли остатки в 1С
                                            else
                                            {                                                                                                
                                                Dictionary<int, string> d = new Dictionary<int, string>();
                                                d.Add(0, row_bitrix[0]); // ID
                                                d.Add(1, "Под заказ");   // Наличие(служебное)
                                                d.Add(2, "Под заказ");   // Наличие
                                                for_save.data.Add(d);
                                                log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Код товара: " + row_bitrix[4] + " В файле 1С не нашли остаток !!! Ставлю под заказ");
                                            }
                                        }//конец если есть код товара

                                        //если по коду товара не нашли
                                        if (bFindBild == false)
                                        {
                                            //если есть url водопарада
                                            if (row_bitrix[5] != "")
                                            {
                                                //для всех строк из файла водопарада
                                                foreach (Dictionary<int, string> row_vodoparad in vodoparad.data)
                                                {

                                                    if (row_bitrix[5] == row_vodoparad[4])
                                                    {
                                                        Dictionary<int, string> d = new Dictionary<int, string>();
                                                        d.Add(0, row_bitrix[0]); // ID
                                                        d.Add(1, "В наличии на удаленном складе");   // Наличие(служебное)
                                                        d.Add(2, "В наличии");   // Наличие
                                                        for_save.data.Add(d);
                                                        log.WriteMessage("ID Элемента: " + row_bitrix[0] + " URL водопарада: " + row_bitrix[5] + " Статус: В наличии на удаленном складе");
                                                        bFindVodoparad = true;
                                                        break;
                                                    }
                                                }
                                            }//если есть url водопарада
                                        }//конец если по коду товара не нашли

                                        //если нет ни кода товара ни url водопарада, то ставим Под заказ
                                        if (bFindBild == false && bFindVodoparad == false)
                                        {
                                            Dictionary<int, string> d = new Dictionary<int, string>();
                                            d.Add(0, row_bitrix[0]); // ID
                                            d.Add(1, "Под заказ");   // Наличие(служебное)
                                            d.Add(2, "Под заказ");   // Наличие
                                            for_save.data.Add(d);
                                            log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Нет кода товара и URL водопарада. Статус: Под заказ");
                                        }

                                    }//конец для каждой позиции из битрикса


                                    log.WriteMessage("ok");

                                    //-------------------------------------------
                                    log.WriteMessage("Записываю файл csv с ценами для сайта " + settings.local_directory_data + settings.local_ostatki_full_filename);
                                    for_save.WriteDataToCSV();
                                    log.WriteMessage("ok");

                                    //-------------------------------------------
                                    log.WriteMessage("Подключаюсь к фтп " + settings.ftp_ip);
                                    ftp = new CFtp(settings.ftp_ip, settings.ftp_login, settings.ftp_password);
                                    log.WriteMessage("ok");

                                    //-------------------------------------------
                                    log.WriteMessage("Копирую файл с ценами на фтп");
                                    log.WriteMessage("Отсюда");
                                    log.WriteMessage(for_save.GetFileFullName());
                                    log.WriteMessage("Сюда");
                                    log.WriteMessage(settings.ftp_directory + settings.ftp_ostatki_full_filename);
                                    ftp.UploadFile(for_save.GetFileFullName(), settings.ftp_directory + settings.ftp_ostatki_full_filename);
                                    log.WriteMessage("ok");

                                    //-------------------------------------------
                                    log.WriteMessage("Перемещаю рабочие файлы с остатками в папку backup");
                                    CFileSystem.CreateDirectoryIfPossible(directory_backup);
                                    log.WriteMessage(f_xml.FullName);
                                    CFileSystem.RemoveFile(f_xml.FullName, directory_backup + f_xml.Name);
                                    log.WriteMessage("ok");
                                    log.WriteMessage(f_end.FullName);
                                    CFileSystem.RemoveFile(f_end.FullName, directory_backup + f_end.Name);
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
                Console.WriteLine();
                Console.ReadLine();
                return;
            }

            //-------------------------------------------
        }
    }
}
