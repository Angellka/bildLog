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
    public class CGood_akvamir : CGood
    {
        public CGood_akvamir() { }
        public CGood_akvamir(string url) : base(url)
        {

        }

        public override string Parce(WebDriver driver)
        {            
            try
            {
                driver.Navigate().GoToUrl(this.url);
                //datetime
                this.datetime = DateTime.Now;
                //name
                IWebElement div_name = driver.FindElement(By.Id("pagetitle"));
                this.name = div_name.Text;

                //price1 price2
                try
                {
                    IReadOnlyCollection<IWebElement> list = driver.FindElements(By.ClassName("price_currency"));
                    this.price1 = list.ElementAt(1).Text;
                    this.price2 = list.ElementAt(0).Text;
                }
                catch { }

                //article
                try
                {                    
                    IWebElement div_table = driver.FindElement(By.ClassName("props_list"));
                    IReadOnlyCollection<IWebElement> trs = div_table.FindElements(By.TagName("tr"));
                    foreach (IWebElement tr in trs)
                    {
                        string char_name = tr.FindElement(By.ClassName("char_name")).FindElement(By.ClassName("props_item")).FindElement(By.TagName("span")).Text;
                        if (char_name == "Артикул")
                            this.article = tr.FindElement(By.ClassName("char_value")).FindElement(By.TagName("span")).Text;
                    }
                }
                catch { }


                //наличие
                try
                {
                    IWebElement div_buy = driver.FindElement(By.ClassName("buy_block"));
                    string s = div_buy.Text;
                    if (s.ToUpper().Contains("ПОД ЗАКАЗ"))
                        this.availability = "Под заказ";
                    if (s.ToUpper().Contains("В КОРЗИНУ"))
                        this.availability = "В наличии";
                }
                catch { }

                //бренд
                try
                {
                    IWebElement div_brand = driver.FindElement(By.ClassName("brand__picture"));
                    IWebElement img_brand = div_brand.FindElement(By.TagName("img"));
                    this.brand = img_brand.GetAttribute("title");
                }
                catch { }

                return null;
            }
            catch (Exception exc)
            {
                this.parce_error = exc.Message;
                return exc.Message;
            }
            
            return null;
        }
    }
}
