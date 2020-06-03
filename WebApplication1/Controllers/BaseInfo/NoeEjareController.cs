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
    [RoutePrefix("api/NoeEjare")]
    public partial class NoeEjareController : NewApiControllerBaseAsync<NoeEjare, NoeEjareVM>
    {

        public NoeEjareRule Rule => this.BusinessRule as NoeEjareRule;

        protected override IBusinessRuleBaseAsync<NoeEjare> CreateBusinessRule()
        {
            return new NoeEjareRule();
        }


        [Route("GetAllByOrganId")]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                NoeEjareRule noeEjareRule = new NoeEjareRule();
                var list = await noeEjareRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
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
        public virtual async Task<HttpResponseMessage> Add(NoeEjareVM noeEjareVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);


            var noeEjare = new NoeEjare();


            Mapper.Map(noeEjareVM, noeEjare);
            noeEjare.OrganId = organId;

            if (noeEjare.ID > 0)
            {
                Rule.Update(noeEjare);

            }
            else
            {
                Rule.Insert(noeEjare);
            }

            await Rule.SaveChangesAsync();
        

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = noeEjare });
        }

        [HttpPost]
        public virtual async Task<HttpResponseMessage> Post(NoeEjareVM noeEjareVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            var noeEjare = new NoeEjare();

            Mapper.Map(noeEjareVM, noeEjare);

            Rule.Insert(noeEjare);

            noeEjare.OrganId = organId;

            await Rule.SaveChangesAsync();
            //await _unitOfWork.SaveAllChangesAsync();

            var afetrGoroheKala = new NoeEjare();
            afetrGoroheKala.OrganId = noeEjare.OrganId;
            afetrGoroheKala.Title = noeEjare.Title;
            // گرید آی دی جدید را به این صورت دریافت می‌کند

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = afetrGoroheKala });




        }

        [HttpPut]
        // [Route("Update/{id:int}")]
        public virtual async Task<HttpResponseMessage> Update(int id, NoeEjare noeEjare)
        {
            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });


            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            //  var NoeEjare = new NoeEjare();
            //  Mapper.Map(GoroheKalaVM, NoeEjare);

            Rule.Update(noeEjare);

            await Rule.SaveChangesAsync();
            //await _unitOfWork.SaveAllChangesAsync();

            //Return HttpStatusCode.OK
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = noeEjare });

        }


        [HttpPost]
        [Route("Delete")]
        public virtual async Task<HttpResponseMessage> Delete([FromBody]int id)
        {

            var item = Rule.Find(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });

            await Rule.DeleteAsync(id);
            await Rule.SaveChangesAsync();
            //await _unitOfWork.SaveAllChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = item });


        }
    }
}