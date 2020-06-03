using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface ICheque
    {
        IList<Cheque> GetAll();
        Task<IList<Cheque>> GetAllAsync();
        IList<Cheque> GetAllByOrganId(int personId);
        Task<IList<Cheque>> GetAllByOrganIdAsync(int personId);
        Cheque GetById(int id);
        Task<Cheque> GetByIdAsync(int id);
        bool Insert(Cheque cheque);
        bool Update(Cheque cheque);
        bool Delete(Cheque cheque);
        bool Delete(int id);
        Task<List<ChequeToPayVM>> GetChequesToPay(int organId);
    }
}

