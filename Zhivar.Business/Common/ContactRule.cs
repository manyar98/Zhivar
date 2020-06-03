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
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;
using AutoMapper;

namespace Zhivar.Business.Common
{
    public partial class ContactRule : BusinessRuleBase<Contact>
    {
        public ContactRule()
            : base()
        {

        }

        public ContactRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ContactRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }
        public Contact GetContactByUserId(int userId)

        {
            var ContactID = Convert.ToInt32(SecurityManager.CurrentUserContext.TagInt1);

            var Contact = this.Queryable().Where(x => x.ID == ContactID).SingleOrDefault();

            return Contact;
        }

        public async Task<List<Contact>> GetAllByOrganIdAsync(int organId)
        {
            var ContactQuery = this.Queryable().Where(x => x.OrganId == organId);

            return await ContactQuery.ToListAsync2();


          //  List<ContactVM> contactVMs = Mapper.Map<IList<Contact>, List<ContactVM>>(contactList);
            
           
            //return contactVMs;
        }

        public async Task UpdateContact(DomainClasses.ZhivarEnums.NoeFactor invoiceType, int contactId)
        {
            var contact = await this.Queryable().Where(x => x.ID == contactId).SingleOrDefaultAsync2();
            if (contact != null)
            {
                switch (invoiceType)
                {
                    case DomainClasses.ZhivarEnums.NoeFactor.Sell:
                        contact.IsCustomer = true;
                        break;
                    case DomainClasses.ZhivarEnums.NoeFactor.Buy:
                        contact.IsVendor = true;
                        break;
                    case DomainClasses.ZhivarEnums.NoeFactor.ReturnSell:
                        contact.IsVendor = true;
                        break;
                    case DomainClasses.ZhivarEnums.NoeFactor.ReturnBuy:
                        contact.IsCustomer = true;
                        break;
                    default:
                        break;
                }

                Update(contact);
            }
        }
    }
}