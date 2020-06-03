using OMF.Business;
using OMF.Common.Security;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.Business.BaseInfo
{
    public class MadarekPayvastRule : BusinessRuleBase<MadarekPayvast>
    {
        public MadarekPayvastRule()
            : base()
        {

        }

        public MadarekPayvastRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public MadarekPayvastRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        protected override MadarekPayvast FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            //if (entity.TasvirBlob == null)
            //    this.LoadReference<TasvirBlob>(entity, dtd => dtd.TasvirBlob);

            return entity;
        }

        protected override async Task<MadarekPayvast> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            //if (entity.TasvirBlob == null)
            //    await this.LoadReferenceAsync<TasvirBlob>(entity, dtd => dtd.TasvirBlob);

            return entity;
        }

        protected override void InsertEntity(MadarekPayvast entity)
        {
            //entity.TasvirBlob.ID = entity.ID;
            //entity.TasvirBlob.ObjectState = OMF.Common.Enums.ObjectState.Added;

            base.InsertEntity(entity);
        }

        protected override void UpdateEntity(MadarekPayvast entity)
        {
            MadarekPayvast entityDb = this.Find(entity.ID);

            entityDb.DocTypeID = entity.DocTypeID;
            entityDb.FileName = entity.FileName;
            entityDb.FileSize = entity.FileSize;
            entityDb.MimeType = entity.MimeType;

            //entityDb.TasvirBlob.Blob = entity.TasvirBlob.Blob;
            //entityDb.TasvirBlob.ObjectState = OMF.Common.Enums.ObjectState.Modified;



            base.UpdateEntity(entityDb);
        }

        protected override List<string> CheckInsertRules(MadarekPayvast entity)
        {
            return base.CheckInsertRules(entity);
        }
    }
}
