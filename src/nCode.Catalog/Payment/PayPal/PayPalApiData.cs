#pragma warning disable 1591

using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace nCode.Catalog.Payment.PayPal
{
    public sealed class PayPalApiData : NameValueCollection
    {
        private const string AMPERSAND = "&";
        private const string EQUALS = "=";
        private static readonly char[] AMPERSAND_CHAR_ARRAY = AMPERSAND.ToCharArray();
        private static readonly char[] EQUALS_CHAR_ARRAY = EQUALS.ToCharArray();

        /// <summary>
        /// Returns the built NVP string of all name/value pairs in the Hashtable
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            StringBuilder sb = new StringBuilder();
            bool firstPair = true;
            foreach (string kv in AllKeys)
            {
                string name = UrlEncode(kv);
                string value = UrlEncode(this[kv]);
                if (!firstPair)
                {
                    sb.Append(AMPERSAND);
                }
                sb.Append(name).Append(EQUALS).Append(value);
                firstPair = false;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Decoding the string
        /// </summary>
        /// <param name="nvpstring"></param>
        public void Decode(string nvpstring)
        {
            Clear();
            foreach (string nvp in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
            {
                string[] tokens = nvp.Split(EQUALS_CHAR_ARRAY);
                if (tokens.Length >= 2)
                {
                    string name = UrlDecode(tokens[0]);
                    string value = UrlDecode(tokens[1]);
                    Add(name, value);
                }
            }
        }

        private static string UrlDecode(string s) { return HttpUtility.UrlDecode(s); }
        private static string UrlEncode(string s) { return HttpUtility.UrlEncode(s); }

        public void Add(string name, string value, int index)
        {
            this.Add(GetArrayName(index, name), value);
        }

        public void Remove(string arrayName, int index)
        {
            this.Remove(GetArrayName(index, arrayName));
        }

        public string this[string name, int index]
        {
            get
            {
                return this[GetArrayName(index, name)];
            }
            set
            {
                this[GetArrayName(index, name)] = value;
            }
        }

        private static string GetArrayName(int index, string name)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index can not be negative : " + index);
            }
            return name + index;
        }
    }
}
