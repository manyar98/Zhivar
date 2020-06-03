using AutoMapper;
using OMF.Business;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Common;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Account;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses.Contract;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;
using Zhivar.ViewModel.Contract;

namespace Zhivar.Web.Controllers.BaseInfo
{
    [RoutePrefix("api/NoeSaze")]
    public partial class NoeSazeController : NewApiControllerBaseAsync<NoeSaze, NoeSazeVM>
    {

        public NoeSazeRule Rule => this.BusinessRule as NoeSazeRule;

        protected override IBusinessRuleBaseAsync<NoeSaze> CreateBusinessRule()
        {
            return new NoeSazeRule();
        }


        [Route("GetAllByOrganId")]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


                var list = await Rule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                var list2 = list.Select(x => new { FamilyTree = "کالاها و خدمات", Id = x.ID, Title = x.Title, Parents = ",2,", SystemAccount = 2 }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list2.AsQueryable() });
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }

        [Route("Add")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Add(NoeSazeVM noeSazeVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var noeSaze = new NoeSaze();


            Mapper.Map(noeSazeVM, noeSaze);
            noeSaze.OrganId = organId;

            if (noeSazeVM.ID.HasValue && noeSaze.ID > 0)
            {
                Rule.Update(noeSaze);

            }
            else
            {
               Rule.Insert(noeSaze);
            }

            await Rule.SaveChangesAsync();
            //await _unitOfWork.SaveAllChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = noeSaze });
        }

        [HttpPost]
        public virtual async Task<HttpResponseMessage> Post(NoeSazeVM noeSazeVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            var noeSaze = new NoeSaze();

            Mapper.Map(noeSazeVM, noeSaze);

            Rule.Insert(noeSaze);

            noeSaze.OrganId = organId;

            await Rule.SaveChangesAsync();
            //await _unitOfWork.SaveAllChangesAsync();

            var afetrGoroheKala = new NoeSaze();
            afetrGoroheKala.OrganId = noeSaze.OrganId;
            afetrGoroheKala.Title = noeSaze.Title;
            // گرید آی دی جدید را به این صورت دریافت می‌کند

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = afetrGoroheKala });




        }

        [HttpPut]
        // [Route("Update/{id:int}")]
        public virtual async Task<HttpResponseMessage> Update(int id, NoeSaze noeSaze)
        {
            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });


            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            //  var NoeSaze = new NoeSaze();
            //  Mapper.Map(GoroheKalaVM, NoeSaze);

            Rule.Update(noeSaze);

            await Rule.SaveChangesAsync();
    
            //Return HttpStatusCode.OK
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = noeSaze });

        }


        [HttpPost]
        [Route("Delete")]
        public async Task<HttpResponseMessage> Delete([FromBody]int id)
        {

            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });

            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();
            //await _unitOfWork.SaveAllChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = item });


        }
    }
}