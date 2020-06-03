using OMF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{

    public class MadarekPayvast : LoggableEntityName, IActivityLoggable, ILogicalDeletable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Delete | ActionLog.Update;

        public int RecordID { get; set; }
        public int DocTypeID { get; set; }
        public int FileSize { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public bool IsDeleted { get; set; }
       // public TasvirBlob TasvirBlob { get; set; }
        public string TasvirBlobBase64 { get; set; }
        public byte[] Blob { get; set; }
        //public DateTime? InsertDateTime { get; set; }
        //public int? InsertUserID { get; set; }
        //public DateTime? UpdateDateTime { get; set; }
        //public int? UpdateUserID { get; set; }
    }
    //public class TasvirBlob : Entity
    //{
    //    public byte[] Blob { get; set; }
    //}
}
