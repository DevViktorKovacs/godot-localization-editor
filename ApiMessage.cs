
using System.Collections.Generic;

namespace godotlocalizationeditor
{
    internal class ApiMessage
    {
        public string text { get; set; }

        public string target_lang { get; set; }

        public string source_lang { get; set; }

        public IEnumerable<KeyValuePair<string, string>> GetKeyValuePairs()
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("target_lang", target_lang),
                new KeyValuePair<string, string>("source_lang", source_lang)
            };        
        }
    }
}
