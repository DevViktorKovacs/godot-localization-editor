using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace godotlocalizationeditor
{
    internal class ApiResponse
    {
        public Translation[] translations { get; set; }
    }

    public class Translation
    {
        public string detected_source_language { get; set; }
        public string text { get; set; }
    }
}
