using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            CLog.CLog log = new CLog.CLog("c:\\test\\logs");
            log.WriteMessage("тест");

            CCsv.CCsv csv = new CCsv.CCsv("c:\\test\\test.csv", ';');
            csv.ReadDataToCSV();

            csv.WriteDataToCSV("c:\\test\\test2.csv");

            Console.ReadKey();
        }
    }
}
