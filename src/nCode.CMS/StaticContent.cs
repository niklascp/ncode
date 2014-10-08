using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using nCode.CMS.UI;

namespace nCode.CMS
{
    /// <summary>
    /// Content Type for Static Content.
    /// </summary>
    public class StaticContent : ContentType
    {
        private static readonly Guid id = new Guid("36a74d12-1415-4219-9174-b971f3c64f4b");
        private static readonly string name = "StaticContent";

        /// <summary>
        /// Gets a unique ID that identifies the Static Content Type.
        /// </summary>
        public override Guid ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets a localized title for the Static Content Type.
        /// </summary>
        public override string Title
        {
            get { return "Tekst og billeder"; }
        }

        /// <summary>
        /// Gets the non-localized system name for this Content Type.
        /// </summary>
        public override string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets a localized description for the Static Content Type.
        /// </summary>
        public override string Description
        {
            get { return "Benyt denne indholdstype til tekst, billeder, links m.m."; }
        }

        /// <summary>
        /// True if this Content Type supports Content Part Mode, otherwise false.
        /// </summary>
        public override bool SupportContentPartMode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a ContentControl that can be used to view the Static Content Type.
        /// </summary>
        public override ContentControl GetControl(Page page)
        {
            return (ContentControl)page.LoadControl("~/Admin/CMS/ContentControls/StaticContent/View.ascx");
        }

        /// <summary>
        /// Gets a ContentEditControl that can be used to edit the Static Content Type.
        /// </summary>
        public override ContentEditControl GetEditControl(Page page)
        {
            return (ContentEditControl)page.LoadControl("~/Admin/CMS/ContentControls/StaticContent/Edit.ascx");
        }
    }
}
