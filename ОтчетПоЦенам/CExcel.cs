using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NCsv;
using ВыгрузкаАквамир;
using NParser;
using System.IO;
using System.Drawing;

namespace ОтчетПоЦенам
{
    public class CExcel
    {
        int all_count = 0;
        int great_price_count = 0;
        int bad_price_count = 0;

        private int row_index = 8;

        public string filename;
        public CCsv bild;
        public CParcer_akvamir akvamir;
        public List<CMatch> matches_akvair;

        Microsoft.Office.Interop.Excel.Application ex = new Microsoft.Office.Interop.Excel.Application();
        Microsoft.Office.Interop.Excel.Workbook workbook;

        Microsoft.Office.Interop.Excel.Worksheet sheet1;
        Microsoft.Office.Interop.Excel.Worksheet sheet2;
        Microsoft.Office.Interop.Excel.Worksheet sheet3;
        Microsoft.Office.Interop.Excel.Worksheet sheet4;

        //--------------------------------------------------------------------------------------
        public CExcel(string filename)
        {
            this.filename = filename;


            

            
        }

        //--------------------------------------------------------------------------------------
        public void Open()
        {
            ex.Visible = true;
            ex.DisplayAlerts = false;

            workbook = ex.Workbooks.Open(filename);
            sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(1);
            sheet2 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(2);
            sheet3 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(3);
            sheet4 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(4);

            sheet1.Name = "Отчет";
            sheet2.Name = "Соответствия Аквамир";
            sheet3.Name = "Обработать Аквамир";
            sheet4.Name = "Исключения Аквамир";            
        }

        //--------------------------------------------------------------------------------------
        public void LoadBildPrices(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            bild = new CCsv(fileInfo.FullName, ';', Encoding.GetEncoding(1251));
            bild.ReadDataToCSV();
        }

        //--------------------------------------------------------------------------------------
        public void LoadAkvamir(string filename)
        {
            akvamir = CParcer_akvamir.LoadXML(filename, null);
        }

        //--------------------------------------------------------------------------------------
        public void SaveAndQuit()
        {
            workbook.Save();
            workbook.Close();
            ex.Quit();
        }

        //--------------------------------------------------------------------------------------
        public void Calculate()
        {            
            this.ClearSheet(sheet1);
            this.LoadMatches(ref matches_akvair, sheet2);

            int k = 7;
            sheet1.Cells[k, 1].Value = "Код товара";
            sheet1.Cells[k, 2].Value = "Артикул";
            sheet1.Cells[k, 3].Value = "ЦенаР";
            sheet1.Cells[k, 4].Value = "%";
            sheet1.Cells[k, 5].Value = "ЦенаА";
            sheet1.Cells[k, 6].Value = "%";
            sheet1.Cells[k, 7].Value = "ЦенаБ";
            sheet1.Cells[k, 8].Value = "%";
            sheet1.Cells[k, 9].Value = "ЦенаВ";
            sheet1.Cells[k, 10].Value = "%";
            sheet1.Cells[k, 11].Value = "ЦенаАкц";
            sheet1.Cells[k, 12].Value = "%";
            sheet1.Cells[k, 13].Value = "Источник";
            sheet1.Cells[k, 14].Value = "Бренд источника";
            sheet1.Cells[k, 15].Value = "Цена товара";
            sheet1.Cells[k, 16].Value = "Цена продажи";
            sheet1.Cells[k, 17].Value = "Вид соответствия";
            sheet1.Cells[k, 18].Value = "Артикул";
            sheet1.Cells[k, 19].Value = "Наименование";
            sheet1.Cells[k, 20].Value = "URL";
            //для всех позиций билда
            foreach (Dictionary <int, string> b in bild.data)
            {
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

                //ищем позицию в соответствиях
                CMatch match = matches_akvair.Find(m => b[0] == m.code);
                if (match != null)
                {
                    all_count++;

                    if (b[8] == "я")
                    {
                        this.WriteRow(b, null, "", "Позиция на Я");
                        continue;
                    }                    
                    //если нашли, то ищем позицию Аквамира по url из соответствия
                    CGood good = akvamir.catalog.Goods().Find(g => g.url.Replace("product", "catalog") == match.url.Replace("product","catalog"));
                    //если нашли, то записываем строку
                    if (good != null)
                    {
                        if (good.price1 == "" && good.price2 == "")
                        {
                            this.WriteRow(b, good, "", "У источника нет цен");
                        }
                        else
                        {
                            this.WriteRow(b, good, "По соответствию", null);
                        }
                    }
                    else
                    {
                        this.WriteRow(b, null, "", "Позиция не найдена в выгрузке источника");
                    }
                }
                else
                //если не нашли позицию по соответствию
                {
                    if (b[9] != "")
                    {
                        //то ищем по совпадению артикулов
                        CGood good = akvamir.catalog.Goods().Find(g => g.article.ToUpper().Replace(" ", "") == b[9].ToUpper().Replace(" ", ""));
                        if (good != null)
                        {
                            all_count++;
                            if (good.price1 == "" && good.price2 == "")
                            {
                                this.WriteRow(b, good, "", "У источника нет цен");
                            }
                            else
                            {
                                this.WriteRow(b, good, "По артикулу", null);
                            }
                        }
                    }
                }
                
                //CGood good = akvamir.catalog.Goods().Find(g => g.url == );
                //if (good != null)
                //    
            }

            sheet1.Cells[1, 1].Value = "Кол-во товаров Аквамир:";
            sheet1.Cells[1, 2].Value = akvamir.catalog.Goods().Count;
            sheet1.Cells[2, 1].Value = "Кол-во отработанных товаров:";
            sheet1.Cells[2, 2].Value = all_count;
            sheet1.Cells[3, 1].Value = "У нас цена лучше или равна:";
            sheet1.Cells[3, 2].Value = great_price_count;
            sheet1.Cells[4, 1].Value = "У нас цена хуже:";
            sheet1.Cells[4, 2].Value = bad_price_count;
        }

