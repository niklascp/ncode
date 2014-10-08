using nCode.CMS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace nCode.CMS
{
    public class ContentPartContentType : ContentType
    {
        private static readonly Guid id;
        private static readonly string name;

        static ContentPartContentType()
        {
            id = new Guid("F535464B-576E-447E-BB2F-813F16386C97");
            name = "ContentPart";
        }

        /// <summary>
        /// A unique ID identifies this Content Type.
        /// </summary>
        public override Guid ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the non-localized system name for this Content Type.
        /// </summary>
        public override string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets a localized title for this Content Type.
        /// </summary>
        public override string Title
        {
            get { return "Sammensat"; }
        }

        /// <summary>
        /// Gets a localized description for this Content Type.
        /// </summary>
        public override string Description
        {
            get { return "Opretter en side hvor flere indholdstyper bagefter kan tilføjes til"; }
        }

        public override bool SupportContentPartMode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a ContentControl that can be used to view this Content Type.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public override ContentControl GetControl(Page page)
        {
            return (ContentControl)page.LoadControl("~/Admin/CMS/ContentControls/ContentPart/View.ascx");
        }

        /// <summary>
        /// Gets a ContentEditControl that can be used to edit this Content Type.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public override ContentEditControl GetEditControl(Page page)
        {
            return null;
            //return (ContentEditControl)page.LoadControl("~/Admin/CMS/ContentControls/ContentPart/Edit.ascx");
        }
    }
}
