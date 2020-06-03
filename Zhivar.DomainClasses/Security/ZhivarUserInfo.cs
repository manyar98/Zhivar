//using Behsho.Common.Profile;
using OMF.Common;
using OMF.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Security
{
    //public class ZhivarUserInfo : UserInfo, ILogicalDeletable//, IActivityLoggable
    //{
    //    public int? ShakhsId
    //    {
    //        get
    //        {
    //            return this.TagInt1 != null ? this.TagInt1 : (int?)null;
    //        }
    //        set
    //        {
    //            this.TagInt1 = value != null ? value : null;
    //        }
    //    }

    //    //public int? MarkazDarmaniId
    //    //{
    //    //    get
    //    //    {
    //    //        return this.TagInt2 != null ? this.TagInt2 : (int?)null;
    //    //    }
    //    //    set
    //    //    {
    //    //        this.TagInt2 = value != null ? value : null;
    //    //    }
    //    //}

    //    public string DisplayName
    //    {
    //        get
    //        {
    //            return this.Tag2;
    //        }
    //        set
    //        {
    //            this.Tag2 = value;
    //        }
    //    }
    //}

    public class ZhivarUserInfo2

    {
        public int ID { get; set; }
        public Gender? Jensiat { get; set; }
        public string Nam { get; set; }
        public string NameKhanevadegi { get; set; }
        public string CodeMeli { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string UserName { get; set; }
        //public string PlainPassword { get; set; }
        //public string Password { get; set; }
        public string Mobile { get; set; }
        //public int? MarkazDarmaniId { get; set; }
        //public string OTPCode { get; set; }
        public ZhivarEnums.ZhivarUserType NoeKarbar { get; set; }
        public int? ShakhsId { get; set; }
        public DateTime? TarikheTavallod { get; set; }

    }
}
