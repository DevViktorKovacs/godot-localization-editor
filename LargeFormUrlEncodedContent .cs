using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace godotlocalizationeditor
{
    public class LargeFormUrlEncodedContent: ByteArrayContent
    {
        private static readonly Encoding Utf8Encoding = Encoding.UTF8;

        public LargeFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
              : base(GetContentByteArray(nameValueCollection))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        public static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException(nameof(nameValueCollection));
            }

            var str = string.Join(
                  "&",
                  nameValueCollection.Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}"));
            return Utf8Encoding.GetBytes(str);
        }

        public static string GetContentAsString(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        { 
            return string.Join(
                  "&",
                  nameValueCollection.Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}"));
        }
    }
}
