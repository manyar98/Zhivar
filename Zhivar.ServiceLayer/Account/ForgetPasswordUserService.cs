using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.ServiceLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Account;

namespace Zhivar.ServiceLayer.Contracts.Account
{
    public class ForgetPasswordUserService : IForgetPasswordUserService
    {
        private readonly IMappingEngine Mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDbSet<ForgetPasswordUser> _forgetPasswordUser;
        public ForgetPasswordUserService(IUnitOfWork unitOfWork, IMappingEngine mappingEngine)
        {
            _unitOfWork = unitOfWork;
            _forgetPasswordUser = unitOfWork.Set<ForgetPasswordUser>();
            Mapper = mappingEngine;
        } 
        public async Task<bool> CanUseService(int userId)
        {
            var firstDate = DateTime.Now.AddMonths(-1);

           var count = await _forgetPasswordUser.Where(x => x.UserId == userId && x.Date <= DateTime.Now && x.Date >= firstDate).CountAsync();

            if (count >= 3)
                return false;

            return true;
        }
        public async Task Add(int userId) {
            var forgetPasswordUser = new ForgetPasswordUser()
            {
                UserId = userId,
                Date = DateTime.Now
            };

            _forgetPasswordUser.Add(forgetPasswordUser);
        }

        public async Task<double> Calculate(int id)
        {
            var firstDate = DateTime.Now.AddMonths(-1);
            var forgetPasswordUser = await _forgetPasswordUser.Where(x => x.Date <= DateTime.Now && x.Date >= firstDate).OrderBy(x => x.ID).FirstOrDefaultAsync();

            var diff = DateTime.Now - forgetPasswordUser.Date;
            return 31 - diff.TotalDays;
        }
    }
}
 