using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.UI
{
    /// <summary>
    /// Dommy method for navigation extension methods.
    /// </summary>
    public class Navigation
    {
        private const string currentNavigationItem = "nCode.UI.INavigationItem";
        private const string currentNavigationPath = "nCode.UI.INavigationPath";

        private static void UpdateCurrentPath()
        {
            var currentPath = new List<INavigationItem>();
            var contentPage = Current;

            while (contentPage != null)
            {
                currentPath.Add(contentPage);
                contentPage = contentPage.GetParent();
            }

            currentPath.Reverse();

            HttpContext.Current.Items[currentNavigationPath] = currentPath;
        }

        /// <summary>
        /// Gets the current ContentPage for this request, or null if this is not
        /// a content page request.
        /// </summary>
        public static INavigationItem Current
        {
            get
            {
                return HttpContext.Current.Items[currentNavigationItem] as INavigationItem;
            }
            set
            {
                HttpContext.Current.Items[currentNavigationItem] = value;
                UpdateCurrentPath();
            }
        }

        /// <summary>
        /// Gets the current Path of content pages for this requist, or an empty list if this os 
        /// not a content page request.
        /// </summary>
        public static IList<INavigationItem> CurrentPath
        {
            get
            {
                /* NCP 2014-07-17 - Always return a instance, defaulting to an empty list if the current path is unknown (e.g. null). */
                return HttpContext.Current.Items[currentNavigationPath] as IList<INavigationItem> ?? new List<INavigationItem>();
            }
        }
    }
}
