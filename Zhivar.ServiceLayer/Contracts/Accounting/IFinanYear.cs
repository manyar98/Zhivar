using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;


namespace Zhivar.ServiceLayer.Contracts.Accunting
{
    public interface IFinanYear
    {
        IList<FinanYear> GetAll();
        Task<IList<FinanYear>> GetAllAsync();
        IList<FinanYear> GetAllByOrganId(int personId);
        Task<IList<FinanYear>> GetAllByOrganIdAsync(int personId);
        FinanYear GetById(int id);
        Task<FinanYear> GetByIdAsync(int id);
        bool Insert(FinanYear finanYear);
        bool Update(FinanYear finanYear);
        bool Delete(FinanYear finanYear);
        bool Delete(int id);
        Task<FinanYear> GetCurrentFinanYear(int organId);
    }
}