        //--------------------------------------------------------------------------------------
        private void ClearSheet(Microsoft.Office.Interop.Excel.Worksheet sheet)
        {
            sheet.get_Range("A1", "Z999999").Clear();
        }

        //--------------------------------------------------------------------------------------
        private void LoadMatches(ref List<CMatch> matches, Microsoft.Office.Interop.Excel.Worksheet sheet)
        {
            matches = new List<CMatch>();
            int i = 2;
            int j = 0;
            while (true)
            {
                string code = Convert.ToString(sheet.Cells[i, 1].Value);
                string url = sheet.Cells[i, 2].Value;
                
                if (code == "" || code == null) j++;
                else
                {
                    j = 0;
                    matches.Add(new CMatch(code, url));
                }
                if (j > 3) break;                
                i++;
            }
        }

        //--------------------------------------------------------------------------------------
        private void ToBorder(Microsoft.Office.Interop.Excel.Range range)
        {
            range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
            range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
            range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
            range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
            range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
            range.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
        }

        //--------------------------------------------------------------------------------------
        private void ToGreen(Microsoft.Office.Interop.Excel.Range range)
        {
            range.Interior.Color = ColorTranslator.ToOle(Color.YellowGreen);
        }

        //--------------------------------------------------------------------------------------
        private void ToRed(Microsoft.Office.Interop.Excel.Range range)
        {
            range.Interior.Color = ColorTranslator.ToOle(Color.Tomato);
        }

        //--------------------------------------------------------------------------------------
        private void AddGrayLine(Microsoft.Office.Interop.Excel.Worksheet sheet)
        {
            sheet.Range[sheet1.Cells[row_index, 1], sheet1.Cells[row_index, 18]].Interior.Color = ColorTranslator.ToOle(Color.LightGray);
            this.row_index++;
        }
        private void GrayLine(Microsoft.Office.Interop.Excel.Worksheet sheet, int row)
        {
            sheet.Range[sheet1.Cells[row, 1], sheet1.Cells[row, 18]].Interior.Color = ColorTranslator.ToOle(Color.LightGray);
        }

        //--------------------------------------------------------------------------------------
        private int CalculatePercent(double value1, double value2)
        {
            if (value1 == 0 || value2 == 0) return 0;
            double percent = 100.0 * (value1 - value2) / value1;
            return Convert.ToInt32(Math.Round(percent));
        }

