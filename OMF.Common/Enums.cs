using System.ComponentModel;
using System;
using OMF.Common.Attributes;

namespace OMF.Common
{
    public class Enums
    {
        //public enum ZhivarUserType
        //{
        //    [PersianTitle("سیستمی")]
        //    SystemUser = 1,

        //    [PersianTitle("کاربر اپلیکیشن")]
        //    MobileAppUser = 2,

        //    [PersianTitle("کاربر مرکز درمانی")]
        //    MarkazDarmaniUser = 3,

        //    [PersianTitle("پزشک")]
        //    PezeshkUser = 4,

        //    [PersianTitle("مشاور")]
        //    Moshaver = 5,

        //    [PersianTitle("سازمان")]
        //    Organization = 9,

        //    [PersianTitle("برنامه نویس")]
        //    [Browsable(false)]
        //    Developers = 10
        //}

        public enum ObjectState
        {
            Unchanged,
            Added,
            Modified,
            Deleted,
            Detached,
        }

        public enum ExceptionType
        {
            [PersianTitle("عمومی")] General,
            [PersianTitle("لایه کسب و کار")] Business,
            [PersianTitle("لایه اتصال به داده")] DataAccess,
            [PersianTitle("پیکر بندی")] Configuration,
            [PersianTitle("مدیریت خطاها")] ExceptionManagement,
            [PersianTitle("توکن اشتباه")] InvalidToken,
            [PersianTitle("ورود به سیستم")] Login,
            [PersianTitle("ثبت رویداد")] LogManagement,
            [Browsable(false)] ShouldImplemented,
            [PersianTitle("محدودیت دسترسی")] PermissionException,
            [PersianTitle("استخراج به فایل")] Export,
            [PersianTitle("ورود اطلاعات در اکسل")] ImportExcel,
            [PersianTitle("وب سرویس")] WebService,
            [PersianTitle("ویندوز سرویس")] WinService,
            [PersianTitle("فراخوانی وب سرویس")] CallWebService,
            [PersianTitle("خطا در فرایند")] Workflow,
            [PersianTitle("پرداخت الکترونیک")] OnlinePayment,
        }

        public enum ConfigurationSourceType
        {
            ConfigFile,
            DataBase,
        }
   

        public enum Gender
        {
            [PersianTitle("آقا")] Male = 1,
            [PersianTitle("خانم")] Female = 2,
        }

        public enum ActionType
        {
            [PersianTitle("بازیابی رکورد")] ReadRecord = 1,
            [PersianTitle("درج رکورد")] AddData = 2,
            [PersianTitle("به روز رسانی")] EditData = 3,
            [PersianTitle("حذف")] DeleteData = 4,
            [PersianTitle("جستجو")] SearchData = 5,
            [PersianTitle("ورود به سیستم")] Login = 6,
            [PersianTitle("خروج از سیستم")] Logout = 7,
            [PersianTitle("ورود ناموفق")] LoginFailed = 8,
            [PersianTitle("تغییر رمز عبور")] ChangePassword = 9,
        }
        public enum AppLanguage
        {
            [PersianTitle("فارسی")] Farsi,
            [PersianTitle("انگلیسی")] English,
        }

        [Flags]
        public enum ActionLog
        {
            Read = 1,
            Insert = 2,
            Update = 4,
            Delete = 8,
            Search = 16, // 0x00000010
            All = 32, // 0x00000020
        }

        public enum CaptchaFormat
        {
            Alphabetic,
            Numeric,
            AlphaNumeric,
        }
        public enum OperationType
        {
            [PersianTitle("موجودیت")] Entity = 1,
            [PersianTitle("منوی اصلی")] MainMenu = 2,
            [PersianTitle("منوی فرعی"), Browsable(false)] SubMenu = 3,
            [PersianTitle("سایر")] Other = 10, // 0x0000000A
        }

        public enum UnitChartType
        {
            [PersianTitle("الگو")] Template = 1,
            [PersianTitle("انحصاری")] Exclusive = 2,
        }

        public enum ForgotPasswordMode
        {
            Mobile,
            Email,
        }

        public enum SmsReceiverType
        {
            [PersianTitle("نامشخص")] Unknown,
            [PersianTitle("مشتری")] Customer,
            [PersianTitle("کاربر سیستم")] User,
        }

        public enum OTPSendStatus
        {
            [PersianTitle("موفق")] Succeed,
            [PersianTitle("ناموفق")] Failed,
        }

        public enum SmsResponseStatus
        {
            [PersianTitle("موفق")] Succeed,
            [PersianTitle("ناموفق")] Failed,
        }

        public enum OperationAccessType
        {
            Insert,
            View,
            Update,
            Delete,
            Export,
            Print,
            Import,
        }

        public enum CompressionType
        {
            Other,
            Compress,
            Deflate,
            GZip,
            Identity,
            Any,
        }
        public enum CompressionLevel
        {
            [PersianTitle("معمولی")] Normal,
            [PersianTitle("بالا")] High,
            [PersianTitle("پایین")] Low,
        }
        public enum ViewStateStorageBehavior
        {
            FirstLoad,
            EachLoad,
        }

        public enum ViewStateStorageMethod
        {
            Default,
            File,
            IsolatedStorage,
            Session,
        }

 
    }
}