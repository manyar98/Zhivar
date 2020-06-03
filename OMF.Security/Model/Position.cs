namespace OMF.Security.Model
{
    public class Position : RoleBase
    {
        public int? OrganizationId { get; set; }

        public int OrganizationUnitChartId { get; set; }
    }
}
