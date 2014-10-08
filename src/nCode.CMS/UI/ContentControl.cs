/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web.UI;

using nCode;
using nCode.Security;

namespace nCode.CMS.UI
{
    /// <summary>
    /// Provides a abstract class for Content Type Controls.
    /// </summary>
    [Obsolete("Please use the new Content Part Framework.")]
    public abstract class ContentControl : UserControl
    {
        protected virtual Guid ContentTypeID {
            get 
            {
                throw new NotImplementedException("You need to overide ContentTypeID and provide a unique identifier");
            }
        }

        protected virtual string ContentTypeName
        {
            get
            {
                throw new NotImplementedException("You need to overide ContentTypeName and provide a name");
            }
        }

        /// <summary>
        /// Gets the Content that are currently being edited.
        /// </summary>
        public virtual ContentPage ContentPage { get; set; }
    }
}
