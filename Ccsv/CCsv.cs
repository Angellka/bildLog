using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCsv
{
    public class CCsv
    {
        Encoding _encoding;
        /// <summary>
        /// Полный путь к файлу csv
        /// </summary>
        string _filename;

        /// <summary>
        /// Символ разделителя между значениями в файле
        /// </summary>
        char _delimiter;

        /// <summary>
        /// Содержит в себе текст ошибки, если она произошла. Используется в функции чтения файла ReadDataToCSV()
        /// </summary>
        string _error;
        
        /// <summary>
        /// Заголовки данных файла. Первая строка файла
        /// </summary>
        public Dictionary<string, int> headers = new Dictionary<string, int>();

        /// <summary>
        /// Данные csv файла
        /// При этом ключ доступа к столбцу формируется из Dictionary headers
        /// </summary>         
        public List<Dictionary<int, string>> data = new List<Dictionary<int, string>>();

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="fileFullName">Полный путь к файлу csv</param>
        /// <param name="delimiter">Символ разделителя между значениями в файле</param>
        public CCsv(string fileFullName, char delimiter, Encoding encoding)
        {
            _filename = fileFullName;
            _delimiter = delimiter;
            _encoding = encoding;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFileFullName()
        {
            return _filename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            return (new FileInfo(_filename)).Name;
        }

        /// <summary>
        /// Читает содержимое файла, указанного в конструкторе и записывает заголовки в headers, а данные в data
        /// </summary>
        public void ReadDataToCSV()
        {
            int i;
            //читаем файл
            string[] arStr = File.ReadAllLines(_filename, _encoding);
            if (arStr == null || arStr.Count() == 0)
            {
                throw new Exception("Функция CCsv.ReadDataToCsv() Файл не содержит данных");
            }

            //читаем заголовок и заносим его в headers
            string s = arStr[0];
            string[] split = s.Split(_delimiter);
            i = 0;
            foreach (string splt in split)
            {
                headers.Add(splt.Trim('\"'), i); // удаляем кавычки в заголовках
                i++;
            }

            //читаем строки csv файла и заносим их в data
            //ключ доступа к ячейке строки данных формируем из headers                
            for (i = 1; i < arStr.Length; i++)
            {
                Dictionary<int, string> row = new Dictionary<int, string>();
                s = arStr[i];
                split = s.Split(_delimiter);
                int j = 0;
                foreach (string splt in split)
                {
                    row.Add(j, splt.Trim('\"')); // удаляем кавычки в данных
                    j++;
                }
                //Добавляем пустое значение в конец каждого словаря на случай, если в файле в конце строки не стоит знак разделителя
                //это костыль, надо попросить Юрия Андреевича поставить точки с запятой в столбец Цена акций, где нет значений
                row.Add(j, "");

                data.Add(row);
            }
        }

        /// <summary>
        /// Записывает файл CSV из headers и data в файл
        /// </summary>
        /// <param name="filename">Полный путь к файлу с расширением</param>
        public void WriteDataToCSV(string filename)
        {
            //открываем файл для записи
            StreamWriter SW = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), Encoding.UTF8);            

            //записываем заголовки
            string h = "";
            foreach (KeyValuePair<string, int> pair in headers)
            {
                h += pair.Key + _delimiter;
            }
            SW.WriteLine(h);

            //записываем данные
            string d = "";
            foreach (Dictionary<int, string> row in data)
            {
                d = "";
                foreach(KeyValuePair<int, string> pair in row)
                {
                    d += pair.Value + _delimiter;
                }
                SW.WriteLine(d);
            }            

            //закрываем файл
            SW.Close();
        }

        /// <summary>
        /// Записывает файл CSV из headers и data в файл, путь к которому указан был в конструкторе
        /// </summary>
        public void WriteDataToCSV()
        {
            this.WriteDataToCSV(_filename);
        }
    }
}
