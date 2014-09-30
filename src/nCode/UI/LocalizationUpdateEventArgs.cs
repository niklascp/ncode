using System;
using System.Data;
using System.Web.UI;

namespace nCode.UI
{
    /// <summary>
    /// Summary description for UpdateLoxalizationEventArgs
    /// </summary>
    public class LocalizationUpdateEventArgs : LocalizationEventArgs
    {
        private Control container;

        public LocalizationUpdateEventArgs(DataRow row, Control container)
            : base(row)
	    {
            this.container = container;
	    }

        public Control Container
        {
            get { return container; }
        }
    }
}