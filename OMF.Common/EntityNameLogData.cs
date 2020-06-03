using System;

namespace OMF.Common
{
    public class EntityNameLogData
    {
        public DateTime InsertDateTime { get; set; }

        public string InsertUserName { get; set; }

        public DateTime? UpdateDateTime { get; set; }

        public string UpdateUserName { get; set; }
    }
}
