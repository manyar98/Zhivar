using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Attributes;

namespace Zhivar.DomainClasses
{
    public class ZhivarEnums
    {
        public enum ZhivarUserType
        {
            [PersianTitle("سیستمی")]
            SystemUser = 1,

            [PersianTitle("کاربر اپلیکیشن")]
            MobileAppUser = 2,

            [PersianTitle("کاربر مرکز درمانی")]
            MarkazDarmaniUser = 3,

            [PersianTitle("پزشک")]
            PezeshkUser = 4,

            [PersianTitle("مشاور")]
            Moshaver = 5,

            [PersianTitle("سازمان")]
            Organization = 9,

            [PersianTitle("برنامه نویس")]
            [Browsable(false)]
            Developers = 10
        }
        public enum Jenseat
        {
            [PersianTitle("زن")]
            Zan = 1,
            [PersianTitle("مرد")]
            Maard = 2

        }

        public enum NoeShakhs
        {
            [PersianTitle("حقیقی")]
            Haghighi = 1,
            [PersianTitle("حقوقی")]
            Hoghoghi = 2,
            [PersianTitle("نامشخص")]
            NaMoshakhs = 3
        }

        public enum TypeHoghoghi
        {
            [PersianTitle("شرکت")]
            Sherkat = 1,
            [PersianTitle("اداره")]
            Edare = 2,
            [PersianTitle("کانون")]
            Kanon = 3
        }
        public enum ActionType
        {
            Add,
            Edit
        }

        public enum TypeRole
        {
            [PersianTitle("خصوصی")]
            Private = 1,
            [PersianTitle("عمومی")]
            Public = 2,
            [PersianTitle("محافظت شده")]
            Protected = 3
        }

        //public enum NoeEjare
        //{
        //    [PersianTitle("روزانه")]
        //    Rozane = 1,
        //    [PersianTitle("ماهانه")]
        //    Mahane = 2,

        //}
        public enum Vaziat
        {
            [PersianTitle("فعال")]
            Faal = 1,
            [PersianTitle("غیر فعال")]
            GhireFaal = 2

        }

        public enum NoeSemat
        {
            [PersianTitle("مدیر داخلی	")]
            InternalManager = 3,
            [PersianTitle("نصاب")]
            Nasab = 4,
            [PersianTitle("چاپخانه	")]
            ChapKhane = 5,
            [PersianTitle("طراح")]
            Designer = 6,
            [PersianTitle("بازاریاب")]
            Visitor = 7
        }
        public enum NoeKala
        {
            [PersianTitle("کالا")]
            Kala = 1,
            [PersianTitle("خدمات")]
            Khadmat = 2,

        }
        public enum HasShakhsInDatabase
        {
            [PersianTitle("تعریف شخص جدید")]
            No = 0,
            [PersianTitle("انتخاب از لیست اشخاص")]
            Yes = 1,
        }

        public enum NoeFactor
        {
            //[PersianTitle("فاکتور فروش")]
            //Forosh = 1,
            //[PersianTitle("فاکتور خرید")]
            //Kharid = 2,
            //[PersianTitle("برگشت از فروش")]
            //BargashtForosh = 3,
            //[PersianTitle("برگشت خرید")]
            //BargashtKharid = 4

            [PersianTitle("فاکتور فروش")]
            Sell = 0,
            [PersianTitle("فاکتور خرید")]
            Buy = 1,
            [PersianTitle("برگشت از فروش")]
            ReturnSell = 2,
            [PersianTitle("برگشت خرید")]
            ReturnBuy = 3,
            [PersianTitle("فاکتور اجاره")]
            RentTo = 5,
            [PersianTitle("فاکتور اجاره از طرف حساب")]
            RentFrom = 6,
        }
        public enum ContractType
        {
            [PersianTitle("قرارداد اجاره دادن")]
            RentTo = 0,
            [PersianTitle("قرارداد اجاره گرفتن")]
            RentFrom = 1,
            [PersianTitle("پیش قرارداد")]
            PreContract = 2,
        }
        public enum NoeResid
        {
            [PersianTitle(" دریافت")]
            Dariaft = 1,
            [PersianTitle("پرداخت ")]
            Pardakht = 2,

        }

        public enum PayRecevieType
        {
            [PersianTitle("شخص")]
            AzShakhs = 1,
            [PersianTitle("درآمد")]
            Daramd = 2,
            [PersianTitle("هزینه")]
            Hazine = 4,
            [PersianTitle("سایر")]
            Sir = 3,

        }

        public enum AccountType
        {
            [PersianTitle("گروه")]
            Gorohe = 1,
            [PersianTitle("کل")]
            Kol = 2,
            [PersianTitle("معین ")]
            Moen = 3,
            [PersianTitle("تفضیلی")]
            Tafzeli = 4,

        }
        public enum ResultCode
        {
            Successful = 0,
            ValidationError = 1,
            AccessDenied = 2,
            UserContextNull = 3,
            Exception = 4,
            NotFound = 5
        }

