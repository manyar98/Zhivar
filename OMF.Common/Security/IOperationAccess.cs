namespace OMF.Common.Security
{
    public interface IOperationAccess
    {
        bool CanInsert { get; set; }

        bool CanView { get; set; }

        bool CanUpdate { get; set; }

        bool CanDelete { get; set; }

        bool CanExport { get; set; }

        bool CanPrint { get; set; }

        bool CanImport { get; set; }
    }
}
