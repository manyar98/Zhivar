using System.Collections.Generic;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accounting
{
    public interface ITransaction
    {
        //IList<Transaction> GetAll();
        Task<List<TransactionVM>> GetAllByInvoiceIdAsync(int organId,bool isDebt, bool isCredit );
        Task<List<TransactionVM>> GetAllByCostIdAsync(int costId);
        Task<List<Transaction>> GetAllByOrganIdAsync(int organId);
        //Transaction GetById(int id);
        Task<Transaction> GetByIdAsync(int id);
        bool Insert(Transaction transaction);
        bool Update(Transaction transaction);
        bool Delete(Transaction transaction);
        Task<bool> Delete(int id);
        Task<BalanceModelVM> GetBalanceAccountAsync(int accountId);
    }
}