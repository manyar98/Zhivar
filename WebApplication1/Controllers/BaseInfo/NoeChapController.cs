using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.ServiceLayer.Accunting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Net;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;
using System.Net.Http;
using Newtonsoft.Json;
using Zhivar.Utilities;
using Zhivar.ServiceLayer.Contracts.Common;
using System.Web.Http;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accounting;
using Zhivar.ServiceLayer.Contracts.Accounting;
using OMF.Common.Security;
using OMF.Business;
using OMF.Enterprise.MVC;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Common;
using Zhivar.Business.Accounting;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.Web.Controllers.Accunting
{
    [RoutePrefix("api/NoeChap")]
    public partial class NoeChapController : NewApiControllerBaseAsync<NoeChap, NoeChapVM>
    {
        public NoeChapRule Rule => this.BusinessRule as NoeChapRule;

        protected override IBusinessRuleBaseAsync<NoeChap> CreateBusinessRule()
        {
            return new NoeChapRule();
        }

        [Route("GetAllByOrganId")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


            NoeChapVM noeChapVM = new NoeChapVM();
            List<NoeChapVM> noeChapVMs = new List<NoeChapVM>();


            var list = await Rule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list });
        }

        [Route("GetNewNoeChapObject")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetNewNoeChapObject()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                NoeChapVM noeChapVM = new NoeChapVM()
                {
                    Title = "",
                    ID = 0,

                };

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = noeChapVM });
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        [Route("GetNoeChapById")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetNoeChapById([FromBody] int id)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var noeChap = await Rule.FindAsync(id);





            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = noeChap });
        }

        [Route("Add")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Add(NoeChapVM noeChapVM)
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var noeChap = new NoeChap();

                Mapper.Map(noeChapVM, noeChap);

                noeChap.OrganId = organId;

                if (noeChapVM.ID > 0)
                {
                    Rule.Update(noeChap);
                }
                else
                {
                    Rule.Insert(noeChap);
                }

                await this.BusinessRule.UnitOfWork.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = ZhivarEnums.ResultCode.Successful, data = noeChap });// RedirectToAction(MVC.noeChap.ActionNames.Index);
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }


        [Route("Delete")]
        [HttpPost]
        public async Task<HttpResponseMessage> Delete([FromBody]int id)
        {

            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });


            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = item });

        }

        [HttpPost]
        [Route("DeleteNoeChaps")]
        public async Task<HttpResponseMessage> DeleteNoeChaps([FromBody]List<NoeChap> noeChaps)
        {
            try
            {
                foreach (var noeChap in noeChaps)
                {
                    var noeChapFind = await Rule.FindAsync(noeChap.ID);

                    if (noeChapFind != null)
                    {
                        await Rule.DeleteAsync(noeChapFind.ID);


                        await this.BusinessRule.UnitOfWork.SaveChangesAsync();
                    }

                }

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = noeChaps });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "در حذف خطایی به وجود آمد" });
            }
         


           
        }
    }
}