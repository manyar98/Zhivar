namespace OMF.Workflow.Cartable.Model
{
    public class NextStepsRequest
    {
        public int ActionId { get; set; }

        public int StepId { get; set; }

        public int? WfInstanceStateID { get; set; }

        public int? WfInstanceID { get; set; }
    }
}
