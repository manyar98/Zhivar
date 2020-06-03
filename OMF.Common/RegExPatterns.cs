using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Common
{
    public class RegExPatterns
    {
        public const string PreNoOfMobile = "^9[0|1|2|3|4|9][0-9]{1}[-][0-9]{1}$";
        public const string Mobile = "^9[0|1|2|3|4|9][0-9]{8}$";
        public const string IPAddress = "^(?<First>[01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.(?<Second>[01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.(?<Third>[01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.(?<Fourth>[01]?\\d\\d?|2[0-4]\\d|25[0-5])$";
        public const string Latitude = "^([0-9]{1,2}|[0-9]{1,2}[.][0-9]{1,6}|)$";
        public const string Time = "^(([0-1][0-9])|([2][0-3])):([0-5][0-9])$";
        public const string Longitude = "^([0-9]{1,2}|[0-9]{1,2}[.][0-9]{1,6}|)$";
        public const string PostalCode = "^[^0|2]{10}$";
        public const string Email = "^[_a-z0-9A-Z]+(\\.[_a-z0-9A-Z]+)*@[a-z0-9-A-Z]+(\\.[a-z0-9-A-Z]+)*(\\.[a-zA-Z]{2,3})$";
        public const string SiteAddress = "^[a-zA-Z0-9\\-\\.]+\\.[a-zA-Z]{2,3}$";
        public const string NationalCode = "^\\d{10}$";
    }
}