        //public enum InvoiceType
        //{
        //    [PersianTitle("فاکتور فروش")]
        //    Sell = 0,
        //    [PersianTitle("فاکتور خرید")]
        //    Buy = 1,
        //    [PersianTitle("برگشت از فروش")]
        //    ReturnSell = 2,
        //    [PersianTitle("برگشت خرید")]
        //    ReturnBuy = 3

        //}
        public enum NoeInsertFactor
        {
            [PersianTitle("ذخیره موقت")]
            Temporary = 0,
            [PersianTitle("منتظر تایید")]
            waitingForConfirmation = 1,
            [PersianTitle("منتظر دریافت")]
            WaitingToReceive = 2,
            [PersianTitle("دریافت شده")]
            Received = 3
        }

        public enum Status
        {
            [PersianTitle("ذخیره موقت")]
            Temporary = 0,
            [PersianTitle("ارسال پیش قرارداد")]
            SendPreContract = 1,
            [PersianTitle("تایید پیش قرارداد")]
            ConfirmationPreContract = 2,
            [PersianTitle("ارسال قرارداد")]
            SendContract = 3,
            [PersianTitle("تایید قرارداد")]
            ConfirmationContract = 4
        }

        public enum CostStatus
        {
            [PersianTitle("ذخیره موقت")]
            Temporary = 0,
            [PersianTitle("منتظر تایید")]
            waitingForConfirmation = 1,
            [PersianTitle("منتظر پرداخت")]
            WaitingToPay = 2,
            [PersianTitle("پرداخت شده")]
            Paid = 3
        }
        public enum ChequeStatus
        {
            [PersianTitle("عادی")]
            Normal = 0,
            [PersianTitle("در جریان وصول")]
            InProgress = 1,
            [PersianTitle("پاس شده")]
            Passed = 2,
            [PersianTitle("برگشت خورده")]
            ChequeNotPass = 3,
            [PersianTitle("عودت شده")]
            ChequeReturn = 4,
            [PersianTitle("خرج شده")]
            Sold = 5,
        }
        public enum ChequeType
        {
            [PersianTitle("دریافتنی")]
            Dareaftani = 0,
            [PersianTitle("پرداختنی")]
            Pardakhtani = 1,

        }

        public enum DocumentStatus
        {
            [PersianTitle("پیش نویس")]
            Peshnvis = 1,
            [PersianTitle("تایید شده")]
            TaeedShode = 2,
        }

        public enum ContactType
        {
            [PersianTitle("نامشخص")]
            NaMoshakhs = 0,
            [PersianTitle("حقیقی")]
            Haghighi = 1,
            [PersianTitle("حقوقی")]
            Hoghoghi = 2,
        }

        public enum systemAccountTypes
        {
            [PersianTitle("حساب های دریافتنی")]
            debtor = 0,
            [PersianTitle("حساب های پرداختنی")]
            creditors = 1,
            [PersianTitle("صندوق")]
            Cash = 2,
            [PersianTitle("بانک")]
            Bank = 3,
            [PersianTitle("تنخواه گردان")]
            fund = 4,
            [PersianTitle("اسناد دریافتنی")]
            receivable = 5,
            [PersianTitle("اسناد پرداختنی")]
            payable = 6,
            [PersianTitle("اسناد در جریان وصول")]
            inProgres = 7,
            [PersianTitle("موجودی کالا")]
            inventory = 8,
            [PersianTitle("فروش")]
            Sale = 9,
            [PersianTitle("خرید")]
            Buy = 10,
            [PersianTitle("برگشت از فروش")]
            SaleReturn = 11,
            [PersianTitle("برگشت از خرید")]
            BuyReturn = 12,
            [PersianTitle("مالیات بر ارزش افزوده")]
            VAT = 13,
            [PersianTitle("مالیات بر ارزش افزوده فروش")]
            ValueAddedSale = 14,
            [PersianTitle("مالیات بر ارزش افزوده خرید")]
            ValueAddedBuy = 15,
            [PersianTitle("مالیات بر درآمد")]
            incomeTax = 16,
            [PersianTitle("فروش خدمات")]
            SalesOfServices = 17,
            [PersianTitle("هزینه خدمات خریداری شده")]
            CostOfPurchasedServices = 18,
            [PersianTitle("سرمایه اولیه")]
            InitialInvestment = 19,
            [PersianTitle("افزایش یا کاهش سرمایه")]
            ReduceOrIncreaseCapital = 20,
            [PersianTitle("برداشت")]
            Removal = 21,
            [PersianTitle("سهم سود و زیان")]
            Share = 22,
            [PersianTitle("تخفیفات نقدی خرید")]
            BuyCashDiscounts = 23,
            [PersianTitle("تخفیفات نقدی فروش")]
            SaleCashDiscounts = 24,
            [PersianTitle("هزینه ضایعات کالا")]
            CostWasteGoods = 25,
            [PersianTitle("کنترل ضایعات کالا")]
            ControlWasteProducts = 26,
            [PersianTitle("حقوق")]
            Salary = 27,
            [PersianTitle("تراز افتتاحیه")]
            OpeningBalance = 28,
            [PersianTitle("تراز اختتامیه")]
            ClosureAlignment = 29,
            [PersianTitle("خلاصه سود و زیان")]
            SummaryProfitAndLoss = 30,
            [PersianTitle("سود انباشته")]
            AccumulatedProfit = 31,
        }
        public enum NoeItem
        {
            [PersianTitle("کالا")]
            Item = 0,
            [PersianTitle("خدمات")]
            Service = 1
        }