        //--------------------------------------------------------------------------------------
        private void WriteRow(Dictionary<int, string> bild, CGood good, string type_math, string message)        
        {
            int i = row_index;

            sheet1.Cells[i, 1].Value = bild[0];
            if (message != null)
            {
                GrayLine(sheet1, i);
                sheet1.Cells[i, 2].Value = message;
                if (good != null)
                    sheet1.Cells[i, 19].Value = good.url;
            }
            else
            {
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

                double priceR = Convert.ToDouble(bild[5].Replace(".",","));
                double priceA = Convert.ToDouble(bild[1].Replace(".", ","));
                double priceB = Convert.ToDouble(bild[2].Replace(".", ","));
                double priceC = Convert.ToDouble(bild[3].Replace(".", ","));
                double priceAkc = 0;
                if (bild[6] != "")
                    priceAkc = Convert.ToDouble(bild[6].Replace(".", ","));
                double price1;
                if (good.price1 == "")
                    price1 = 0;
                else
                    price1 = Convert.ToDouble(good.price1.Replace(".", ","));
                double price2;
                if (good.price2 == "")
                {
                    price2 = 0;
                }
                else
                {
                    price2 = Convert.ToDouble(good.price2.Replace(".", ","));
                }
                string code = bild[0];
                string article = bild[9];
                //-----------------------------
                sheet1.Cells[i, 2].Value = bild[9];
                //-----------------------------
                sheet1.Cells[i, 3].Value = priceR;
                if (priceR > price2) this.ToRed(sheet1.Cells[i, 3]);
                if (priceR < price2) this.ToGreen(sheet1.Cells[i, 3]);

                sheet1.Cells[i, 4].Value = this.CalculatePercent(priceR, price2) + " %";
                //-----------------------------
                sheet1.Cells[i, 5].Value = priceA;
                if (priceA > price2) this.ToRed(sheet1.Cells[i, 5]);
                if (priceA < price2) this.ToGreen(sheet1.Cells[i, 5]);

                sheet1.Cells[i, 6].Value = this.CalculatePercent(priceA, price2) + " %";
                //-----------------------------
                sheet1.Cells[i, 7].Value = priceB;
                if (priceB > price2) this.ToRed(sheet1.Cells[i, 7]);
                if (priceB < price2) this.ToGreen(sheet1.Cells[i, 7]);

                sheet1.Cells[i, 8].Value = this.CalculatePercent(priceB, price2) + " %";
                //-----------------------------
                sheet1.Cells[i, 9].Value = priceC;
                if (priceC > price2) this.ToRed(sheet1.Cells[i, 9]);
                if (priceC < price2) this.ToGreen(sheet1.Cells[i, 9]);

                sheet1.Cells[i, 10].Value = this.CalculatePercent(priceC, price2) + " %";
                //-----------------------------
                sheet1.Cells[i, 11].Value = priceAkc;
                if (priceAkc != 0)
                {
                    if (priceAkc > price2) this.ToRed(sheet1.Cells[i, 11]);
                    if (priceAkc < price2) this.ToGreen(sheet1.Cells[i, 11]);

                    sheet1.Cells[i, 12].Value = this.CalculatePercent(priceAkc, price2) + " %";
                }
                //-----------------------------
                sheet1.Cells[i, 13].Value = "Аквамир";
                sheet1.Cells[i, 14].Value = good.brand;
                sheet1.Cells[i, 15].Value = price1;
                sheet1.Cells[i, 16].Value = price2;
                sheet1.Cells[i, 17].Value = type_math;
                sheet1.Cells[i, 18].Value = good.article;
                sheet1.Cells[i, 19].Value = good.name;
                sheet1.Cells[i, 20].Value = good.url;

                //-----------------------------
                if (priceAkc > 0)
                {
                    ToBorder(sheet1.Cells[i, 11]);
                    if (priceAkc <= price2)
                        great_price_count++;
                    else
                        bad_price_count++;
                }
                else
                {
                    if (priceR < 1500)
                    {
                        ToBorder(sheet1.Cells[i, 3]);
                        if (priceR <= price2)
                            great_price_count++;
                        else
                            bad_price_count++;
                    }
                    if (1500 <= priceR && priceR < 15000)
                    {
                        ToBorder(sheet1.Cells[i, 5]);
                        if (priceA <= price2)
                            great_price_count++;
                        else
                            bad_price_count++;
                    }
                    if (15000 <= priceR && priceR < 50000)
                    {
                        ToBorder(sheet1.Cells[i, 7]);
                        if (priceB <= price2)
                            great_price_count++;
                        else
                            bad_price_count++;
                    }
                    if (50000 <= priceR)
                    {
                        ToBorder(sheet1.Cells[i, 9]);
                        if (priceC <= price2)
                            great_price_count++;
                        else
                            bad_price_count++;
                    }
                }

                /*sheet1.Cells[i, 2].Value = good.price1;
                this.ToBorder(sheet1.Cells[i, 2]);
                this.ToGreen(sheet1.Cells[i, 2]);
                sheet1.Cells[i, 3].Value = good.price2;
                this.ToRed(sheet1.Cells[i, 3]);
                sheet1.Cells[i, 13].Value = good.url;*/

            }


            this.row_index++;                        
        }

        //--------------------------------------------------------------------------------------
    }
}
