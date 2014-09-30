using System;
using System.Data;

namespace nCode.UI
{
    /// <summary>
    /// Summary description for LocalizationEventArgs
    /// </summary>
    public class LocalizationEventArgs : EventArgs
    {
        private DataRow localizationRow;

        public LocalizationEventArgs(DataRow localizationRow)
        {
            this.localizationRow = localizationRow;
        }

        public DataRow LocalizationRow
        {
            get { return localizationRow; }
        }
    }
}
