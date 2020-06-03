using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.Utilities
{
    public static class PersianDateUtils
    {
        public static string ToPersianDate(this DateTime date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, new GregorianCalendar());
            var persianCalendar = new PersianCalendar();
            return
                $"{persianCalendar.GetYear(dateTime)}/{persianCalendar.GetMonth(dateTime).ToString("00")}/{persianCalendar.GetDayOfMonth(dateTime).ToString("00")}";
        }
        public static string ToPersianDate(this DateTime? date2)
        {
            if (date2 == null)
                return string.Empty;

            DateTime date = Convert.ToDateTime(date2);

            var dateTime = new DateTime(date.Year, date.Month, date.Day, new GregorianCalendar());
            var persianCalendar = new PersianCalendar();
            return
                $"{persianCalendar.GetYear(dateTime)}/{persianCalendar.GetMonth(dateTime).ToString("00")}/{persianCalendar.GetDayOfMonth(dateTime).ToString("00")}";
        }
        public static string ToPersianDateTime(this DateTime date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, new GregorianCalendar());
            var persianCalendar = new PersianCalendar();
            return
                $"{persianCalendar.GetYear(dateTime)}/{persianCalendar.GetMonth(dateTime).ToString("00")}/{persianCalendar.GetDayOfMonth(dateTime).ToString("00")}";
        }

        public static DateTime ToDateTime(string dateShamsi)
        {
            dateShamsi = DateConverter(dateShamsi);
            string  sal, mah, roz;
            int intSal, intMah, intRoz;

            sal = dateShamsi.Substring(0, 4);
            mah = dateShamsi.Substring(5, 2);
            roz = dateShamsi.Substring(8, 2);

            intSal = Convert.ToInt32(sal);
            intMah = Convert.ToInt32(mah);
            intRoz = Convert.ToInt32(roz);

            PersianCalendar pc = new PersianCalendar();
            return pc.ToDateTime(intSal, intMah, intRoz, 0, 0, 0, 0);
        }


        private static string DateConverter(string str)
        {
            String[] split = str.Split('/');
            PersianCalendar PerCal = new PersianCalendar();

            string Year, Day, Month;
            Year = split[0];
            Month = split[1]; 
            Day = split[2]; 


            if (Day.Length == 1)
            {
                Day = Day.ToString().Insert(0, "0");
            }
            if (Month.Length == 1)
            {
                Month = Month.ToString().Insert(0, "0");
            }
            return Year + '/' + Month + '/' + Day;
        }

        public static TempDate ToTempPersianDate(this DateTime date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, new GregorianCalendar());
            var persianCalendar = new PersianCalendar();

            TempDate tempDate = new TempDate();

            tempDate.Day = persianCalendar.GetDayOfMonth(dateTime).ToString();
            tempDate.Month = persianCalendar.GetMonth(dateTime).ToString();
            tempDate.Year = persianCalendar.GetYear(dateTime).ToString();

 
            switch (persianCalendar.GetMonth(dateTime))
            {
                case 1:
                    {
                        tempDate.TitleMonth = "فروردین";
                        tempDate.IntMonth = 1;
                        break;
                    }
                case 2:
                    {
                        tempDate.TitleMonth = "اردیبهشت";
                        tempDate.IntMonth = 2;
                        break;
                    }
                case 3:
                    {
                        tempDate.TitleMonth = "خرداد";
                        tempDate.IntMonth = 3;
                        break;
                    }
                case 4:
                    {
                        tempDate.TitleMonth = "تیر";
                        tempDate.IntMonth = 4;
                        break;
                    }
                case 5:
                    {
                        tempDate.TitleMonth = "مرداد";
                        tempDate.IntMonth = 5;
                        break;
                    }
                case 6:
                    {
                        tempDate.TitleMonth = "شهریور";
                        tempDate.IntMonth = 6;
                        break;
                    }
                case 7:
                    {
                        tempDate.TitleMonth = "مهر";
                        tempDate.IntMonth = 7;
                        break;
                    }
                case 8:
                    {
                        tempDate.TitleMonth = "آبان";
                        tempDate.IntMonth = 8;
                        break;
                    }
                case 9:
                    {
                        tempDate.TitleMonth = "آذر";
                        tempDate.IntMonth = 9;
                        break;
                    }
                case 10:
                    {
                        tempDate.TitleMonth = "دی";
                        tempDate.IntMonth = 10;
                        break;
                    }
                case 11:
                    {
                        tempDate.TitleMonth = "بهمن";
                        tempDate.IntMonth = 11;
                        break;
                    }
                case 12:
                    {
                        tempDate.TitleMonth = "اسفند";
                        tempDate.IntMonth = 12;
                        break;
                    }
                default:
                    break;
            }

            switch (persianCalendar.GetDayOfWeek(dateTime))
            {
                case DayOfWeek.Sunday:
                    tempDate.TitleDay = "یکشنبه";
                    break;
                case DayOfWeek.Monday:
                    tempDate.TitleDay = "دوشنبه";
                    break;
                case DayOfWeek.Tuesday:
                    tempDate.TitleDay = "سه شنبه";
                    break;
                case DayOfWeek.Wednesday:
                    tempDate.TitleDay = "چهارشنبه";
                    break;
                case DayOfWeek.Thursday:
                    tempDate.TitleDay = "پنجشنبه";
                    break;
                case DayOfWeek.Friday:
                    tempDate.TitleDay = "جمعه";
                    break;
                case DayOfWeek.Saturday:
                    tempDate.TitleDay = "شنبه";
                    break;
                default:
                    break;
            }
            return tempDate;


        }


        public static StartEndMonth StartEndMonth(this DateTime date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, new GregorianCalendar());
            var persianCalendar = new PersianCalendar();

            StartEndMonth startEndMonth = new StartEndMonth();

            var Day = persianCalendar.GetDayOfMonth(dateTime).ToString();
            var Month = persianCalendar.GetMonth(dateTime).ToString();
            var Year = persianCalendar.GetYear(dateTime).ToString();

            var days = persianCalendar.GetDaysInMonth(persianCalendar.GetYear(date), persianCalendar.GetMonth(date), PersianCalendar.PersianEra);

            //var days = persianCalendar.GetDaysInMonth(persianCalendar.GetDayOfMonth(dateTime), persianCalendar.GetMonth(dateTime), PersianCalendar.PersianEra);

            var startStr = $"{persianCalendar.GetYear(dateTime)}/{persianCalendar.GetMonth(dateTime).ToString("00")}/01";
            var endStr = $"{persianCalendar.GetYear(dateTime)}/{persianCalendar.GetMonth(dateTime).ToString("00")}/{days.ToString("00")}";
            var startDate = ToDateTime(startStr);
            var endDate = ToDateTime(endStr);

            startEndMonth.StartDate = startDate;
            startEndMonth.EndDate = endDate;

            return startEndMonth;
        }
        public static StartEndMonth StartEndYear(this DateTime date)
        {
            var dateTime = new DateTime(date.Year, date.Month, date.Day, new GregorianCalendar());
            var persianCalendar = new PersianCalendar();

            StartEndMonth startEndMonth = new StartEndMonth();

            var Day = persianCalendar.GetDayOfMonth(dateTime).ToString();
            var Month = persianCalendar.GetMonth(dateTime).ToString();
            var Year = persianCalendar.GetYear(dateTime).ToString();

            var days = persianCalendar.GetDaysInMonth(date.Year, 12, PersianCalendar.PersianEra);

            var startStr = $"{persianCalendar.GetYear(dateTime)}/01/01";
            var endStr = $"{persianCalendar.GetYear(dateTime)}/12/{days.ToString("00")}";
            var startDate = ToDateTime(startStr);
            var endDate = ToDateTime(endStr);

            startEndMonth.StartDate = startDate;
            startEndMonth.EndDate = endDate;

            return startEndMonth;
        }

        public static string NameOfMonth(this int month)
        {


            switch (month)
            {
                case 1:
                    {
                        return("فروردین");
                    }
                case 2:
                    {
                        return "اردیبهشت";
                    }
                case 3:
                    {
                        return "خرداد";
                    }
                case 4:
                    {
                        return "تیر";
                    }
                case 5:
                    {
                        return "مرداد";
                    }
                case 6:
                    {
                        return "شهریور";
                    }
                case 7:
                    {
                        return "مهر";
                    }
                case 8:
                    {
                        return "آبان";
                    }
                case 9:
                    {
                        return "آذر";
                    }
                case 10:
                    {
                        return "دی";
                    }
                case 11:
                    {
                        return "بهمن";
                    }
                case 12:
                    {
                        return "اسفند";
                    }
                default:
                    return string.Empty;
            }

        }

    }

    public class TempDate
    {
        public string Day { get; set; }
        public string TitleDay { get; set; }
        public string Month { get; set; }
        public string TitleMonth { get; set; }
        public int IntMonth { get; set; }
        public string Year { get; set; }
    }

    public class StartEndMonth
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
