namespace OMF.Workflow
{
    public class WorkflowContinueInfo
    {
        public int ActionId { get; set; }

        public int CurrentStateID { get; set; }

        public string UserComment { get; set; }

        public int? TargetUserId { get; set; }

        public WFExchangeData ExchangeData { get; set; }
    }
}
