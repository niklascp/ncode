using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Newtonsoft.Json;

namespace nCode.UI
{
    public static class StylesHelper
    {
        /* Default style sheet search order. */
        private static readonly string[] styleSheets = { "~/layout/editor.css", "~/cms/editor.css", "~/layout/stylesheet.css" };

        /* Default style set search order. */
        private static readonly string[] styleSets = { "~/layout/editorstyles.js", "~/cms/editorstyles.js" };

        private static string styleSheet = null;
        private static string styleSet = null;

        public static string DefaultEditorStylesheet
        {
            get
            {
                /* Set style sheet. */
                if (styleSheet == null)
                {
                    foreach (string s in styleSheets)
                    {
                        if (File.Exists(HttpContext.Current.Server.MapPath(s)))
                        {
                            styleSheet = VirtualPathUtility.ToAbsolute(s);
                            break;
                        }
                    }
                }

                return styleSheet;
            }
        }

        public static string DefaultEditorStyleset
        {
            get
            {
                /* Set style sheet. */
                if (styleSet == null)
                {
                    foreach (string s in styleSets)
                    {
                        if (File.Exists(HttpContext.Current.Server.MapPath(s)))
                        {
                            //JsonConvert.DeserializeObject<dynamic>(
                            styleSet = "default:" + VirtualPathUtility.ToAbsolute(s);
                            break;
                        }
                    }
                }

                return styleSet;
            }
        }

    }
}
