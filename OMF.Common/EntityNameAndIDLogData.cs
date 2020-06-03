using System;

namespace OMF.Common
{
    public class EntityNameAndIDLogData
    {
        public DateTime InsertDateTime { get; set; }

        public string InsertUserName { get; set; }

        public int InsertUserId { get; set; }

        public DateTime? UpdateDateTime { get; set; }

        public string UpdateUserName { get; set; }

        public int? UpdateUserId { get; set; }
    }
}
