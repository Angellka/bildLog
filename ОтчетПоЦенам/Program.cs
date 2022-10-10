using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ОтчетПоЦенам
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CExcel excel = new CExcel(@"C:\bild\ИнтернетМагазин\Отчет\Отчет.xls");
            excel.LoadBildPrices(@"C:\bild\ИнтернетМагазин\Отчет\Билд.csv");
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
            excel.LoadAkvamir(@"C:\bild\ИнтернетМагазин\data\akvamir.xml");
            excel.Open();
            excel.Calculate();
            excel.SaveAndQuit();

            //Console.WriteLine("");
            //Console.ReadKey();
        }
    }
}
