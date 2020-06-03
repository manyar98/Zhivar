using OMF.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.DomainClasses
{
    public static class ZhivarExtensions
    {
        public static bool IsDeveloperUser(this UserContext userContext)
        {
            if (userContext == null)
                return false;

            return userContext.AuthenticationType == (int)ZhivarEnums.ZhivarUserType.Developers;
        }

        public static bool IsSystemUser(this UserContext userContext)
        {
            if (userContext == null)
                return false;

            return userContext.AuthenticationType == (int)ZhivarEnums.ZhivarUserType.SystemUser;
        }

        public static bool IsDeveloperOrSystemUser(this UserContext userContext)
        {
            if (userContext == null)
                return false;

            return IsDeveloperUser(userContext) || IsSystemUser(userContext);
        }

        public static bool IsOrganizationUser(this UserContext userContext)
        {
            if (userContext == null)
                return false;

            return userContext.AuthenticationType == (int)ZhivarEnums.ZhivarUserType.Organization;
        }

        public static bool IsMarkazDarmaniUser(this UserContext userContext)
        {
            if (userContext == null)
                return false;

            return userContext.AuthenticationType == (int)ZhivarEnums.ZhivarUserType.MarkazDarmaniUser;
        }
        public static bool IsOperatorPersonnelUser(this UserContext userContext)
        {
            if (userContext == null)
                return false;
            return userContext.AuthenticationType == (int)ZhivarEnums.ZhivarUserType.MarkazDarmaniUser;
        }

        public static bool IsKargozariPersonnelUser(this UserContext userContext)
        {
            if (userContext == null)
                return false;
            return userContext.AuthenticationType == (int)ZhivarEnums.ZhivarUserType.MarkazDarmaniUser;
        }
    }
}

