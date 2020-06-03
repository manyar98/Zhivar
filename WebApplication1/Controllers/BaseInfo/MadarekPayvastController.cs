using OMF.Business;
using OMF.Common;
using OMF.Enterprise.MVC;
using OMF.EntityFramework.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Zhivar.Business.BaseInfo;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.BaseInfo
{
    public class MadarekPayvastController : NewApiControllerBaseAsync<MadarekPayvast, MadarekPayvastVM>
    {
        public MadarekPayvastRule Rule => this.BusinessRule as MadarekPayvastRule;

        protected override IBusinessRuleBaseAsync<MadarekPayvast> CreateBusinessRule()
        {
            return new MadarekPayvastRule();
        }

        protected override Expression<Func<MadarekPayvastVM, bool>> CreateSearchExpressionByFilterInfo(FilterInfo filterInfo)
        {
            return base.CreateSearchExpressionByFilterInfo(filterInfo);
        }
        protected override Expression<Func<MadarekPayvastVM, bool>> CreateDefaultSearchExpression()
        {
            return base.CreateDefaultSearchExpression();
        }
        protected override IQueryable<MadarekPayvastVM> CreateSearchQuery(IQueryable<MadarekPayvast> query)
        {
            var noeTasvirQuery = BusinessRule.UnitOfWork.Repository<DocType>().Queryable();

            var joinQuery = from drTasvirMadarek in query
                            join noeTasvir in noeTasvirQuery
                            on drTasvirMadarek.DocTypeID equals noeTasvir.ID
                            select new MadarekPayvastVM()
                            {
                                RecordID = drTasvirMadarek.RecordID,
                                FileName = drTasvirMadarek.FileName,
                                FileSize = drTasvirMadarek.FileSize,
                                ID = drTasvirMadarek.ID,
                                IsDeleted = drTasvirMadarek.IsDeleted,
                                MimeType = drTasvirMadarek.MimeType,
                                DocTypeID = noeTasvir.ID,
                                NoeTasvirStr = noeTasvir.DocName,

                            };

            return joinQuery;
        }

        public HttpResponseMessage GetMadrakWithBlob([FromUri] int id)
        {
            MadarekPayvast tasvirmadark = BusinessRule.UnitOfWork.Repository<MadarekPayvast>().Queryable().Where(x => x.ID == id).SingleOrDefault();
          //  var tasvirmadarkBlob = BusinessRule.UnitOfWork.Repository<TasvirBlob>().Queryable().Where(x => x.ID == id).SingleOrDefault();

            //  var drTasvirMadarekVM = new MadarekPayvastVM();

            MadarekPayvastVM drTasvirMadarekVM = tasvirmadark.Translate<MadarekPayvastVM>();
           // drTasvirMadarekVM.Blob = tasvirmadarkBlob.Blob;

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ResultCode.Successful, data = drTasvirMadarekVM });

        }

        protected override MadarekPayvastVM TranslateEntityToEntityVM(MadarekPayvast entity)
        {
            MadarekPayvastVM MadarekPayvastVM = base.TranslateEntityToEntityVM(entity);
            //if (entity.TasvirBlob != null)
            //{
            //    MadarekPayvastVM.TasvirBlobBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(entity.TasvirBlob.Blob));
            //    MadarekPayvastVM.Blob = entity.TasvirBlob.Blob;
            //}

            if (entity.Blob != null)
            {
                MadarekPayvastVM.TasvirBlobBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(entity.Blob));
                MadarekPayvastVM.Blob = entity.Blob;
            }

            return MadarekPayvastVM;
        }

        protected override MadarekPayvast TranslateEntityVMToEntity(MadarekPayvastVM entityVM)
        {
            var entity = base.TranslateEntityVMToEntity(entityVM);

            entity.Blob = Convert.FromBase64String(entityVM.TasvirBlobBase64);

            //entity.TasvirBlob = new TasvirBlob();
            //entity.TasvirBlob.Blob = Convert.FromBase64String(entityVM.TasvirBlobBase64);
            return entity;
        }

     
    }
}
