using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace godotlocalizationeditor
{
    public  class LocalizedTexts
    {
        public string Locale { get; set; }

        public Dictionary<string,string> Texts { get; set; }
    }
}
