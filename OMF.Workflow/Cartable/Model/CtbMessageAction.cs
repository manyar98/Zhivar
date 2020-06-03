namespace OMF.Workflow.Cartable.Model
{
    public class CtbMessageAction
    {
        public int StepId { get; set; }

        public int ActionId { get; set; }

        public string ActionTitle { get; set; }

        public bool NeedConfirm { get; set; }

        public string ConfirmMessage { get; set; }
    }
}
