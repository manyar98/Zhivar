using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Contracts.BaseInfo
{
    public interface IShareholder
    {
        IList<Shareholder> GetAll();
        Task<IList<Shareholder>> GetAllAsync();
        Task<IList<ShareholderVM>> GetAllByOrganIdAsync(int personId);
        Shareholder GetById(int id);
        Task<Shareholder> GetByIdAsync(int id);
        bool Insert(Shareholder shareholder);
        bool Update(Shareholder shareholder);
        bool Delete(Shareholder shareholder);
        bool Delete(int id);
        Task<Shareholder> GetShareholderByContractIdAsync(int organId, int contactId);
        Task<List<Shareholder>> GetShareholderByPersonIdAsync(int id);
    }
}
