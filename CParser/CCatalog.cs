using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NParser
{
    [Serializable]
    public abstract class CCatalog
    {
        public string url;
        public List<CChapter> chapters;
        public string parce_error = "";

        public CCatalog() { }
        public CCatalog(string url)
        {
            this.url = url;
            chapters = new List<CChapter>();
        }

        public List<CGood> Goods()
        {
            List<CGood> goods = new List<CGood>();
            if (chapters != null)
                foreach (CChapter chapter in chapters)
                {
                    goods.AddRange(chapter.goods);
                }
            return goods;
        }

        public abstract void Parce(WebDriver driver);
    }
}
