using OMF.Common.Security;
using System.Collections.Generic;

namespace OMF.EntityFramework.Query
{
    public class QueryInfo
    {
        public int Take { get; set; }

        public int Skip { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public OperationAccess OperationAccess { get; set; }

        public List<SortInfo> Sort { get; set; }

        public FilterInfo Filter { get; set; }
    }
}
