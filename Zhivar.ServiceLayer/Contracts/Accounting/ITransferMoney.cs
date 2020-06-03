using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface ITransferMoney
    {
        //IList<TransferMoney> GetAll();
        //Task<IList<TransferMoney>> GetAllAsync();
        Task<IList<TransferMoney>> GetAllByOrganIdAsync(int personId);
        //TransferMoney GetById(int id);
        Task<TransferMoney> GetByIdAsync(int id);
        Task<bool> Insert(TransferMoney transferMoney);
        bool Update(TransferMoney transferMoney);
        Task<TransferMoney> Delete(TransferMoney transferMoney);
        Task<TransferMoney> Delete(int id);
        Task<TransferMoney> GetByDocIdAsync(int id);
    }
}
