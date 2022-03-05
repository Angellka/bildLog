using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScanner
{
    //-----------------------------------------------------------------------
    public class CGood
    {

    }
    

    //-----------------------------------------------------------------------
    public abstract class CChapter
    {
        public string name;
        public Uri uri;
        public List<CChapter> level2 = new List<CChapter>();
        public abstract void GetChapters();
        
    }

    //-----------------------------------------------------------------------
    public class Catalog
    {
        public List<CChapter> chapters = new List<CChapter>();
        //public 
    }
    //-----------------------------------------------------------------------
    public class CScanner
    {

    }
    //-----------------------------------------------------------------------
    
    

    
}
