using System.Collections.Generic;
using Microsoft.Owin.Security;

namespace Zhivar.Web.ViewModels.Identity
{
    public class ManageLoginsViewModel
    {
      //  public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }
}