        public enum CalendarType
        {
            [PersianTitle("شمسی")]
            Persian = 0,
            [PersianTitle("میلادی")]
            Miladi = 1
        }

        public enum Currency
        {
            [PersianTitle("ریال")]
            Rial = 0,
            [PersianTitle("تومان")]
            Toman = 1,
            [PersianTitle("افغانی")]
            Afghani = 2,
            [PersianTitle("سامانی")]
            Samani =3,
            [PersianTitle("سوم")]
            Sevom = 4,
            [PersianTitle("دلار")]
            Dolar = 5,
            [PersianTitle("درهم")]
            Darham = 6,
            [PersianTitle("یورو")]
            Eouro = 7,
            [PersianTitle("یوان")]
            Yovan = 8,
            [PersianTitle("پوند")]
            Pound = 9,
            [PersianTitle("لیر")]
            Lir = 10,
            [PersianTitle("لاری")]
            Lari = 11,
            [PersianTitle("رینگیت")]
            Rengit = 12,
            [PersianTitle("ین")]
            ean = 13,
            [PersianTitle("روپیه")]
            Ropeh = 14,
            [PersianTitle("دینار")]
            Dinar = 15,
            [PersianTitle("وون")]
            Voon = 16
        }

        public enum BalanceType
        {
            [PersianTitle("بدهکار")]
            Debit = 0,
            [PersianTitle("بستانکار")]
            Credit = 1
        }
        public enum Gender
        {
            [PersianTitle("آقا")] Male = 1,
            [PersianTitle("خانم")] Female = 2,
        }

        public enum WorkFlowActionType
        {
            [PersianTitle("تایید")]
            Taeed = 2,
            [PersianTitle("عدم تایید")]
            AdameTaeed = 3,
            [PersianTitle("ارسال جهت بررسي")]
            ErsalJahateBaresi = 4,
            [PersianTitle("ارسال جهت ويرايش")]
            ErsalJahateVirayesh = 6,
            [PersianTitle(" عدم تایید و خاتمه")]
            AdameTaeedVaKhateme = 7,
            [PersianTitle("شروع فرایند")]
            Shoroe = 8,
            [PersianTitle("تایید و خاتمه")]
            TaeedVaKhateme = 9,
            [PersianTitle("تایید و تبدیل به  قرارداد")]
            TabdelPishghrardadBeGharardad = 10,


        }
        public enum MapItemSazeType
        {
            [PersianTitle("گروه")] Group = 1,
            [PersianTitle("زیر گروه")] SubGroup = 2,
        }

        public enum NoeMozd
        { 
            [PersianTitle("پروژه ای")] Project = 1,
            [PersianTitle("ماهانه")] Month = 2,
        }
        public enum ChapkhaneType
        {
            [PersianTitle("چاپخانه شرکت")] Sherkat = 1,
            [PersianTitle("چاپخانه دیگر")] Degar = 2,
        }

        public enum DetailPayReceiveType
        {
            [PersianTitle("صندوق")] Sandogh = 1,
            [PersianTitle("فیش بانکی")] Fish = 2, 
            [PersianTitle("چـک")] Cheque = 3,
            [PersianTitle("خرج چـک")] KharjCheque = 4,
        }

        public enum DocRequest
        {
            [PersianTitle("قرارداد/طراح")]
            ContractTarah = 1,

        }

        public enum TempleteType
        {
            [PersianTitle("هفته")]
            Week = 1,
            [PersianTitle("ماه")]
            Month = 2,
            [PersianTitle("سال")]
            Year = 3,

        }

        public enum ContractStopType
        {
            [PersianTitle("بدون هیچ واکنشی")]
            NoAction = 1,
            [PersianTitle("کسر هزینه مدت زمان توقف")]
            CostDeduction = 2,
            [PersianTitle("افزودن عین خاموشی به زمان اکران")]
            AddTime = 3,
            [PersianTitle(" افزودن به زمان اکران نسبت به ضریب")]
            RatioAddTime = 4,

        }

        public enum NoeDoc
        {
            [PersianTitle("سند افتتاحیه")]
            FirstDoc = 0,
            [PersianTitle("فاکتور فروش")]
            Sell = 1,
            [PersianTitle("فاکتور خرید")]
            Buy = 2,
            [PersianTitle("برگشت از فروش")]
            ReturnSell = 3,
            [PersianTitle("برگشت خرید")]
            ReturnBuy = 4,
            [PersianTitle("فاکتور اجاره")]
            RentTo = 5,
            [PersianTitle("فاکتور اجاره از طرف حساب")]
            RentFrom = 6,
            [PersianTitle("رسید دریافت")]
            Recive = 7,
            [PersianTitle("رسید پرداخت")]
            Pay = 8,
            [PersianTitle("هزینه")]
            Cost = 9,
            [PersianTitle("انتقال")]
            Transfer = 10,

        }
    }
}
