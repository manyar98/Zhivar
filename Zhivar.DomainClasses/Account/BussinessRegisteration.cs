using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.DomainClasses.Account
{
    public class BussinessRegisteration
    {
        public string CodePosti { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string MobileCaptcha { get; set; }
        public string Captcha { get; set; }
        public bool IsFinalStep { get; set; }
    }
}
