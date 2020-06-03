using OMF.Common;
using System.Collections.Generic;
using Zhivar.DomainClasses.Common;
using static OMF.Common.Enums;

namespace Zhivar.DomainClasses.Accunting
{
    public class Sahamdaran : Entity, IActivityLoggable
    {
        public ActionLog ActionsToLog => ActionLog.Insert | ActionLog.Update | ActionLog.Delete;
        public int ShakhsId { get; set; }

        public int OrganId { get; set; }

        public decimal Sahm { get; set; }
        public int HasShakhsInDatabase { get; set; }


    }

}