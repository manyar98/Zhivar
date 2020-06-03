using OMF.Business;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhivar.DomainClasses.BaseInfo;
using System.Drawing.Imaging;

namespace Zhivar.Business.BaseInfo
{
    public class SazeImageRule : BusinessRuleBase<SazeImage>
    {
        public SazeImageRule()
            : base()
        {

        }

        public SazeImageRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public SazeImageRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        protected override SazeImage FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            return entity;
        }

        protected override async Task<SazeImage> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity == null)
                return null;

            return entity;
        }
        protected override List<string> CheckInsertRules(SazeImage entity)
        {
            List<string> failures = base.CheckInsertRules(entity);
            //var inputBytes = Convert.FromBase64String(entity.RequestTasvirBlob);
            using (var inputStream = new MemoryStream(entity.Blob))
            {
                var image = Image.FromStream(inputStream);
                var size = image.Size;
                var width = size.Width;
                var height = size.Height;

                //if ( (width > 200 || height > 300))
                //    failures.Add("حداکثر سایز تصویر باید 200 * 300 باشد");

                if ((width > 500 || height > 500))
                    failures.Add("حداکثر سایز تصویر شناسنامه باید 500 * 500 باشد");
            }
            return failures;
        }
        protected override List<string> CheckUpdateRules(SazeImage entity)
        {
            List<string> failures = base.CheckUpdateRules(entity);
            //var inputBytes = Convert.FromBase64String(entity.RequestTasvirBlob);
            using (var inputStream = new MemoryStream(entity.Blob))
            {
                var image = Image.FromStream(inputStream);
                var size = image.Size;
                var width = size.Width;
                var height = size.Height;

                //if (entity.DocTypeID == (int)BRDEnums.PayvastEnums.TasvirShakhs && (width > 200 || height > 300))
                //    failures.Add("حداکثر سایز تصویر شخص باید 200 * 300 باشد");

                //if (width > 500 || height > 500)
                //    failures.Add("حداکثر سایز تصویر باید 500 * 500 باشد");
            }
            return failures;
        }
        protected override void InsertEntity(SazeImage entity)
        {
            entity.ID = entity.ID;
            entity.ObjectState = OMF.Common.Enums.ObjectState.Added;


            base.InsertEntity(entity);
        }

        protected override void UpdateEntity(SazeImage entity)
        {
            SazeImage entityDb = this.Find(entity.ID);

           // entityDb.DocTypeID = entity.DocTypeID;
            entityDb.FileName = entity.FileName;
            entityDb.FileSize = entity.FileSize;
            entityDb.MimeType = entity.MimeType;

            entityDb.Blob = entity.Blob;
            entityDb.ObjectState = OMF.Common.Enums.ObjectState.Modified;




            base.UpdateEntity(entityDb);
        }
        protected override void DeleteEntity(SazeImage entity)
        {
            SazeImage entityDb = this.Find(entity.ID);

            //entityDb.DocTypeID = entity.DocTypeID;
            entityDb.FileName = entity.FileName;
            entityDb.FileSize = entity.FileSize;
            entityDb.MimeType = entity.MimeType;

            entityDb.Blob = entity.Blob;
            entityDb.ObjectState = OMF.Common.Enums.ObjectState.Deleted;




            base.DeleteEntity(entityDb);
        }
    }
}
