using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NParser;
using OpenQA.Selenium;
using System.Threading;

namespace ВыгрузкаВодопарад
{
    [Serializable]
    public class CCatalog_vodoparad : CCatalog
    {
        public CCatalog_vodoparad() { }
        public CCatalog_vodoparad(string url) : base(url) { }
        public override void Parce(WebDriver driver)
        {
            this.chapters = new List<CChapter>();
            driver.Navigate().GoToUrl(url);

            this.ClickCityRostov(driver);

            IWebElement catalog = driver.FindElement(By.ClassName("sidebar-filter__elem"));

            IReadOnlyCollection<IWebElement> list = catalog.FindElements(By.ClassName("elem-item--lvl-2"));
            int i = 0;
            foreach (IWebElement l in list)
            {
                IWebElement a = l.FindElement(By.TagName("a"));
                //url
                string url = a.GetAttribute("href");
                //name
                string name = a.Text;
                //label_count
                string label_count = l.FindElement(By.ClassName("add-sidebar-filter__element-count")).Text;

                chapters.Add(new CChapter_vodoparad(name, url, label_count));
            }

            foreach (CChapter_vodoparad chapter in chapters)
            {
                chapter.Parce(driver);
            }
            //chapters[6].Parce(driver);

            foreach(CGood_vodoparad good in this.Goods())
            {
                good.Parce(driver);
            }

        }
        
        private void ClickCityRostov(WebDriver driver)
        {
            try
            {
                Thread.Sleep(5000);
                IWebElement div_city = driver.FindElement(By.Id("select-city-second-modal"));
                IWebElement button = div_city.FindElement(By.ClassName("btn--light-blue"));
                button.Click();                
                Thread.Sleep(5000);

            }
            catch (Exception e)
            {
                
            }
        }
    }
}
