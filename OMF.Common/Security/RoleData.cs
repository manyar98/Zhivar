using System.Runtime.Serialization;

namespace OMF.Common.Security
{
    [DataContract]
    public class RoleData
    {
        [DataMember]
        public int RoleID { get; set; }

        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public string RoleCode { get; set; }

        [DataMember]
        public int? RelatedUserId { get; set; }
    }
}
