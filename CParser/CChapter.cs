using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NParser
{
    [Serializable]
    public abstract class CChapter
    {
        public string name;
        public string url;

        public string label_count = "";
        public int count = 0;

        public List<CPage> pages = new List<CPage>();
        public List<CGood> goods = new List<CGood>();
        public string parce_error = "";

        public CChapter() { }

        public CChapter(string name, string url, string label_count)
        {
            this.name = name;
            this.url = url;
            this.label_count = label_count;
        }               

        public abstract void Parce(WebDriver driver);
    }
}
