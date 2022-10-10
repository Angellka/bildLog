using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NParser;
using OpenQA.Selenium;

namespace ВыгрузкаАквамир
{
    [Serializable]
    public class CChapter_akvamir : CChapter
    {
        public CChapter_akvamir() { }
        public CChapter_akvamir(string name, string url, string label_count) : base(name, url, label_count)
        {
        }

        public override void Parce(WebDriver driver)
        {
            try
            {
                pages = new List<CPage>();
                driver.Navigate().GoToUrl(url);
                string s = "";            
                try
                {
                    s = driver.FindElement(By.ClassName("module-pagination")).Text;
                }
                catch
                {
                    //если одна страница в разделе
                    pages.Add(new CPage_akvamir(this.url));
                    return;
                }
                
                string[] ss = s.Split(' ');
                int ii = Convert.ToInt32(ss[ss.Count() - 1]);
                
                for (int i = 1; i <= ii; i++)
                {
                    string page_url = this.url + "?PAGEN_1=" + i;
                    pages.Add(new CPage_akvamir(page_url));
                }


                foreach(CPage_akvamir page in this.pages)
                {
                    List<CGood> gg = page.Parce(driver);
                    foreach (CGood g in gg)
                        if (goods.FirstOrDefault(e => g.url == e.url) == null)
                            goods.Add(g);
                }
                this.count = goods.Count();

            }
            catch (Exception exc)
            {
                this.parce_error = exc.Message;
            }

        }

    }
}
