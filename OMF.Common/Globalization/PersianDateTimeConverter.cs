using System;
using System.Linq;

namespace OMF.Common.Globalization
{
    internal class PersianDateTimeConverter
    {
        public static void PersianDateDetail(
          DateTime TD,
          out int Y,
          out int M,
          out int D,
          out long DOW)
        {
            int num1 = 365;
            int num2 = num1 * 4 + 1;
            int num3 = num1 * 33 + 8;
            Y = 0;
            M = 0;
            D = 0;
            DOW = 0L;
            long num4 = (long)((int)Math.Floor(TD.ToOADate()) + 422504);
            if (num4 <= 0L)
                return;
            DOW = (num4 + 2L) % 7L + 1L;
            Y = 122;
            int num5 = (int)(num4 / (long)num3);
            D = (int)(num4 % (long)num3);
            Y += num5 * 33;
            if (D > 7 * num2 + num1)
            {
                ++Y;
                D -= num1;
            }
            int num6 = D / num2;
            D %= num2;
            Y += num6 * 4;
            int num7 = D / num1;
            D %= num1;
            if (num7 == 4)
            {
                --num7;
                D += num1;
            }
            Y += num7;
            if (D < 186)
            {
                M = D / 31;
                D %= 31;
                ++M;
                ++D;
            }
            else
            {
                D -= 186;
                M = D / 30;
                D %= 30;
                M += 7;
                ++D;
            }
        }

        public static bool IsLeapYear(long Y)
        {
            long num1 = (Y - 1276L) / 33L;
            long num2 = (Y - 1276L) % 33L;
            return num2 % 4L == 0L && num2 != 32L;
        }

        public static string PersianDateToString(DateTime date)
        {
            int Y;
            int M;
            int D;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(date, out Y, out M, out D, out DOW);
            string str1 = Y.ToString() + "/";
            if (M < 10)
                str1 += "0";
            string str2 = str1 + M.ToString() + "/";
            if (D < 10)
                str2 += "0";
            return str2 + D.ToString();
        }

        public static string PersianDateToRString(DateTime date)
        {
            int Y;
            int M;
            int D;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(date, out Y, out M, out D, out DOW);
            return D.ToString() + "/" + M.ToString() + "/" + Y.ToString();
        }

        public static DateTime FromPersianDate(string Datestr)
        {
            int Y;
            int M;
            int D;
            int hour;
            int minute;
            int second;
            if (!PersianDateTimeConverter.PersianDateParse(Datestr, out Y, out M, out D, out hour, out minute, out second))
                throw new InvalidPersianDateException();
            if (!PersianDateTimeConverter.IsValidPersianDate(Y, M, D))
                throw new InvalidPersianDateException();
            long num1 = (long)((Y - 1277) / 33);
            long num2 = (long)((Y - 1277) % 33) / 4L;
            if (num2 == 8L)
                --num2;
            long num3 = num2 + num1 * 8L + (long)((Y - 1279) * 365) + 80L + (long)((M - 1) * 31) + (long)D;
            if (M > 7)
                num3 -= (long)(M - 7);
            DateTime dateTime = DateTime.FromOADate((double)num3);
            dateTime = dateTime.AddHours((double)hour);
            dateTime = dateTime.AddMinutes((double)minute);
            dateTime = dateTime.AddSeconds((double)second);
            return dateTime;
        }

        public static DateTime FromPersianDate(
          int Y,
          int M,
          int D,
          int hour,
          int minute,
          int second)
        {
            DateTime dateTime = PersianDateTimeConverter.FromPersianDate(Y, M, D);
            dateTime = dateTime.AddHours((double)hour);
            dateTime = dateTime.AddMinutes((double)minute);
            dateTime = dateTime.AddSeconds((double)second);
            return dateTime;
        }

        public static DateTime FromPersianDate(int Y, int M, int D)
        {
            PersianDateTimeConverter.ValidatePersianDate(ref Y, ref M, ref D);
            long num1 = (long)((Y - 1277) / 33);
            long num2 = (long)((Y - 1277) % 33) / 4L;
            if (num2 == 8L)
                --num2;
            long num3 = num2 + num1 * 8L + (long)((Y - 1279) * 365) + 80L + (long)((M - 1) * 31) + (long)D;
            if (M > 7)
                num3 -= (long)(M - 7);
            return DateTime.FromOADate((double)num3);
        }

        public static void ValidatePersianDate(ref int Y, ref int M, ref int D)
        {
            if (Y < 0)
                Y = 1200;
            if (Y < 100)
                Y += 1300;
            while (M < 1)
            {
                M += 12;
                --Y;
            }
            while (M > 12)
            {
                M -= 12;
                ++Y;
            }
            if (D < 1)
                D = 1;
            else if (M < 7 && D > 31)
                D = 31;
            else if (M > 6 && M < 12 && D > 30)
                D = 30;
            else if (!PersianDateTimeConverter.IsLeapYear((long)Y) && M == 12 && D > 29)
            {
                D = 29;
            }
            else
            {
                if (!PersianDateTimeConverter.IsLeapYear((long)Y) || M != 12 || D <= 30)
                    return;
                D = 30;
            }
        }

