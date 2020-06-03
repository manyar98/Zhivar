using OMF.Business;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.Business.BaseInfo
{
    public class DocTypeRule : BusinessRuleBase<DocType>
    {
        public DocTypeRule()
            : base()
        {

        }

        public DocTypeRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public DocTypeRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public async Task<List<DocType>> GetDocType(ZhivarEnums.DocRequest noeDocID)
        {

            var docTypeQuery = this.UnitOfWork.RepositoryAsync<DocType>()
                                                         .Queryable()
                                                         .Where(ndr => ndr.DocRequestType == noeDocID &&
                                                                       ndr.IsActive == true);




            return await docTypeQuery.ToListAsync();


        }

        protected override void UpdateEntity(DocType entity)
        {
            base.UpdateEntity(entity);
        }

    }
}

