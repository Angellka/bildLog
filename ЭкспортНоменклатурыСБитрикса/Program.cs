using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NFtp;
using NLog;
using NSettings;
namespace ЭкспортНоменклатурыСБитрикса
{
    class Program
    {
        static void Main(string[] args)
        {
            CLog log = new CLog(AppDomain.CurrentDomain.BaseDirectory + "log\\");
            log.WriteDelimiter();


            CSettings settings = null;
            CFtp ftp = null;
            

            try
            {
                //-------------------------------------------
                log.WriteMessage("Десериализирую настройки");
                settings = CSettings.Deserialize();
                log.WriteMessage("ok");

                //-------------------------------------------
                log.WriteMessage("Подключаюсь к фтп");
                ftp = new CFtp(settings.ftp_ip, settings.ftp_login, settings.ftp_password);
                log.WriteMessage("ok");

                //-------------------------------------------
                log.WriteMessage("Качаю файл с фтп");
                log.WriteMessage("Из: " + settings.ftp_directory + settings.ftp_bitrix_filename);
                log.WriteMessage("В: " + settings.local_directory_data + settings.local_bitrix_filename);
                ftp.DownoladFile(settings.ftp_directory + settings.ftp_bitrix_filename, settings.local_directory_data + settings.local_bitrix_filename);
                log.WriteMessage("ok");
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
