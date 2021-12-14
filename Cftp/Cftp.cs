using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NFtp
{
    public class CFtp
    {
        /// <summary>
        /// IP адрес FTP сервера
        /// </summary>
        string _IP;

        /// <summary>
        /// Логин при коннекте к FTP серверу
        /// </summary>
        string _login;

        /// <summary>
        /// Пароль при коннекте к FTP серверу
        /// </summary>
        string _password;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="IP">IP адрес FTP сервера</param>
        /// <param name="login">Логин при коннекте к FTP серверу</param>
        /// <param name="password">Пароль при коннекте к FTP серверу</param>
        public CFtp(string IP, string login, string password)
        {
            _IP = IP;
            _login = login;
            _password = password;
        }

        /// <summary>
        /// Скачать файл с FTP сервера
        /// </summary>
        /// /// <param name="path_ftp">Путь к файлу (Директория + имя файла). Без IP адреса</param>
        /// /// <param name="filename">Полный путь к файлу на локальном диске</param>        
        /// <returns>true - удачно. false - смотри в Exception</returns>
        public bool DownoladFile(string path_ftp, string filename)
        {
            if (path_ftp[0] != '/')
                path_ftp = '/' + path_ftp;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _IP + path_ftp);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            
            request.Credentials = new NetworkCredential(_login, _password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream reader = response.GetResponseStream();

            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            contentLen = reader.Read(buff, 0, buffLength);
            FileInfo fileInf = new FileInfo(filename);
            FileStream strm = fileInf.Create();
            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = reader.Read(buff, 0, buffLength);
            }

            reader.Close();
            response.Close();
            strm.Close();

            return true;
        }

        /// <summary>
        /// Скопировать файл на FTP Сервер
        /// </summary>
        /// /// <param name="filename">Полный путь к файлу на локальном диске</param>
        /// <param name="path_ftp">Путь к файлу (Директория + имя файла). Без IP адреса</param>
        /// <returns>true - удачно. false - смотри в Exception</returns>
        public bool UploadFile(string filename, string path_ftp)
        {
            FileInfo fileInf = new FileInfo(filename);
            FtpWebRequest reqFTP;

            // Создаем объект FtpWebRequest
            if (path_ftp[0] != '/')
                path_ftp = '/' + path_ftp;
            Uri uri = new Uri("ftp://" + _IP + path_ftp);
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);

            // Учетная запись
            reqFTP.Credentials = new NetworkCredential(_login, _password);
            reqFTP.KeepAlive = false;

            // Задаем команду на закачку
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Тип передачи файла
            reqFTP.UseBinary = true;

            // Сообщаем серверу о размере файла
            reqFTP.ContentLength = fileInf.Length;

            // Буффер в 2 кбайт
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Файловый поток
            FileStream fs = fileInf.OpenRead();
            Stream strm = reqFTP.GetRequestStream();

            // Читаем из потока по 2 кбайт
            contentLen = fs.Read(buff, 0, buffLength);

            // Пока файл не кончится
            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = fs.Read(buff, 0, buffLength);
            }

            // Закрываем потоки
            strm.Close();
            fs.Close();

            return true;
        }        
    }
}
