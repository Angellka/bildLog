using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NParser
{
    [Serializable]
    public abstract class CPage
    {
        public string url;        
        public string parce_error = "";

        public CPage() { }

        public CPage(string url)
        {
            this.url = url;
        }

        public abstract List<CGood> Parce(WebDriver driver);
    }
}
