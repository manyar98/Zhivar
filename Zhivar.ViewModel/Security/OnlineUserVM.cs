using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zhivar.ViewModel.Security
{
    public class OnlineUserVM
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public DateTime LastLoginDate { get; set; }       
    }
}