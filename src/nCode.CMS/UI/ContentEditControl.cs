/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Web;
using System.Web.UI;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.IO;

namespace nCode.CMS.UI
{
    /// <summary>
    /// Represents an abstract class for Content Types Editing Controls.
    /// </summary>
    [Obsolete("Please use the new Content Part Framework.")]
    public abstract class ContentEditControl : UserControl
    {
        private ContentPage contentPage;

        /// <summary>
        /// Gets the Content that are currently being edited.
        /// </summary>
        public virtual ContentPage ContentPage
        {
            get
            {
                return contentPage;
            }
            set
            {
                contentPage = value;
            }
        }

        /// <summary>
        /// Loads the Content from the Database.
        /// </summary>
        public virtual void LoadContent(bool isCopy) { }

        /// <summary>
        /// Saves the Content to the Database.
        /// </summary>
        public virtual void SaveContent(bool isNew) { }
    }
}
