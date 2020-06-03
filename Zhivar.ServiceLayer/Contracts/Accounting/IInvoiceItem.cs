using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IInvoiceItem
    {
        IList<InvoiceItem> GetAll();
        IList<InvoiceItem> GetAllByInvoiceId(int invoiceId);
        Task<IList<InvoiceItem>> GetAllByInvoiceIdAsync(int invoiceId);
        Task<IList<InvoiceItem>> GetAllAsync();
        InvoiceItem GetById(int id);
        Task<InvoiceItem> GetByIdAsync(int id);
        bool Insert(InvoiceItem invoiceItem);
        bool Update(InvoiceItem invoiceItem);
        bool Delete(InvoiceItem invoiceItem);
        bool Delete(int id);
        Task<List<InvoiceItem>> GetByInvoiceIdAsync(int invoiceId);
    }
}