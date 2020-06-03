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

namespace Zhivar.Business.Common
{
    public partial class PersonRule : BusinessRuleBase<Person>
    {
        public PersonRule()
            : base()
        {

        }

        public PersonRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public PersonRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }
        //public Person GetPersonByUserId(int userId)

        //{
        //    var personID = Convert.ToInt32(SecurityManager.CurrentUserContext.TagInt1);

        //    var person = this.Queryable().Where(x => x.ID == personID).SingleOrDefault();

        //    return person;
        //}

        public async Task<List<PersonVM>> GetAllByOrganIdAsync(int organId)
        {
            var personQuery = this.Queryable().Where(x => x.OrganId == organId);

            var resualt = from person in personQuery
                          select new PersonVM
                          {
                              ID = person.ID,
                              Nam = person.Nam,
                              NamKhanvadegi = person.NamKhanvadegi
                          };
            return await resualt.ToListAsync2();
        }
    }
}