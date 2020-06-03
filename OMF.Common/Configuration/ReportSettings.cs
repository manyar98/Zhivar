using System;
using System.Configuration;

namespace OMF.Common.Configuration
{
    internal class ReportSettings : ConfigurationElement
    {
        [ConfigurationProperty("useGridViewDataForExport")]
        public bool UseGridViewDataForExport
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["useGridViewDataForExport"]);
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
