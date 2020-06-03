using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IChequeBank
    {
        IList<ChequeBank> GetAll();
        Task<IList<ChequeBank>> GetAllAsync();
        IList<ChequeBank> GetAllByOrganId(int personId);
        Task<IList<ChequeBank>> GetAllByOrganIdAsync(int personId);
        ChequeBank GetById(int id);
        Task<ChequeBank> GetByIdAsync(int id);
        bool Insert(ChequeBank chequeBank);
        bool Update(ChequeBank chequeBank);
        bool Delete(ChequeBank chequeBank);
        bool Delete(int id);
    }
}

