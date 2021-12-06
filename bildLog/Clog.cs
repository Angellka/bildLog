using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace bildLog
{
    public class CLog
    {
        /// <summary>
        /// Путь к директории с логами
        /// </summary>
        private string directory;
        /// <summary>
        /// параметр, который показывает были ли за сегодня ошибки, что потом отобразиться в названии файла
        /// как дополнении к нему _ERROR
        /// в конструкторе, если нашли файл за сегодняшний день с поментой _ERROR isError = true
        /// </summary>
        private bool isError;
        /// <summary>
        /// Текущая дата, по которой находится файл с логами за этот день, иначе создается новый файл
        /// </summary>
        private DateTime time = DateTime.Now;

        /// <summary>
        /// Файл с логами за сегодняшний день
        /// </summary>
        private FileInfo file = null;

        /// <summary>
        /// Коснтруктор класса, указывается директория, куда будут сохраняться логи
        /// Логи будут храниться в файлах по алгоритму на каждый новый день новый файл
        /// Именоваться файлы будут по алгоритму ДДММГГ.txt
        /// </summary>
        /// <param name="directory">Директория, куда будут сохраняться файлы с логами</param>        
        public CLog(string directory)
        {
            this.directory = directory;

            //Удаляем старые файлы с логами, возрастом более 30 дней
            this.DeleteOldLogs();

            //Если директории для логов  нет, то создаем ее
            DirectoryInfo dir = new DirectoryInfo(directory);
            if (dir.Exists == false)
                dir.Create();


            //Ищем файл с логами за сегодняшний день с пометкой _ERROR
            string filename = directory + time.Day + time.Month + time.Year + "_ERROR.txt";            
            file = new FileInfo(filename);
            if (file.Exists == true)
            {
                this.isError = true;
                file.Open(FileMode.Open, FileAccess.Write);
            }
            //Если с пометкой _ERROR файл с логами не найден, то ищем обычный файл, иначе создаем новый
            else
            {
                filename = filename.Replace("_ERROR", "");
                file = new FileInfo(filename);
                file.Open(FileMode.OpenOrCreate, FileAccess.Write);
            }            
        }

        /// <summary>
        /// Деструктор, закрывает файл для текущих логов
        /// </summary>
        ~CLog()
        {
            
        }

        /// <summary>
        /// Удаляет старые файлы с логами, которым более 30 дней
        /// </summary>
        private void DeleteOldLogs()
        {
            CLog.DeleteOldFiles(this.directory);
        }

        /// <summary>
        /// Удаляет старые файлы из папки, которым более 30 дней
        /// </summary>
        /// <param name="directory">Путь к директории с файлами</param>
        public static void DeleteOldFiles(string directory)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool WriteMessage(string message)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error_message"></param>
        /// <returns></returns>
        public bool WriteError(string error_message)
        {
            return true;
        }        
    }
}
