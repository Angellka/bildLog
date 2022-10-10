using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ОтчетПоЦенам
{
    public class CMatch
    {
        public string code;
        public string url;

        public CMatch(string code, string url)
        {
            this.code = code;
            this.url = url;
        }
    }
}
