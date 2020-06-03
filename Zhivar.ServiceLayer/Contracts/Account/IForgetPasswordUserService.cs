using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ServiceLayer.Contracts.Account
{
    public interface IForgetPasswordUserService
    {
        Task<bool> CanUseService(int userId);
        Task Add(int userId);
        Task<double> Calculate(int id);
    }
}
