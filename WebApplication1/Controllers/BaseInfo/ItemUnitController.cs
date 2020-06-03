using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Zhivar.DataLayer.Context;
using Zhivar.ServiceLayer.Contracts.Accunting;
using System.Net;
using Zhivar.DomainClasses.Accunting;
using Zhivar.ViewModel.Accunting;
using System.Net.Http;
using Newtonsoft.Json;
using Zhivar.Utilities;
using Zhivar.ServiceLayer.Contracts.Common;
using System.Web.Http;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Accounting;
using OMF.Common.Security;
using OMF.Enterprise.MVC;
using OMF.Business;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Accounting;
using Zhivar.Business.Common;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.Web.Controllers.BaseInfo
{
    [RoutePrefix("api/ItemUnit")]
    public partial class ItemUnitController : NewApiControllerBaseAsync<ItemUnit, ItemUnitVM>
    {
        public ItemUnitRule Rule => this.BusinessRule as ItemUnitRule;

        protected override IBusinessRuleBaseAsync<ItemUnit> CreateBusinessRule()
        {
            return new ItemUnitRule();
        }

        [Route("GetAllByOrganId")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetAllByOrganId()
        {
            try
            {
                var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

                ItemUnitRule itemUnitRule = new ItemUnitRule();
                var list = await itemUnitRule.GetAllByOrganIdAsync(Convert.ToInt32(organId));

                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

 
        [Route("Add")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Add(ItemUnitVM itemUnitVM)
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.ValidationError, data = "" });
            }

            var itemUnit = new ItemUnit();
            Mapper.Map(itemUnitVM, itemUnit);

            itemUnit.OrganID = organId;

            if (itemUnitVM.ID>0)
            {
                Rule.Update(itemUnit);
            }
            else
            {
                Rule.Insert(itemUnit);

              
            }

            await this.BusinessRule.UnitOfWork.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = itemUnit });

        }

    

        [Route("Delete")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> Delete([FromBody]int id)
        {

            var item = Rule.Find(id);

            if (item == null)
                return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.NotFound, data = "" });

            await Rule.DeleteAsync(id);

            await Rule.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = "" });

        }

       
    }
}