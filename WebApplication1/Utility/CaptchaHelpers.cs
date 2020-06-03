using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zhivar.Web.Utility
{
    public static class CaptchaHelpers
    {
        public static int CreateSalt()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }
}