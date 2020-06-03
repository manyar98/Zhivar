using OMF.Business;
using OMF.Common;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Enterprise.MVC;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using Zhivar.Business.BaseInfo;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.Web.Controllers.BaseInfo
{
    public class SazeImageController : NewApiControllerBaseAsync<SazeImage, SazeImageVM>
    {
        public SazeImageRule Rule => this.BusinessRule as SazeImageRule;

        protected override IBusinessRuleBaseAsync<SazeImage> CreateBusinessRule()
        {
            return new SazeImageRule();
        }


        protected override Expression<Func<SazeImageVM, bool>> CreateDefaultSearchExpression()
        {
            return base.CreateDefaultSearchExpression();
        }
        protected override IQueryable<SazeImageVM> CreateSearchQuery(IQueryable<SazeImage> query)
        {
            var noeTasvirQuery = BusinessRule.UnitOfWork.Repository<DocType>()
                                                        .Queryable()
                                                        .Where(md => md.IsActive == true);

            var joinQuery = from drTasvirMadarek in query
                           // join noeTasvir in noeTasvirQuery
                          //  on drTasvirMadarek.DocTypeID equals noeTasvir.ID
                            select new SazeImageVM()
                            {
                                SazeId = drTasvirMadarek.SazeId,
                                
                                //RequestID = drTasvirMadarek.RequestID,
                                FileName = drTasvirMadarek.FileName,
                                FileSize = drTasvirMadarek.FileSize,
                                ID = drTasvirMadarek.ID,
                                IsDeleted = drTasvirMadarek.IsDeleted,
                                MimeType = drTasvirMadarek.MimeType,
                               // DocTypeID = noeTasvir.ID,
                              //  NoeTasvirStr = noeTasvir.DocName,

                            };

            return joinQuery;
        }

        //public HttpResponseMessage GetMadrakWithBlob([FromUri] int id)
        //{
        //    SazeImage tasvirmadark = BusinessRule.UnitOfWork.Repository<SazeImage>().Queryable().Where(x => x.ID == id).SingleOrDefault();
        //    var tasvirmadarkBlob = BusinessRule.UnitOfWork.Repository<RequestTasvirBlob>().Queryable().Where(x => x.ID == id).SingleOrDefault();

        //    //  var drTasvirMadarekVM = new SazeImageVM();

        //    SazeImageVM drTasvirMadarekVM = tasvirmadark.Translate<SazeImageVM>();
        //    drTasvirMadarekVM.Blob = tasvirmadarkBlob.Blob;

        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ResultCode.Successful, data = drTasvirMadarekVM });

        //}

        protected override SazeImageVM TranslateEntityToEntityVM(SazeImage entity)
        {
            SazeImageVM SazeImageVM = base.TranslateEntityToEntityVM(entity);
            //if (entity.RequestTasvirBlob != null)
            //{
            //    SazeImageVM.TasvirBlobBase64 = string.Format(@"data:image/jpeg;base64,{0}", Convert.ToBase64String(entity.RequestTasvirBlob.Blob));
            //    SazeImageVM.Blob = entity.RequestTasvirBlob.Blob;
            //}

            return SazeImageVM;
        }

        protected override SazeImage TranslateEntityVMToEntity(SazeImageVM entityVM)
        {
            var entity = base.TranslateEntityVMToEntity(entityVM);
         //   entity. = new RequestTasvirBlob();
            entity.Blob = Convert.FromBase64String(entityVM.TasvirBlobBase64);
            return entity;
        }
        [HttpPost]
        [Route("DeleteImage")]
        public async Task<HttpResponseMessage> DeleteImage([FromBody] int id)
        {
            await Rule.DeleteAsync(id);
            await Rule.SaveChangesAsync();
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });
        }

    }
}
