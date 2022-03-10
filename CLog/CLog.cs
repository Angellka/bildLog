using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NLog
{
    public class CLog
    {
        /// <summary>
        /// Транслировать записи в лог в консольное приложение
        /// </summary>
        private bool _writeToConsole;
        /// <summary>
        /// Путь к директории с логами
        /// </summary>
        private string _directory;
        /// <summary>
        /// параметр, который показывает были ли за сегодня ошибки, что потом отобразиться в названии файла
        /// как дополнении к нему _ERROR
        /// в конструкторе, если нашли файл за сегодняшний день с поментой _ERROR isError = true
        /// </summary>
        private bool _isError;
        /// <summary>
        /// Текущая дата, по которой находится файл с логами за этот день, иначе создается новый файл
        /// </summary>
        private DateTime _time = DateTime.Now;
        /// <summary>
        /// Файл с логами за сегодняшний день
        /// </summary>
        private FileInfo _file = null;

        /// <summary>
        /// Коснтруктор класса, указывается директория, куда будут сохраняться логи
        /// Логи будут храниться в файлах по алгоритму на каждый новый день новый файл
        /// Именоваться файлы будут по алгоритму ДДММГГ.txt
        /// </summary>
        /// <param name="directory">Директория, куда будут сохраняться файлы с логами</param>        
        public CLog(string directory)
        {
            _writeToConsole = false;
            _directory = directory;

            if (_directory[_directory.Count() - 1] != '\\')
                _directory += "\\";

            //Если директории для логов  нет, то создаем ее
            DirectoryInfo dir = new DirectoryInfo(_directory);
            if (dir.Exists == false)
                dir.Create();

            //Удаляем старые файлы с логами, возрастом более 30 дней
            this.DeleteOldLogs();

            //Ищем файл с логами за сегодняшний день с пометкой _ERROR
            string filename = _directory + _time.Day + "_" + _time.Month + "_" + _time.Year + "_ERROR.txt";            
            _file = new FileInfo(filename);
            if (_file.Exists == true)
            {
                _isError = true;                
            }
            //Если с пометкой _ERROR файл с логами не найден, то ищем обычный файл, иначе создаем новый
            else
            {
                _isError = false;
                filename = filename.Replace("_ERROR", "");
                _file = new FileInfo(filename);
                if (_file.Exists == false)
                {
                    using (File.Create(_file.FullName)) { }
                }                              
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        ~CLog()
        {
            
        }

        /// <summary>
        /// Удаляет старые файлы с логами, которым более 30 дней
        /// </summary>
        private void DeleteOldLogs()
        {
            CLog.DeleteOldFiles(_directory);
        }

        /// <summary>
        /// Удаляет старые файлы из папки, которым более 30 дней
        /// </summary>
        /// <param name="directory">Путь к директории с файлами</param>
        public static void DeleteOldFiles(string directory)
        {            
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {                                
                DateTime d1 = File.GetCreationTime(file);
                DateTime d2 = DateTime.Now;
                if ((d2 - d1).Days > 30)
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolean"></param>
        public void SetWriteToConsole(bool boolean)
        {
            _writeToConsole = boolean;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool WriteMessage(string message)
        {
            //!!!!!!!!!!!!!!!!!!!!!переписать на дополнение строки в существующий файл            
            using (StreamWriter sw = File.AppendText(_file.FullName))
            {
                sw.WriteLine(DateTime.Now.ToString() + " : " + message);
                sw.Close();
            }
            Console.WriteLine(message);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public bool WriteError(string error_message)
        {
            _isError = true;         
            using (StreamWriter sw = File.AppendText(_file.FullName))
            {
                sw.WriteLine(DateTime.Now.ToString() + " ERROR : " + error_message);
                sw.Close();
            }
            Console.WriteLine(error_message);
            //!!!!!!!!!!!!!!!!!!!!!дописать переименование файла в filename_ERROR.txt


            return true;
        }

        /// <summary>
        /// Записывает в лог файл разделительную строку в виде четрочек
        /// </summary>
        /// <returns></returns>
        public bool WriteDelimiter()
        {
            return this.WriteMessage("-----------------------------------");
        }
    }
}
