using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IInvoice
    {
        //IList<Invoice> GetAll();
        //Task<IList<InvoiceVM>> GetAllByOrganIdAsync(int organId, Enums.InvoiceType noeInvoice);
        //Task<IList<Invoice>> GetAllByOrganIdAsync(int organId);
        Task<List<InvoiceItemContactItem>> GetAllByContactIdAsync(int organId, int contactId);
        //Task<IList<Invoice>> GetAllAsync();
        //Invoice GetById(int id);
        //Task<Invoice> GetByIdAsync(int id);
        //Task <bool> Insert(Invoice invoice);
        Task <bool> Update(Invoice invoice);
        //bool Delete(Invoice invoice);
        //Task<bool> Delete(int id);
    }

}