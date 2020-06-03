using System.Collections.Generic;

namespace OMF.EntityFramework.Query
{
    public class FilterData
    {
        public FilterData()
        {
            this.Filters = new List<FilterData>();
        }

        public string Operator { get; set; }

        public string Field { get; set; }

        public string Value { get; set; }

        public string Logic { get; set; }

        public List<FilterData> Filters { get; set; }
    }
}
