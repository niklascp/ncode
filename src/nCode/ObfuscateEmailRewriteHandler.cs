using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace nCode
{
    public class ObfuscateEmailRewriteHandler : ContentRewriteHandler
    {
        // The length of the string the email address is split into.
        const int splitLength = 7;

        private readonly Regex regex;

        public ObfuscateEmailRewriteHandler()
        {
            regex = new Regex("<a[^>]*href\\s*=\\s*([\"'])mailto:[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}([\"'])[^>]*>(.*?)</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public override Regex Regex
        {
            get
            {
                if (Settings.ObfuscateEmail)
                    return regex;
                return null;
            }
        }

        public override string RewritePreRenderMatch(Match match)
        {
            var email = match.Value;

            // Escape the original email address for javascript chars.
            email = email.Replace("'", "\\'");

            // The javascript string that will contain the final script.
            string javaEmail = "document.write('";

            // Split and build an string
            int i = 0;
            while (i < email.Length)
            {
                if (i % splitLength == 0)
                    javaEmail += "'+'";

                javaEmail += email[i];

                i++;
            }

            javaEmail += "');";

            //Encode the script
            return "<script type=\"text/javascript\">eval(unescape('" + nCode.Utilities.UrlEncodeString(javaEmail) + "'));</script>";
        }
    }
}
