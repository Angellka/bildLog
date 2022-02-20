using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSettings;

namespace CParser
{
    public class CParser
    {
        string site_url;
        CCatalog catalog;
        CSettings settings;


        CParser(string site_url, CSettings settings)
        {
            this.site_url = site_url;
            this.settings = settings;
        }

        
    }
}
