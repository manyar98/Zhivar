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

namespace Zhivar.Web.Controllers.Accunting
{

    public partial class ItemGroupController : NewApiControllerBaseAsync<ItemGroup, ItemGroupVM>
    {

        public ItemGroupRule Rule => this.BusinessRule as ItemGroupRule;

        protected override IBusinessRuleBaseAsync<ItemGroup> CreateBusinessRule()
        {
            return new ItemGroupRule();
        }


        [Route("GetAllByOrganId")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                var list = await Rule.GetAllByOrganIdAsync(Convert.ToInt32(organId));
                var list2 = list.Select(x => new { FamilyTree = "کالاها و خدمات", Id = x.ID, Name = x.Name, Parents = ",2,", SystemAccount = 2 }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list2.AsQueryable() });
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        [Route("Add")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Add([FromBody]ItemGroupVM itemGroupVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var itemGroup = new ItemGroup();


            Mapper.Map(itemGroupVM, itemGroup);
            itemGroup.OrganID = organId;

            if ( itemGroup.ID>0)
            {
                Rule.Update(itemGroup);

            }
            else
            {
                Rule.Insert(itemGroup);
            }

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = itemGroup });
        }
      
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Post(ItemGroupVM itemGroupVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });
   

            var itemGroup = new ItemGroup();

            Mapper.Map(itemGroup, itemGroupVM);

            Rule.Insert(itemGroup);

            itemGroup.OrganID = organId;

  
            await Rule.SaveChangesAsync();
            var afetrItemGroup = new ItemGroup();
            afetrItemGroup.OrganID = itemGroup.OrganID;
            afetrItemGroup.Name = itemGroup.Name;
            // گرید آی دی جدید را به این صورت دریافت می‌کند

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = afetrItemGroup });




        }

        [HttpPut]
        // [Route("Update/{id:int}")]
        public virtual async Task<HttpResponseMessage> Update(int id, ItemGroup itemGroup)
        {
            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });


            if (!ModelState.IsValid || id != item.ID)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = "" });


            //  var ItemGroup = new ItemGroup();
            //  Mapper.Map(ItemGroupVM, ItemGroup);

            Rule.Update(itemGroup);

            await Rule.SaveChangesAsync();

            //Return HttpStatusCode.OK
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = itemGroup });

        }


        [HttpPost]
        [Route("Delete")]
        public async Task<HttpResponseMessage> Delete([FromBody]int id)
        {

            var item = await Rule.FindAsync(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });

            await  Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Exception, data = item });


        }
    }
}