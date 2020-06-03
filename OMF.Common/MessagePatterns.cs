using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMF.Common
{
    public class MessagePatterns
    {
        public const string Mandatory = "{0} اجباری می باشد";
        public const string NotDefined = "{0} با مقدار '{1}' تعریف نشده است";
        public const string NotFound = "{0} با مقدار '{1}' وجود ندارد";
        public const string NotActive = "{0} با مقدار '{1}' غیرفعال است";
        public const string Successfull = "عملیات با موفقیت انجام شد";
        public const string Invalid = "{0} معتبر نمی باشد";
        public const string InvalidWithFormat = "{0} معتبر نمی باشد '{1}'";
        public const string DuplicateData = "{0} با مقدار '{1}' تکراری می باشد";
        public const string GreaterThan = "مقدار {0} می بایست بزرگتر از '{1}' باشد";
        public const string LessThan = "مقدار {0} می بایست کوچکتر از '{1}' باشد";
        public const string GreaterThanOrEqual = "مقدار {0} می بایست بزرگتر یا مساوی '{1}' باشد";
        public const string LessThanOrEqual = "مقدار {0} می بایست کوچکتر یا مساوی '{1}' باشد";
        public const string ExclusiveBetween = "مقدار {0} می بایست از '{1}' تا '{2}' باشد";
        public const string ExclusiveBetweenString = "مقدار {0} می بایست از '{1}' تا '{2}' کاراکتر باشد";
        public const string InvalidLength = "طول مقدار وارد شده معتبر نمی باشد";
        public const string InvalidLength2 = "طول مقدار {0} معتبر نمی باشد";
    }
}
