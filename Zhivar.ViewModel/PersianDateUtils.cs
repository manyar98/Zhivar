using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel
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
            string sal, mah, roz;
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
    }
}
