using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NParser;
using OpenQA.Selenium;

namespace ВыгрузкаВодопарад
{
    [Serializable]
    public class CPage_vodoparad : CPage
    {
        public CPage_vodoparad() { }
        public CPage_vodoparad(string url) : base(url) { }

        public override void Parce(WebDriver driver)
        {
            this.goods = new List<CGood>();
            driver.Navigate().GoToUrl(url);
            IReadOnlyCollection<IWebElement> list = driver.FindElements(By.ClassName("product-item"));
            foreach (IWebElement l in list)
            {
                //url
                IWebElement a = l.FindElement(By.TagName("a"));
                string url = a.GetAttribute("href");
                CGood_vodoparad good = new CGood_vodoparad(url);

                //name
                IWebElement good_name = l.FindElement(By.ClassName("product-item__name"));
                good.name = good_name.Text;

                //price1
                //price2
                IWebElement div_price = l.FindElement(By.ClassName("product-item__price"));
                try
                {
                    //есть скидочная цена                    
                    IWebElement div_p1 = div_price.FindElement(By.ClassName("product-item__price-old"));
                    good.price1 = div_p1.Text;
                    IWebElement div_p2 = div_price.FindElement(By.ClassName("product-item__price-new"));
                    good.price2 = div_p2.Text;                    
                }
                catch (Exception e)
                {
                    //нет скидочной цены
                    IWebElement div_p = div_price.FindElement(By.ClassName("product-item__price-new"));
                    good.price1 = div_p.Text;
                    good.price2 = div_p.Text;
                }

                //article
                IWebElement content = l.FindElement(By.ClassName("product-item__content"));
                IReadOnlyCollection<IWebElement> rows = content.FindElements(By.ClassName("product-card__character--row"));
                foreach(IWebElement row in rows)
                {
                    string row_name = row.FindElement(By.ClassName("product-card__character--name")).Text;
                    if (row_name.ToUpper() == "АРТИКУЛ")
                    {
                        IWebElement row_value = row.FindElement(By.ClassName("product-card__character--value"));
                        good.article = row_value.Text;
                        break;
                    }
                }


                this.goods.Add(good);
            }
            //throw new NotImplementedException();
        }
    }
}
