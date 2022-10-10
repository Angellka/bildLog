using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NParser;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace ВыгрузкаВодопарад
{
    [Serializable]
    public class CChapter_vodoparad : CChapter
    {
        public CChapter_vodoparad() { }
        public CChapter_vodoparad(string name, string url, string label_count) : base(name, url, label_count) { }

        public override void Parce(WebDriver driver)
        {
            //throw new NotImplementedException();
            //
            //try
            {
                pages = new List<CPage>();
                driver.Navigate().GoToUrl(url);
                string s = "";
                try
                {
                    s = driver.FindElement(By.ClassName("pagination-layout__list")).Text;
                }
                catch
                {
                    //если одна страница в разделе
                    pages.Add(new CPage_vodoparad(this.url));
                    return;
                }

                string pattern = @"\d{1,}";
                Regex rgx = new Regex(pattern);
                MatchCollection matches = rgx.Matches(s);
                int ii = Convert.ToInt32(matches[matches.Count - 1].Value);
                for (int i = 1; i <= ii; i++)
                {
                    string page_url = this.url + "?PAGEN_1=2&SIZEN_1=24&PAGEN_3=" + i + "&SIZEN_3=24";
                    pages.Add(new CPage_vodoparad(page_url));

                }

                foreach (CPage_vodoparad page in this.pages)
                {
                    page.Parce(driver);
                    this.count += page.goods.Count();
                }
            }
            /*catch (Exception exc)
            {
                this.parce_error = exc.Message;
            }*/
        }
    }
}
