using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NFtp;
using NCsv;
using NLog;
using NSettings;
using NFileSystem;

namespace ЗагрузкаЦенНаСайт
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory_backup = AppDomain.CurrentDomain.BaseDirectory + "backup\\";
            string directory_logs = AppDomain.CurrentDomain.BaseDirectory + "log\\";

            CLog log = new CLog(directory_logs);
            log.WriteDelimiter();

            CSettings settings = null;
            CFtp ftp = null;
            CCsv vodoparad = null;
            CCsv bild = null;
            CCsv for_save = null;
            CCsv bitrix = null;

            //try
            {
                //-------------------------------------------                
                log.WriteMessage("Десериализирую настройки");
                settings = CSettings.Deserialize();
                /*settings = new CSettings();
                settings.local_directory_data = "C:\\bild\\ИнтернетМагазин\\data\\";
                settings.local_directory_bild_price = "C:\\bild\\ИнтернетМагазин\\Цены\\";
                settings.local_vodoparad_csv = "vodoparad.csv";
                settings.local_bitrix_filename = "bitrix_data2.csv";
                settings.local_price_filename = "price2.csv";
                settings.local_brands_filename = "brands.txt";*/
                log.WriteMessage("ok");

                FileInfo f_csv = null;
                FileInfo f_end = null;
                //-------------------------------------------
                bool b = false;
                log.WriteMessage("Ищу файл .end");
                string[] allfiles = Directory.GetFiles(settings.local_directory_bild_price);
                foreach (string file in allfiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension.ToUpper() == ".END")
                    {
                        f_end = fileInfo;
                        log.WriteMessage("Нашел " + fileInfo.Name);                        
                        b = true;
                        break;                    
                    }
                }

                if (b == true) //если файл end найден
                {
                    //-------------------------------------------
                    log.WriteMessage("Ищу файл .csv");
                    foreach (string file in allfiles)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        //если нашел файл csv
                        if (fileInfo.Extension.ToUpper() == ".CSV")
                        {
                            //-------------------------------------------
                            f_csv = fileInfo;
                            log.WriteMessage("Нашел " + fileInfo.Name);
                            log.WriteMessage("Загружаю оттуда цены билда " + fileInfo.FullName);
                            bild = new CCsv(fileInfo.FullName, ';', Encoding.GetEncoding(1251));
                            bild.ReadDataToCSV();
                            log.WriteMessage("ok");
                            // заголовки
                            // 0 - КодТовара
                            // 1 - ЦенаА
                            // 2 - ЦенаБ
                            // 3 - ЦенаВ
                            // 4 - ЦенаГ
                            // 5 - ЦенаР
                            // 6 - ЦенаАкц
                            // 7 - ВидАкции
                            // 8 - Позиция на Я
                            // 9 - Артикул

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
                            log.WriteMessage("Загружаю файл с допустимыми брендами " + settings.local_directory_data + settings.local_brands_filename);
                            string[] brands = File.ReadAllLines(settings.local_directory_data + settings.local_brands_filename);
                            log.WriteMessage("ok");

                            bool find_code = false;
                            bool find_vodoparad = false;
                            bool find_brand = false;
                            bool null_code = true;
                            bool null_vodoparad = true;

                            //-------------------------------------------
                            log.WriteMessage("Формирую файл csv с ценами для сайта");
                            for_save = new CCsv(settings.local_directory_data + settings.local_price_filename, ';', Encoding.UTF8);
                            //формируем заголовки
                            log.WriteMessage("-- формирую заголовки");
                            for_save.headers.Add("ID элемента", 0);
                            for_save.headers.Add("Розничная цена", 1);
                            for_save.headers.Add("Цена со скидкой", 2);
                            for_save.headers.Add("ЦенаА", 3);
                            for_save.headers.Add("ЦенаБ", 4);
                            for_save.headers.Add("ЦенаВ", 5);
                            for_save.headers.Add("Цена со скидкой(служебное)", 6);
                            for_save.headers.Add("Распродажа", 7);
                            //формируем данные
                            log.WriteMessage("-- формирую данные");
                            //для каждой позиции с сайта
                            foreach (Dictionary<int, string> row_bitrix in bitrix.data)
                            {
                                find_code = false;
                                find_vodoparad = false;
                                find_brand = false;
                                null_code = true;
                                null_vodoparad = true;
                                Dictionary<int, string> row_for_save = new Dictionary<int, string>();

                                //----------------------------------------------------------------------------------------
                                //проверяем на позицию на Я
                                //для всех позиций с файла цен билда
                                bool bYa = false;
                                foreach (Dictionary<int, string> row_bild in bild.data)
                                {

                                    //если код товара совпадает
                                    if (row_bitrix[4] == row_bild[0])
                                    {
                                        bYa = false;
                                        //и позиция на я
                                        if (row_bild[8] == "я")
                                        {
                                            //то ставим цену 0
                                            log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Позиция на Я. Ставлю цену ноль. Код товара: " + row_bitrix[4]);
                                            Dictionary<int, string> d = new Dictionary<int, string>();
                                            d.Add(0, row_bitrix[0]);
                                            d.Add(1, "0");
                                            d.Add(2, "0");
                                            d.Add(3, "0");
                                            d.Add(4, "0");
                                            d.Add(5, "0");
                                            d.Add(6, "");
                                            d.Add(7, "Нет");
                                            for_save.data.Add(d);
                                            bYa = true;
                                            continue;
                                        }                                           
                                    }
                                }

                                if (bYa == true) continue;
                                //----------------------------------------------------------------------------------------

                                //если для позиции стоит статус "Обновлять цены - Да"
                                if (row_bitrix[1] == "Да")
                                {
                                    //если для позиции стоит статус "Пометка на удаление - Да"
                                    if (row_bitrix[8] == "Да")
                                    {
                                        //то ставим цену 0
                                        log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Пометка на удаление - Да. Ставлю цену ноль");
                                        Dictionary<int, string> d = new Dictionary<int, string>();
                                        d.Add(0, row_bitrix[0]);
                                        d.Add(1, "0");
                                        d.Add(2, "0");
                                        d.Add(3, "0");
                                        d.Add(4, "0");
                                        d.Add(5, "0");
                                        d.Add(6, "");
                                        d.Add(7, "Нет");
                                        for_save.data.Add(d);
                                        continue;
                                    }
                                        
                                    //проверяем на допуск по бренду
                                    foreach (string brand in brands)
                                    {
                                        if (brand.ToUpper() == row_bitrix[3].ToUpper())
                                        {
                                            find_brand = true;
                                            break;
                                        }
                                    }
                                    //если не прошел по бренду
                                    if (find_brand == false)
                                    {
                                        //ставлю цену 0
                                        log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Не прошел по бренду. Ставлю цену 0. Бренд: " + row_bitrix[3]);
                                        Dictionary<int, string> d = new Dictionary<int, string>();
                                        d.Add(0, row_bitrix[0]);
                                        d.Add(1, "0");
                                        d.Add(2, "0");
                                        d.Add(3, "0");
                                        d.Add(4, "0");
                                        d.Add(5, "0");
                                        d.Add(6, "");
                                        d.Add(7, "Нет");
                                        for_save.data.Add(d);
                                        continue;
                                    }

                                    //если есть код товара
                                    if (row_bitrix[4] != "")
                                    {
                                        null_code = false;
                                        //для всех позиций с файла цен билда
                                        foreach (Dictionary<int, string> row_bild in bild.data)
                                        {
                                            //если код товара совпадает
                                            if (row_bitrix[4] == row_bild[0])
                                            {
                                                find_code = true;
                                                //если цены по акции нет
                                                if (row_bild[6].Replace(',', '.') == "")
                                                {
                                                    //пишем ценуВ
                                                    /*Dictionary<int, string> d = new Dictionary<int, string>();
                                                    d.Add(0, row_bitrix[0]);
                                                    d.Add(1, row_bild[3].Replace(',', '.'));
                                                    d.Add(2, "");
                                                    for_save.data.Add(d);
                                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Записываем новые цены без акции: ЦенаВ " + row_bild[3].Replace(',', '.') + "р. Код товара: " + row_bitrix[4]);*/

                                                    //пишем ценуР
                                                    Dictionary<int, string> d = new Dictionary<int, string>();

                                                    //ID элемента
                                                    d.Add(0, row_bitrix[0]);
                                                    //ЦенаР
                                                    //----------------------------------------------------------------------- СКИДКА НА ВСЕ 12%

                                                    d.Add(1, row_bild[5].Replace(',', '.'));
                                                    //ЦенаАкц
                                                    //double price = Convert.ToDouble(row_bild[5].Replace('.', ','));
                                                    //price *= 0.88;
                                                    //price = Math.Round(price);
                                                    //d.Add(2, Convert.ToString(price).Replace(',', '.'));
                                                    d.Add(2, "0");
                                                    //----------------------------------------------------------------------- СКИДКА НА ВСЕ 12%
                                                    //ЦенаА
                                                    d.Add(3, row_bild[1].Replace(',', '.'));
                                                    //ЦенаБ
                                                    d.Add(4, row_bild[2].Replace(',', '.'));
                                                    //ЦенаВ
                                                    d.Add(5, row_bild[3].Replace(',', '.'));
                                                    //цена со скидкой(служебное)
                                                    d.Add(6, "");
                                                    //Распродажа
                                                    d.Add(7, "Нет");
                                                    for_save.data.Add(d);
                                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Записываем новые цены без акции: ЦенаР " + row_bild[5].Replace(',', '.') + "р. Код товара: " + row_bitrix[4]);
                                                    /*
                                                    d.Add(0, row_bitrix[0]);
                                                    d.Add(1, row_bild[5].Replace(',', '.'));
                                                    double percent = 0.12;
                                                    double price2 = Convert.ToDouble(row_bild[5].Replace('.', ','));
                                                    price2 *= 1 - percent;
                                                    price2 = Math.Ceiling(price2);
                                                    d.Add(2, Convert.ToString(price2));
                                                    for_save.data.Add(d);
                                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Общая акция! ЦенаР " + row_bild[5].Replace(',', '.') + "р. Цена по акции: " + price2 + "р. Код товара: " + row_bitrix[4]);
                                                    */
                                                }
                                                //если есть цена акции
                                                else
                                                {
                                                    //
                                                    //записываем новые цены 
                                                    Dictionary<int, string> d = new Dictionary<int, string>();

                                                    //ID элемента
                                                    d.Add(0, row_bitrix[0]);
                                                    //ЦенаР
                                                    d.Add(1, row_bild[5].Replace(',', '.'));
                                                    //ЦенаАкц
                                                    d.Add(2, row_bild[6].Replace(',', '.'));
                                                    //ЦенаА
                                                    d.Add(3, row_bild[1].Replace(',', '.'));
                                                    //ЦенаБ
                                                    d.Add(4, row_bild[2].Replace(',', '.'));
                                                    //ЦенаВ
                                                    d.Add(5, row_bild[3].Replace(',', '.'));
                                                    //цена со скидкой(служебное)
                                                    d.Add(6, row_bild[6].Replace(',', '.'));
                                                    //Распродажа
                                                    if (row_bild[7] == "Распродажа")
                                                    {
                                                        d.Add(7, "Да");
                                                        log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Записываем новые цены с Распродажей: ЦенаР " + row_bild[5].Replace(',', '.') + "р. ЦенаАкц " + row_bild[6].Replace(',', '.') + "р. Код товара: " + row_bitrix[4]);
                                                    }
                                                    else
                                                    {
                                                        d.Add(7, "Нет");
                                                        log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Записываем новые цены с Акцией: ЦенаР " + row_bild[5].Replace(',', '.') + "р. ЦенаАкц " + row_bild[6].Replace(',', '.') + "р. Код товара: " + row_bitrix[4]);
                                                    }
                                                    for_save.data.Add(d);
                                                    
                                                }
                                                break;
                                            }//конец если код товара совпадает                                        
                                        }//конец для всех позиций с файла цен билда
                                    }
                                    //если нет кода товара, но есть url водопарада
                                    else if (row_bitrix[5] != "")
                                    {
                                        /*null_vodoparad = false;
                                        //для всех позиций с файла цен водопарада
                                        foreach (Dictionary<int, string> row_vodoparad in vodoparad.data)
                                        {
                                            //если URL водопарада совпадают
                                            if (row_bitrix[5] == row_vodoparad[4])
                                            {
                                                find_vodoparad = true;
                                                //записываем новые цены 
                                                Dictionary<int, string> d = new Dictionary<int, string>();
                                                d.Add(0, row_bitrix[0]);
                                                d.Add(1, row_vodoparad[1].Replace(',', '.'));
                                                d.Add(2, "0");
                                                for_save.data.Add(d);
                                                log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Записываем новую цену продажи " + row_vodoparad[1].Replace(',', '.') + ". URL водопарада: " + row_vodoparad[4]);
                                                break;
                                            }//конец если URL водопарада совпадают
                                        }//конец для всех позиций с файла цен водопарада
                                        */
                                    }
                                }//конец если для позиции стоит статус "обновлять цены"
                                else
                                {
                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Стоит статус НЕ обновлять цены");
                                }

                                //если нет ни кода товара, ни url водопарада
                                if (null_code == true && null_vodoparad == true)
                                {
                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Отсутствует Код Товара и URL Водопарада. Цену не трогаю");
                                }
                                else
                                //если информация по ценам не найдена ни в билде, ни на водопараде
                                if (find_code == false && find_vodoparad == false)
                                {
                                    log.WriteMessage("ID Элемента: " + row_bitrix[0] + " Информация по ценам из источников НЕ НАЙДЕНА. Ставлю цену 0");
                                    Dictionary<int, string> d = new Dictionary<int, string>();

                                    d.Add(0, row_bitrix[0]);
                                    d.Add(1, "0");
                                    d.Add(2, "0");
                                    d.Add(3, "0");
                                    d.Add(4, "0");
                                    d.Add(5, "0");
                                    d.Add(6, "");
                                    d.Add(7, "Нет");
                                    for_save.data.Add(d);
                                }
                            }//конец для каждой позиции с сайта
                             //конец формирования файла для записи
                            log.WriteMessage("ok");

                            //-------------------------------------------
                            log.WriteMessage("Записываю файл csv с ценами для сайта " + settings.local_directory_data + settings.local_price_filename);
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
                            log.WriteMessage(settings.ftp_directory + settings.ftp_price_filename);
                            ftp.UploadFile(for_save.GetFileFullName(), settings.ftp_directory + settings.ftp_price_filename);
                            log.WriteMessage("ok");

                            //-------------------------------------------
                            log.WriteMessage("Перемещаю рабочие файлы с ценами в папку backup");                            
                            CFileSystem.CreateDirectoryIfPossible(directory_backup);
                            log.WriteMessage(f_csv.FullName);
                            CFileSystem.RemoveFile(f_csv.FullName, directory_backup + f_csv.Name);
                            log.WriteMessage("ok");
                            log.WriteMessage(f_end.FullName);
                            CFileSystem.RemoveFile(f_end.FullName, directory_backup + f_end.Name);
                            log.WriteMessage("ok");
                            log.WriteMessage(for_save.GetFileFullName());
                            log.WriteMessage("..и переименовываю price.csv в формат даты");
                            CFileSystem.RemoveFile(for_save.GetFileFullName(), directory_backup + "price_" + f_csv.Name);
                            log.WriteMessage("ok");

                        }//конец если нашел файл csv
                    }//конец поиска файла csv
                }//если файл end найден
                else
                {
                    log.WriteMessage("Файл .end не найден");
                }
                //-------------------------------------------
            }
            /*catch (Exception exc)
            {
                log.WriteError(exc.Message);
                Console.WriteLine();
                Console.ReadLine();
                return;
            }*/
            //zapromo.ru
            //-------------------------------------------
        }
    }
}
