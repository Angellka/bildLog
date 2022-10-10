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
    public class CGood_vodoparad : CGood
    {
        public CGood_vodoparad() { }
        public CGood_vodoparad(string url) : base(url) { }

        public override string Parce(WebDriver driver)
        {
            /*string error = "";
            try
            {
                driver.Navigate().GoToUrl(url);

                IWebElement card = driver.FindElement(By.ClassName("card-page"));
                //name
                IWebElement div_name = card.FindElement(By.ClassName("block-title"));
                this.name = div_name.Text;
                //article
                IWebElement table = card.FindElement(By.ClassName("product-card__character--table"));
                IReadOnlyCollection<IWebElement> list = table.FindElements(By.ClassName("product-card__character--row"));
                foreach (IWebElement l in list)
                {
                    IWebElement row_name = card.FindElement(By.ClassName("product-card__character--name"));
                    if (row_name.Text.ToUpper() == "АРТИКУЛ")
                    {
                        this.article = card.FindElement(By.ClassName("product-card__character--value")).Text;
                        break;
                    }
                }
                //price1

                //price2
                //product-item__price-new
            }
            catch (Exception exc)
            {

                this.parce_error = exc.Message;
                error = exc.Message;
            }

            return error;*/
            return "";
        }
    }
}
