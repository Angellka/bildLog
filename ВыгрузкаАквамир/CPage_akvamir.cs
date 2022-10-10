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
    public class CPage_akvamir : CPage
    {
        public CPage_akvamir() { }
        public CPage_akvamir(string url) : base(url)
        {

        }

        public override List<CGood> Parce(WebDriver driver)
        {
            List<CGood> goods = new List<CGood>();
            driver.Navigate().GoToUrl(url);
            IReadOnlyCollection<IWebElement> list = driver.FindElements(By.ClassName("catalog_item"));
            foreach (IWebElement l in list)
            {
                try
                {
                    
                    IWebElement item_info = l.FindElement(By.ClassName("item_info"));
                    string url = item_info.FindElement(By.ClassName("item-title")).FindElement(By.TagName("a")).GetAttribute("href");
                    CGood_akvamir good = new CGood_akvamir(url);
                    good.name = item_info.FindElement(By.ClassName("item-title")).FindElement(By.TagName("a")).FindElement(By.TagName("span")).Text;
                    good.article = item_info.FindElement(By.ClassName("article_block")).GetAttribute("data-value");
                    try
                    {
                        //IReadOnlyCollection<IWebElement> ll = l.FindElements(By.ClassName("price_currency"));
                        //good.price1 = ll.ElementAt(1).Text;
                        //good.price2 = ll.ElementAt(0).Text;
                    }
                    catch { }
                    goods.Add(good);
                }
                catch (Exception exc)
                {
                    parce_error = exc.Message;
                }
            }

            return goods;
        }
    }
}
