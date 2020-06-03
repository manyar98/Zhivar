using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Workflow
{
    public class Enums
    {
        public enum WfStateStatus
        {
            Open = 1,
            Close = 2,
            Refuse = 3,
        }

        public enum MessageType
        {
            Task = 1,
            Message = 2,
        }

        public enum Priority
        {
            VeryLow,
            Low,
            Medium,
            High,
            VeryHigh,
        }

        public enum StepType
        {
            Start = 1,
            HumanActivity = 10, // 0x0000000A
            SystemActivity = 20, // 0x00000014
            SendToStarter = 30, // 0x0000001E
            End = 100, // 0x00000064
        }

        public enum StartType
        {
            SingleEntity = 1,
            MultiEntity = 2,
            Request = 3,
        }

        public enum FetchHistoryMode
        {
            AllStates = 1,
            UpToCurrentState = 2,
        }

        public enum MessageCategory
        {
            InProcess = 1,
            Archive = 2,
        }


    }
}
