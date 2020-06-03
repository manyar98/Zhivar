using System;
using System.Collections.Generic;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCValidation = OMF.Common.Validation;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accunting;
using Zhivar.DomainClasses.Common;

namespace Zhivar.Business.BaseInfo
{
    public partial class ShareholderRule : BusinessRuleBase<Shareholder>
    {
        public ShareholderRule()
            : base()
        {

        }

        public ShareholderRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ShareholderRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Shareholder> GetAllByOrganId(int organId)
        {
            var Shareholders = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return Shareholders;
        }
        public async Task<IList<ShareholderVM>> GetAllByOrganIdAsync(int organId)
        {
            var shareholders = this.unitOfWork.RepositoryAsync<Shareholder>().Queryable().Where(x => x.OrganId == organId);
            var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();

            var joinQuery = from shareholder in shareholders
                            join contact in contactQuery
                            on shareholder.ContactId equals contact.ID
                            select new ShareholderVM()
                            {

                                Address = contact.Address,
                                Website = contact.Website,
                                ID = contact.ID,
                                City = contact.City,
                                Code = contact.Code,
                                ContactEmail = contact.Email,
                                Email = contact.Email,
                                ContactType = contact.ContactType,
                                Credits = contact.Credits,
                                EconomicCode = contact.EconomicCode,
                                Fax = contact.Fax,
                                FirstName = contact.FirstName,
                                IsCustomer = contact.IsCustomer,
                                IsEmployee = contact.IsEmployee,
                                IsShareHolder = contact.IsShareHolder,
                                IsVendor = contact.IsVendor,
                                LastName = contact.LastName,
                                Liability = contact.Liability,
                                Mobile = contact.Mobile,
                                Name = contact.Name,
                                NationalCode = contact.NationalCode,
                                Note = contact.Note,
                                Phone = contact.Phone,
                                PostalCode = contact.PostalCode,
                                Rating = contact.Rating,
                                RegistrationDate = contact.RegistrationDate,
                                RegistrationNumber = contact.RegistrationNumber,
                                SharePercent = shareholder.SharePercent,
                                State = contact.State,


                            };

            return await joinQuery.ToListAsync2();

        }
        public async Task<IList<Shareholder>> GetAllAsync()
        {
            return await this.Queryable().ToListAsync2();
        }
        public async Task<List<Shareholder>> GetShareholderByPersonIdAsync(int organId)
        {
            return await this.Queryable().Where(x => x.OrganId == organId).ToListAsync2();
        }

        public async Task<Shareholder> GetShareholderByContractIdAsync(int organId, int contactId)
        {
            return await this.Queryable().Where(x => x.OrganId == organId && x.ContactId == contactId && x.IsActive == true).SingleOrDefaultAsync2();
        }

    }
}