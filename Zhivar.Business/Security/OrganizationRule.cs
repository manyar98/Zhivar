using OMF.Security.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OMF.EntityFramework.UnitOfWork;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using System.Data.Entity;
using OMF.Common.Configuration;
using Behsho.Common;
using OMF.Business;
using OMF.EntityFramework.Query;
using OMF.Common;

namespace Zhivar.Business.Security
{
    public class OrganizationRule : BusinessRuleBase<Organization>
    {
        public OrganizationRule()
        {
            UseForAnonymousUser = true;
        }
        //بررسی دسترسی های کاربر لاگین کرده برای ویرایش اطلاعات سازمان
        protected override List<string> CheckUpdateRules(Organization entity)
        {
            var failures = base.CheckUpdateRules(entity);
            //if (SecurityManager.CurrentUserContext.IsOrganizationUser())
            //{
            //    if (entity.ParentId == SecurityManager.CurrentUserContext.OrganizationId ||
            //        (entity.ID != SecurityManager.CurrentUserContext.OrganizationId && !IsChildOfCurrentUserOrganization(entity.ID)))
            //        failures.Add("امکان تغییر اطلاعات برای این کاربر وجود ندارد");
            //}
            return failures;
        }

        bool IsChildOfCurrentUserOrganization(int organId)
        {
            var orgData = this.Queryable().Where(org => org.ID == organId).Select(org => new { org.ID, org.ParentId }).SingleOrDefault();
            if (orgData == null)
                return false;

            if (orgData.ParentId == null)
                return false;

            if (orgData.ParentId == SecurityManager.CurrentUserContext.OrganizationId)
                return true;

            return IsChildOfCurrentUserOrganization(orgData.ID);
        }
    }
}
