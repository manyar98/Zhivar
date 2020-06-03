using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IPayRecevie
    {
        //IList<PayRecevie> GetAll();
        Task<IList<PayRecevieVM>> GetAllByOrganIdAsync(int organId);
        Task<IList<PayRecevie>> GetAllByOrganIdAsync(int organId,bool iSRecevie);
        //Task<IList<PayRecevie>> GetAllAsync();
        Task<IList<PayRecevie>> GetByInvoiceIdAsync(int invoiceId);
        Task<IList<PayRecevie>> GetByCostIdAsync(int costId);
        //PayRecevie GetById(int id);
        //Task<PayRecevie> GetByIdAsync(int id);
        //bool Insert(PayRecevie payRecevie);
        bool Update(PayRecevie payRecevie);
        //bool Delete(PayRecevie payRecevie);
        //Task<bool> Delete(int id);
        Task<List<Document>> GetDoucumentIDByChequeIdAsync(int chequeId);
    }
}