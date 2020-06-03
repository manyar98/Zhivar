using OMF.Business;
using OMF.Common;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Zhivar.Business.BaseInfo;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Web.Controllers.BaseInfo
{
   
    public class DocTypeController : NewApiControllerBaseAsync<DocType, DocTypeVM>
    {
        public DocTypeRule Rule => this.BusinessRule as DocTypeRule;

        protected override IBusinessRuleBaseAsync<DocType> CreateBusinessRule()
        {
            return new DocTypeRule();
        }
        [HttpPost]
        public async Task<HttpResponseMessage> GetDocType(GetDocTypeBusi getDocTypeBusi)
        {
            try
            {
                SecurityManager.ThrowIfUserContextNull();

                using (DocTypeRule noeDarkhastPayvastRule = new DocTypeRule())
                {
                    var noeTasvir = await noeDarkhastPayvastRule.GetDocType(getDocTypeBusi.noeDocID);

                    List<KeyValueVM> result = noeTasvir.Select(p => new KeyValueVM() { Key = p.ID, Value = p.DocName }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ResultCode.Successful, data = new { records = result } });
                }

            }
            catch (Exception ex)
            {
                return await this.HandleExceptionAsync(ex);
            }
            finally
            {
                BusinessRule.Dispose();
            }
        }

        protected override IQueryable<DocTypeVM> CreateSearchQuery(IQueryable<DocType> query)
        {
            return base.CreateSearchQuery(query);
        }

        protected override DocType TranslateEntityVMToEntity(DocTypeVM entityVM)
        {
            return base.TranslateEntityVMToEntity(entityVM);
        }

        protected override DocTypeVM TranslateEntityToEntityVM(DocType entity)
        {
            return base.TranslateEntityToEntityVM(entity);
        }
        [HttpPost]
        public async Task<HttpResponseMessage> Deactive([FromBody] int Id)
        {
            var noeDarkhastPayvast = BusinessRule.UnitOfWork.Repository<DocType>().Queryable().Where(x => x.ID == Id).SingleOrDefault();

            noeDarkhastPayvast.IsActive = false;

            this.BusinessRule.UnitOfWork.Repository<DocType>().Update(noeDarkhastPayvast);


            await this.BusinessRule.UnitOfWork.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ResultCode.Successful, data = "" });
        }

    }

    public class GetDocTypeBusi
    {
       public ZhivarEnums.DocRequest noeDocID { get; set; }
    }
}