using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NParser
{
    [Serializable]
    public abstract class CGood
    {
        private string _price1 = "";
        /// <summary>
        /// РРЦ
        /// </summary>
        public string price1
        {
            get { return this._price1; }
            set { this._price1 = this.ClearPrice(value); }
        }

        private string _price2 = "";
        /// <summary>
        /// Цена продажи
        /// </summary>
        public string price2
        {
            get { return this._price2; }
            set { this._price2 = this.ClearPrice(value); }
        }

        /// <summary>
        /// Последняя дата обновления
        /// </summary>
        public DateTime datetime;

        /// <summary>
        /// Название товара
        /// </summary>
        public string name = "";

        /// <summary>
        /// Артикул товара
        /// </summary>
        public string article = "";

        /// <summary>
        /// Картинки. Первая должна быть главной
        /// </summary>
        public List<string> pictures = new List<string>();

        /// <summary>
        /// Список свойств товара
        /// </summary>
        public List<CProperty> properties = new List<CProperty>();

        /// <summary>
        /// URL ссылка на товар
        /// </summary>
        public string url;

        /// <summary>
        /// Наличие
        /// </summary>
        public string availability = "";

        /// <summary>
        /// Производитель
        /// </summary>
        public string brand = "";

        /// <summary>
        /// Текст ошибки во время парсинга
        /// </summary>
        public string parce_error = "";

        public CGood() { }
        public CGood(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Очищает входящую строку, оставляя только цифры
        /// </summary>
        /// <returns></returns>
        private string ClearPrice(string s)
        {
            Regex regexObj = new Regex(@"[^\d]");
            return regexObj.Replace(s, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>Текст ошибки во время парсинга</returns>
        public abstract string Parce(WebDriver driver);

        public override string ToString()
        {
            return this.name;
        }
    }
}