        public static bool IsValidPersianDate(int Y, int M, int D)
        {
            return M >= 1 && M <= 12 && D >= 1 && (M >= 7 || D <= 31) && ((M <= 6 || D <= 30) && (PersianDateTimeConverter.IsLeapYear((long)Y) || M != 12 || D <= 29));
        }

        public static bool IsValidPersianDate(string datestr)
        {
            int Y;
            int M;
            int D;
            int hour;
            int minute;
            int second;
            if (!PersianDateTimeConverter.PersianDateParse(datestr, out Y, out M, out D, out hour, out minute, out second))
                return false;
            return PersianDateTimeConverter.IsValidPersianDate(Y, M, D);
        }

        public static long MonthFirstDay(int Y, int M)
        {
            int D = 1;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(PersianDateTimeConverter.FromPersianDate(Y, M, D), out Y, out M, out D, out DOW);
            return DOW;
        }

        public static int MonthLastDay(int Y, int M)
        {
            if (M < 7)
                return 31;
            return M < 12 || PersianDateTimeConverter.IsLeapYear((long)Y) ? 30 : 29;
        }

        public static long MonthLastDay(DateTime Dt)
        {
            int Y;
            int M;
            int D;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(Dt, out Y, out M, out D, out DOW);
            return (long)PersianDateTimeConverter.MonthLastDay(Y, M);
        }

        public static DateTime GetMonthFirstDay(DateTime dt)
        {
            int Y;
            int M;
            int D;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(dt, out Y, out M, out D, out DOW);
            return PersianDateTimeConverter.FromPersianDate(Y, M, 1, dt.Hour, dt.Minute, dt.Second);
        }

        public static DateTime GetMonthLastDay(DateTime dt)
        {
            int Y;
            int M;
            int D1;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(dt, out Y, out M, out D1, out DOW);
            int D2 = PersianDateTimeConverter.MonthLastDay(Y, M);
            return PersianDateTimeConverter.FromPersianDate(Y, M, D2, dt.Hour, dt.Minute, dt.Second);
        }

        public static long WeekNo(DateTime Dt)
        {
            int Y;
            int M;
            int D;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(Dt, out Y, out M, out D, out DOW);
            return PersianDateTimeConverter.WeekNo(Y, M, D);
        }

        public static long WeekNo(int Y, int M, int D)
        {
            long num1 = PersianDateTimeConverter.MonthFirstDay(Y, 1);
            long num2 = (long)((M - 1) * 31 + D);
            if (M > 7)
                num2 -= (long)(M - 7);
            long num3 = num2 - (8L - num1);
            long num4 = (num3 - 1L) / 7L + 1L;
            if (num1 < 5L)
                ++num4;
            if (M == 12 && num3 % 7L > 0L && (long)(PersianDateTimeConverter.MonthLastDay(Y, M) - D) + num3 % 7L < 4L)
                num4 = 1L;
            return num4;
        }

        public static DateTime AddMonths(DateTime dt, int MonthNo)
        {
            int Y;
            int M1;
            int D;
            long DOW;
            PersianDateTimeConverter.PersianDateDetail(dt, out Y, out M1, out D, out DOW);
            int M2 = M1 + MonthNo;
            return PersianDateTimeConverter.FromPersianDate(Y, M2, D, dt.Hour, dt.Minute, dt.Second);
        }

        public static bool PersianDateParse(
          string datestr,
          out int Y,
          out int M,
          out int D,
          out int hour,
          out int minute,
          out int second)
        {
            string[] strArray1;
            if (datestr.Contains<char>('-'))
                strArray1 = datestr.Split('-');
            else if (datestr.Contains<char>(' '))
                strArray1 = datestr.Split(' ');
            else
                strArray1 = new string[1] { datestr };
            string[] strArray2 = strArray1[0].Split('/');
            Y = 0;
            D = 0;
            M = 0;
            hour = 0;
            minute = 0;
            second = 0;
            if (strArray2.Length != 3)
                return false;
            try
            {
                Y = int.Parse(strArray2[0]);
                M = int.Parse(strArray2[1]);
                D = int.Parse(strArray2[2]);
                if (strArray1.Length > 1)
                {
                    string[] strArray3 = strArray1[1].Split(':');
                    if (strArray3.Length != 3)
                        return false;
                    hour = int.Parse(strArray3[0]);
                    minute = int.Parse(strArray3[1]);
                    second = int.Parse(strArray3[2]);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
