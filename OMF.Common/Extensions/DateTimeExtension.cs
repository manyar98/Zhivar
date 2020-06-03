using OMF.Common.Globalization;
using System;
using System.Linq;

namespace OMF.Common.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToDateTimeString(this DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public static string ToDateString(this DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd");
        }

        public static string ToTimeString(this DateTime dt)
        {
            return dt.ToString("hh:mm:ss");
        }

        public static PersianDateTime ToPersianDateTime(this DateTime dateTime)
        {
            return new PersianDateTime(dateTime);
        }

        public static string ToDateTimeString(this PersianDateTime dt)
        {
            return dt.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public static string ToDateString(this PersianDateTime dt)
        {
            return dt.ToString("yyyy/MM/dd");
        }

        public static string ToTimeString(this PersianDateTime dt)
        {
            return dt.ToString("hh:mm:ss");
        }

        public static DateTime ToGregorianDateTime(this PersianDateTime persianDate)
        {
            return persianDate.Date;
        }

        public static DateTime ToDateTime(this string dateTime)
        {
            string[] strArray1 = (string[])null;
            if (dateTime.Contains<char>('-'))
                strArray1 = dateTime.Split('-');
            else if (dateTime.Contains<char>(' '))
                strArray1 = dateTime.Split(' ');
            string str1;
            string str2;
            if (strArray1 != null)
            {
                str1 = strArray1[0];
                str2 = strArray1[1];
            }
            else
            {
                str1 = dateTime;
                str2 = (string)null;
            }
            string[] strArray2 = str1.Split('/');
            int int32_1 = Convert.ToInt32(strArray2[0]);
            int int32_2 = Convert.ToInt32(strArray2[1]);
            int int32_3 = Convert.ToInt32(strArray2[2]);
            int hour = 0;
            int minute = 0;
            int second = 0;
            if (str2 != null)
            {
                string[] strArray3 = str2.Split(':');
                hour = Convert.ToInt32(strArray3[0]);
                minute = Convert.ToInt32(strArray3[1]);
                second = Convert.ToInt32(strArray3[2]);
            }
            return new DateTime(int32_1, int32_2, int32_3, hour, minute, second);
        }

        public static PersianDateTime ToPersianDateTime(this string dateTime)
        {
            return dateTime.ToDateTime().ToPersianDateTime();
        }

        public static DateTime ToGregorianDateTime(this string persianDate)
        {
            return new PersianDateTime(persianDate).Date;
        }
    }
}
