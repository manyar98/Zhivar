﻿using AutoMapper;
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
    [RoutePrefix("api/VahedTol")]
    public partial class VahedTolController : NewApiControllerBaseAsync<VahedTol, VahedTolVM>
    {
        public VahedTolRule Rule => this.BusinessRule as VahedTolRule;

        protected override IBusinessRuleBaseAsync<VahedTol> CreateBusinessRule()
        {
            return new VahedTolRule();
        }


        [Route("GetAllByOrganId")]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                VahedTolRule vahedTolRule = new VahedTolRule();
                var list = await vahedTolRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
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
        public virtual async Task<HttpResponseMessage> Add(VahedTolVM vahedTolVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var vahedTol = new VahedTol();


            Mapper.Map(vahedTolVM, vahedTol);
            vahedTol.OrganId = organId;

            //VahedTolRule vahedTolRule = new VahedTolRule();
            if (vahedTolVM.ID.HasValue && vahedTol.ID > 0)
            {
                Rule.Update(vahedTol);

            }
            else
            {
                Rule.Insert(vahedTol);
            }
            
            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = vahedTol });
        }

        [HttpPost]
        public virtual async Task<HttpResponseMessage> Post(VahedTolVM vahedTolVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            var vahedTol = new VahedTol();

            Mapper.Map(vahedTolVM, vahedTol);
            VahedTolRule vahedTolRule = new VahedTolRule();
            vahedTolRule.Insert(vahedTol);

            vahedTol.OrganId = organId;

            await Rule.SaveChangesAsync();

            var afetrGoroheKala = new VahedTol();
            afetrGoroheKala.OrganId = vahedTol.OrganId;
            afetrGoroheKala.Title = vahedTol.Title;
            // گرید آی دی جدید را به این صورت دریافت می‌کند

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = afetrGoroheKala });




        }

        [HttpPut]
        // [Route("Update/{id:int}")]
        public virtual async Task<HttpResponseMessage> Update(int id, VahedTol vahedTol)
        {
            //VahedTolRule vahedTolRule = new VahedTolRule();

            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });


            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            //  var VahedTol = new VahedTol();
            //  Mapper.Map(GoroheKalaVM, VahedTol);

            Rule.Update(vahedTol);

            await Rule.SaveChangesAsync();

            //Return HttpStatusCode.OK
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = vahedTol });

        }


        [HttpPost]
        [Route("Delete")]
        public virtual async Task<HttpResponseMessage> Delete([FromBody]int id)
        {
            //VahedTolRule vahedTolRule = new VahedTolRule();
            var item = Rule.Find(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });

            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = item });


        }
    }
}