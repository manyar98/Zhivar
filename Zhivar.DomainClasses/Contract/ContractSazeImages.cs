using OMF.Common;
using System.ComponentModel.DataAnnotations.Schema;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.BaseInfo
{
    public class ContractSazeImages : LoggableEntity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;

        public int Type { get; set; }
        public int ContractSazeId { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public byte[] Blob { get; set; }
        public bool IsDeleted { get; set; }

    }
}
