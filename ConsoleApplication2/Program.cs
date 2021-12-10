using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bildLog;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            CLog log = new CLog("c:\\logs");
            log.WriteMessage("тест");
            Console.ReadKey();
        }
    }
}
