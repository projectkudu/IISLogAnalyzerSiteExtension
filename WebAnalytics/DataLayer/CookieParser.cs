using System;
using System.Collections.Generic;

namespace WebAnalytics.DataLayer
{
    class CookieParser
    {
        public const string ARR_AFFINITY = "ARRAffinity";
        public const string WA_WEBSITE_SID = "WAWebSiteSID";
        public const string D4DAD = "d4dad6935f632ac35975e3001dc7bbe8";

        private static Char[] semiColon = new Char[] { ';' };
        public struct Cookie
        {
            private string key, value;
            public Cookie(string key, string value)
            {
                this.key = key;
                this.value = value;
            }

            public string Key { get { return key; } }
            public string Value { get { return value; } }
        }
        /**
         * <summary>
         *  use this method to extract a string representation of cookies
         *  and return a collection of cookies that came with the response
         * </summary>
         * <param name="stringCookieRepresentation"/>
         * <returns>CookieCollection given the string representation of cookies</returns>
         * */
        public Dictionary<string,string> ExtractServerHeaderResponseCookies(String stringCookieRepresentation)
        {
            if (stringCookieRepresentation.Equals("-"))
            {
                return null;
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string,string>();

            //make code robust to whitespace by removing the + and split the cookies since they are delimted by semi colons
            string[] cookies = stringCookieRepresentation.Replace("+","").Split(new Char[] { ';' });
            foreach (string stringCookie in cookies)
            {
                Cookie tempCookie = CreateCookie(stringCookie);
                if (tempCookie.Key != null && tempCookie.Value != null)
                {
                    //Trace.WriteLine(tempCookie.Key);
                    cookieCollection.Add(tempCookie.Key, tempCookie.Value);
                }
            }
            return cookieCollection;
        }

        private Cookie CreateCookie(String stringCookie)
        {
            //the components in cookies are delimited by '=' to denote name and value pairs
            String[] cookieComponents = stringCookie.Split('=');
            int equalsIndex = stringCookie.IndexOf("=");
            if (equalsIndex > 0)
            {
                string name = stringCookie.Substring(0, equalsIndex);
                string value = stringCookie.Substring(equalsIndex + 1);
                return new Cookie(name, value);
            }
            return new Cookie(null, null);
        }
    }
}
