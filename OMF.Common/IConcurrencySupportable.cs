namespace OMF.Common
{
    public interface IConcurrencySupportable
    {
        string RowVersion { get; set; }
    }
}
