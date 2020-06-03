using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMF.Common
{
    public class EntityIDLogData
    {
        public DateTime? InsertDateTime { get; set; }

        public int? InsertUserID { get; set; }

        public DateTime? UpdateDateTime { get; set; }

        public int? UpdateUserID { get; set; }
    }
}