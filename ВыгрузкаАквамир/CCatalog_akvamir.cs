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
    public class CCatalog_akvamir : CCatalog
    {
        

        public CCatalog_akvamir() { }
        public CCatalog_akvamir(string url) : base(url)
        {
        }

        

        public override void Parce(WebDriver driver)
        {
            try
            {
                driver.Navigate().GoToUrl(url);
                IWebElement catalog = driver.FindElement(By.ClassName("catalog_section_list"));
                IReadOnlyCollection<IWebElement> list = catalog.FindElements(By.ClassName("item_block"));                
                foreach (IWebElement l in list)
                {   
                    //url                                     
                    IWebElement div_a = l.FindElement(By.ClassName("section_info")).FindElement(By.ClassName("name")).FindElement(By.TagName("a"));
                    string url = div_a.GetAttribute("href");
                    //name
                    IWebElement div_name = div_a.FindElement(By.TagName("span"));
                    string name = div_name.Text;
                    //label_count
                    IWebElement div_count = l.FindElement(By.ClassName("element-count2"));
                    string label_count = div_count.Text;

                    if (chapters.FirstOrDefault(e => e.url == url) == null)
                        chapters.Add(new CChapter_akvamir(name, url, label_count));
                }

                foreach(CChapter_akvamir chapter in chapters)
                {
                    chapter.Parce(driver);
                }

            }
            catch (Exception exc)
            {
                this.parce_error = exc.Message;                
            }
        }
    }
}
