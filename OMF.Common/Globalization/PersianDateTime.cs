using OMF.Common.Extensions;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace OMF.Common.Globalization
{
    [TypeConverter(typeof(PersianDateTimeTypeConverter))]
    public class PersianDateTime
    {
        public static string[] MonthNames = new string[12]
        {
      "فروردين",
      "ارديبهشت",
      "خرداد",
      "تير",
      "مرداد",
      "شهريور",
      "مهر",
      "آبان",
      "آذر",
      "دي",
      "بهمن",
      "اسفند"
        };
        public static string[] AbbreviatedMonthNames = new string[12]
        {
      "فروردين",
      "ارديبهشت",
      "خرداد",
      "تير",
      "مرداد",
      "شهريور",
      "مهر",
      "آبان",
      "آذر",
      "دي",
      "بهمن",
      "اسفند"
        };
        public static string[] DayNames = new string[7]
        {
      "شنبه",
      "يكشنبه",
      "دوشنبه",
      "سه\x200Cشنبه",
      "چهارشنبه",
      "پنجشنبه",
      "جمعه"
        };
        public static string[] AbbreviatedDayNames = new string[7]
        {
      "ش",
      "ي",
      "د",
      "س",
      "چ",
      "پ",
      "ج"
        };
        private DateTime date;
        private int Y;
        private int M;
        private int D;
        private long DOW;

        public PersianDateTime()
          : this(DateTime.Now)
        {
        }

        public PersianDateTime(DateTime date)
        {
            this.Date = date;
        }

        public PersianDateTime(int year, int month, int day)
        {
            this.Date = PersianDateTimeConverter.FromPersianDate(year, month, day);
        }

        public PersianDateTime(string date)
        {
            this.Date = PersianDateTimeConverter.FromPersianDate(date);
        }

        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                this.date = value;
                this.SetParams();
            }
        }

        public int Day
        {
            get
            {
                return this.D;
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return (DayOfWeek)this.DOW;
            }
        }

        public int DayOfYear
        {
            get
            {
                long num = (long)(31 * (this.M - 1) + this.D);
                if (this.M > 7)
                    num -= (long)(this.M - 7);
                return (int)num;
            }
        }

        public int Hour
        {
            get
            {
                return this.date.Hour;
            }
        }

        public int Millisecond
        {
            get
            {
                return this.date.Millisecond;
            }
        }

        public int Minute
        {
            get
            {
                return this.date.Minute;
            }
        }

        public int Month
        {
            get
            {
                return this.M;
            }
        }

        public int Second
        {
            get
            {
                return this.date.Second;
            }
        }

        public long Ticks
        {
            get
            {
                return this.date.Ticks;
            }
        }

        public TimeSpan TimeOfDay
        {
            get
            {
                return this.date.TimeOfDay;
            }
        }

        public int Year
        {
            get
            {
                return this.Y;
            }
        }

        protected DateTimeFormatInfo DateTimeFormat
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            }
        }

        public PersianDateTime Add(TimeSpan time)
        {
            return (PersianDateTime)this.date.Add(time);
        }

        public PersianDateTime AddDays(double value)
        {
            return (PersianDateTime)this.date.AddDays(value);
        }

        public PersianDateTime AddHours(double value)
        {
            return (PersianDateTime)this.date.AddHours(value);
        }

        public PersianDateTime AddMilliseconds(double value)
        {
            return (PersianDateTime)this.date.AddMilliseconds(value);
        }

        public PersianDateTime AddMinutes(double value)
        {
            return (PersianDateTime)this.date.AddMinutes(value);
        }

        public PersianDateTime AddMonths(int Months)
        {
            int M = this.M + Months;
            int y = this.Y;
            while (M > 12)
            {
                M -= 12;
                ++y;
            }
            return (PersianDateTime)PersianDateTimeConverter.FromPersianDate(y, M, this.D);
        }

        public PersianDateTime AddSeconds(double value)
        {
            return (PersianDateTime)this.date.AddSeconds(value);
        }

        public PersianDateTime AddTicks(long value)
        {
            return (PersianDateTime)this.date.AddTicks(value);
        }

        public PersianDateTime AddYears(int value)
        {
            return (PersianDateTime)PersianDateTimeConverter.FromPersianDate(this.Y + value, this.M, this.D);
        }

        public int CompareTo(PersianDateTime value)
        {
            return this.date.CompareTo(value.date);
        }

        public string[] GetDateTimeFormats()
        {
            return this.date.GetDateTimeFormats();
        }

        public TypeCode GetTypeCode()
        {
            return this.date.GetTypeCode();
        }

        public bool IsDaylightSavingTime()
        {
            return this.date.IsDaylightSavingTime();
        }

        public TimeSpan Subtract(DateTime value)
        {
            return this.date.Subtract(value);
        }

        public DateTime Subtract(TimeSpan value)
        {
            return this.date.Subtract(value);
        }

        public long ToBinary()
        {
            return this.date.ToBinary();
        }

        public long ToFileTime()
        {
            return this.date.ToFileTime();
        }

        public long ToFileTimeUtc()
        {
            return this.date.ToFileTimeUtc();
        }

        public DateTime ToLocalTime()
        {
            return this.date.ToLocalTime();
        }

        public string ToLongDateString()
        {
            return this.ToString("D");
        }

        public string ToLongTimeString()
        {
            return this.ToString("T");
        }

        public double ToOADate()
        {
            return this.date.ToOADate();
        }

        public string ToShortDateString()
        {
            return this.ToString("d");
        }

        public string ToShortTimeString()
        {
            return this.ToString("t");
        }

        public override string ToString()
        {
            return this.ToString("yyyy/MM/dd", (IFormatProvider)this.DateTimeFormat);
        }

        public string ToString(IFormatProvider provider)
        {
            DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();
            return this.date.ToString("G", provider);
        }

        public string ToString(string format)
        {
            return this.ToString(format, (IFormatProvider)this.DateTimeFormat);
        }

        internal string ToStringOld(string format, IFormatProvider provider)
        {
            DateTimeFormatInfo dateTimeFormatInfo = provider as DateTimeFormatInfo ?? this.DateTimeFormat;
            StringBuilder stringBuilder1;
            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'D':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.LongDatePattern);
                        break;
                    case 'F':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.FullDateTimePattern);
                        break;
                    case 'G':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.ShortDatePattern);
                        stringBuilder1.Append(" ");
                        stringBuilder1.Append(this.DateTimeFormat.LongTimePattern);
                        break;
                    case 'M':
                    case 'm':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.MonthDayPattern);
                        break;
                    case 'R':
                    case 'r':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.RFC1123Pattern);
                        break;
                    case 'T':
                        stringBuilder1 = new StringBuilder(this.DateTimeFormat.LongTimePattern);
                        break;
                    case 'U':
                    case 'u':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.UniversalSortableDateTimePattern);
                        break;
                    case 'Y':
                    case 'y':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.YearMonthPattern);
                        break;
                    case 'd':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.ShortDatePattern);
                        break;
                    case 'f':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.LongDatePattern);
                        stringBuilder1.Append(" ");
                        stringBuilder1.Append(this.DateTimeFormat.ShortTimePattern);
                        break;
                    case 'g':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.ShortDatePattern);
                        stringBuilder1.Append(" ");
                        stringBuilder1.Append(this.DateTimeFormat.ShortTimePattern);
                        break;
                    case 's':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.SortableDateTimePattern);
                        break;
                    case 't':
                        stringBuilder1 = new StringBuilder(dateTimeFormatInfo.ShortTimePattern);
                        break;
                    default:
                        stringBuilder1 = new StringBuilder(format);
                        break;
                }
            }
            else
                stringBuilder1 = new StringBuilder(format);
            StringBuilder stringBuilder2 = stringBuilder1.Replace("yyyy", this.Y.ToString()).Replace("yy", (this.Y % 100).ToString("00")).Replace("y", (this.Y % 100).ToString()).Replace("MMMM", PersianDateTime.MonthNames[this.M - 1]).Replace("MM", this.M.ToString("00")).Replace("M", this.M.ToString()).Replace("dddd", PersianDateTime.DayNames[this.DOW - 1L]).Replace("dd", this.D.ToString("00")).Replace("ddd", PersianDateTime.AbbreviatedDayNames[this.DOW - 1L]).Replace("d", this.D.ToString()).Replace("HH", this.Hour.ToString("00")).Replace("H", this.Hour.ToString());
            int num = this.Hour % 12;
            if (num == 0)
                num = 12;
            StringBuilder stringBuilder3 = stringBuilder2.Replace("hh", num.ToString("00")).Replace("h", num.ToString()).Replace("mm", this.Minute.ToString("00")).Replace("m", this.Minute.ToString()).Replace("ss", this.Second.ToString("00")).Replace("s", this.Second.ToString());
            return (this.Hour <= 12 ? (object)stringBuilder3.Replace("tt", dateTimeFormatInfo.AMDesignator).Replace("t", dateTimeFormatInfo.AMDesignator.Substring(0, 1)) : (object)stringBuilder3.Replace("tt", dateTimeFormatInfo.PMDesignator).Replace("t", dateTimeFormatInfo.PMDesignator.Substring(0, 1))).ToString();
        }

        internal string ToStringReg(string format, IFormatProvider provider)
        {
            DateTimeFormatInfo dtfi = provider as DateTimeFormatInfo;
            if (dtfi == null)
                dtfi = this.DateTimeFormat;
            string input;
            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'D':
                        input = dtfi.LongDatePattern;
                        break;
                    case 'F':
                        input = dtfi.FullDateTimePattern;
                        break;
                    case 'G':
                        input = dtfi.ShortDatePattern + " " + this.DateTimeFormat.LongTimePattern;
                        break;
                    case 'M':
                    case 'm':
                        input = dtfi.MonthDayPattern;
                        break;
                    case 'R':
                    case 'r':
                        input = dtfi.RFC1123Pattern;
                        break;
                    case 'T':
                        input = this.DateTimeFormat.LongTimePattern;
                        break;
                    case 'U':
                    case 'u':
                        input = dtfi.UniversalSortableDateTimePattern;
                        break;
                    case 'Y':
                    case 'y':
                        input = dtfi.YearMonthPattern;
                        break;
                    case 'd':
                        input = dtfi.ShortDatePattern;
                        break;
                    case 'f':
                        input = dtfi.LongDatePattern + " " + this.DateTimeFormat.ShortTimePattern;
                        break;
                    case 'g':
                        input = dtfi.ShortDatePattern + " " + this.DateTimeFormat.ShortTimePattern;
                        break;
                    case 's':
                        input = dtfi.SortableDateTimePattern;
                        break;
                    case 't':
                        input = dtfi.ShortTimePattern;
                        break;
                    default:
                        input = format;
                        break;
                }
            }
            else
                input = format;
            return new Regex("(?<p>[y]+)|(?<p>[M]+)|(?<p>[d]+)", RegexOptions.Compiled).Replace(input, (MatchEvaluator)(m =>
            {
                switch (m.Value[0])
                {
                    case 'H':
                        if (m.Value.Length == 1)
                            return this.Hour.ToString();
                        return this.Hour.ToString("00");
                    case 'M':
                        switch (m.Value.Length)
                        {
                            case 1:
                                return this.M.ToString();
                            case 2:
                                return this.M.ToString("00");
                            default:
                                return PersianDateTime.MonthNames[this.M - 1];
                        }
                    case 'd':
                        switch (m.Value.Length)
                        {
                            case 1:
                                return this.D.ToString();
                            case 2:
                                return this.D.ToString("00");
                            case 3:
                                return PersianDateTime.AbbreviatedDayNames[this.DOW - 1L];
                            default:
                                return PersianDateTime.DayNames[this.DOW - 1L];
                        }
                    case 'h':
                        int num = this.Hour % 12;
                        if (num == 0)
                            num = 12;
                        if (m.Value.Length == 1)
                            return num.ToString();
                        return num.ToString("00");
                    case 'm':
                        if (m.Value.Length == 1)
                            return this.Minute.ToString();
                        return this.Minute.ToString("00");
                    case 's':
                        if (m.Value.Length == 1)
                            return this.Second.ToString();
                        return this.Second.ToString("00");
                    case 't':
                        if (m.Value.Length == 1)
                        {
                            if (this.Hour > 12)
                                return dtfi.PMDesignator.Substring(0, 1);
                            return dtfi.AMDesignator.Substring(0, 1);
                        }
                        if (this.Hour > 12)
                            return dtfi.PMDesignator;
                        return dtfi.AMDesignator;
                    case 'y':
                        switch (m.Value.Length)
                        {
                            case 1:
                                return (this.Y % 100).ToString();
                            case 2:
                                return (this.Y % 100).ToString("00");
                            default:
                                return this.Y.ToString();
                        }
                    default:
                        return "";
                }
            }));
        }

        public string ToString(string format, IFormatProvider provider)
        {
            DateTimeFormatInfo dateTimeFormatInfo = provider as DateTimeFormatInfo ?? this.DateTimeFormat;
            string str;
            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'D':
                        str = dateTimeFormatInfo.LongDatePattern;
                        break;
                    case 'F':
                        str = dateTimeFormatInfo.FullDateTimePattern;
                        break;
                    case 'G':
                        str = dateTimeFormatInfo.ShortDatePattern + " " + this.DateTimeFormat.LongTimePattern;
                        break;
                    case 'M':
                    case 'm':
                        str = dateTimeFormatInfo.MonthDayPattern;
                        break;
                    case 'R':
                    case 'r':
                        str = dateTimeFormatInfo.RFC1123Pattern;
                        break;
                    case 'T':
                        str = this.DateTimeFormat.LongTimePattern;
                        break;
                    case 'U':
                    case 'u':
                        str = dateTimeFormatInfo.UniversalSortableDateTimePattern;
                        break;
                    case 'Y':
                    case 'y':
                        str = dateTimeFormatInfo.YearMonthPattern;
                        break;
                    case 'd':
                        str = dateTimeFormatInfo.ShortDatePattern;
                        break;
                    case 'f':
                        str = dateTimeFormatInfo.LongDatePattern + " " + this.DateTimeFormat.ShortTimePattern;
                        break;
                    case 'g':
                        str = dateTimeFormatInfo.ShortDatePattern + " " + this.DateTimeFormat.ShortTimePattern;
                        break;
                    case 's':
                        str = dateTimeFormatInfo.SortableDateTimePattern;
                        break;
                    case 't':
                        str = dateTimeFormatInfo.ShortTimePattern;
                        break;
                    default:
                        str = format;
                        break;
                }
            }
            else
                str = format;
            StringBuilder stringBuilder = new StringBuilder();
            int index = 0;
            int length = str.Length;
            while (index < length)
            {
                int startIndex = index;
                char ch = str[index];
                ++index;
                while (index < length && (int)str[index] == (int)ch)
                    ++index;
                switch (ch)
                {
                    case 'F':
                        stringBuilder.Append(this.Date.ToString(str.Substring(startIndex, index - startIndex).ToLower()));
                        continue;
                    case 'H':
                        if (index - startIndex == 1)
                        {
                            stringBuilder.Append(this.Hour.ToString());
                            continue;
                        }
                        stringBuilder.Append(this.Hour.ToString("00"));
                        continue;
                    case 'M':
                        switch (index - startIndex)
                        {
                            case 1:
                                stringBuilder.Append(this.M.ToString());
                                continue;
                            case 2:
                                stringBuilder.Append(this.M.ToString("00"));
                                continue;
                            default:
                                stringBuilder.Append(PersianDateTime.MonthNames[this.M - 1]);
                                continue;
                        }
                    case 'd':
                        switch (index - startIndex)
                        {
                            case 1:
                                stringBuilder.Append(this.D.ToString());
                                continue;
                            case 2:
                                stringBuilder.Append(this.D.ToString("00"));
                                continue;
                            case 3:
                                stringBuilder.Append(PersianDateTime.AbbreviatedDayNames[this.DOW - 1L]);
                                continue;
                            default:
                                stringBuilder.Append(PersianDateTime.DayNames[this.DOW - 1L]);
                                continue;
                        }
                    case 'f':
                        stringBuilder.Append(this.Date.ToString(str.Substring(startIndex, index - startIndex)));
                        continue;
                    case 'h':
                        int num = this.Hour % 12;
                        if (num == 0)
                            num = 12;
                        if (index - startIndex == 1)
                        {
                            stringBuilder.Append(num.ToString());
                            continue;
                        }
                        stringBuilder.Append(num.ToString("00"));
                        continue;
                    case 'm':
                        if (index - startIndex == 1)
                        {
                            stringBuilder.Append(this.Minute.ToString());
                            continue;
                        }
                        stringBuilder.Append(this.Minute.ToString("00"));
                        continue;
                    case 's':
                        if (index - startIndex == 1)
                        {
                            stringBuilder.Append(this.Second.ToString());
                            continue;
                        }
                        stringBuilder.Append(this.Second.ToString("00"));
                        continue;
                    case 't':
                        if (index - startIndex == 1)
                        {
                            if (this.Hour > 12)
                            {
                                stringBuilder.Append(dateTimeFormatInfo.PMDesignator.Substring(0, 1));
                                continue;
                            }
                            stringBuilder.Append(dateTimeFormatInfo.AMDesignator.Substring(0, 1));
                            continue;
                        }
                        if (this.Hour > 12)
                        {
                            stringBuilder.Append(dateTimeFormatInfo.PMDesignator);
                            continue;
                        }
                        stringBuilder.Append(dateTimeFormatInfo.AMDesignator);
                        continue;
                    case 'y':
                        switch (index - startIndex)
                        {
                            case 1:
                                stringBuilder.Append((this.Y % 100).ToString());
                                continue;
                            case 2:
                                stringBuilder.Append((this.Y % 100).ToString("00"));
                                continue;
                            default:
                                stringBuilder.Append(this.Y.ToString());
                                continue;
                        }
                    default:
                        stringBuilder.Append(str.Substring(startIndex, index - startIndex));
                        continue;
                }
            }
            return stringBuilder.ToString();
        }

        public DateTime ToUniversalTime()
        {
            return this.date.ToUniversalTime();
        }

        public override int GetHashCode()
        {
            return this.date.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.date.Equals(obj);
        }

        private void SetParams()
        {
            PersianDateTimeConverter.PersianDateDetail(this.date, out this.Y, out this.M, out this.D, out this.DOW);
        }

        public static implicit operator PersianDateTime(DateTime date)
        {
            return new PersianDateTime(date);
        }

        public static implicit operator DateTime(PersianDateTime pd)
        {
            return pd.date;
        }

        public static implicit operator PersianDateTime(string s)
        {
            return new PersianDateTime(s);
        }

        public static bool operator ==(PersianDateTime pd1, PersianDateTime pd2)
        {
            return pd1.date.Equals(pd2.date);
        }

        public static bool operator !=(PersianDateTime pd1, PersianDateTime pd2)
        {
            return !pd1.date.Equals(pd2.date);
        }

        public static PersianDateTime Parse(string dateStr)
        {
            DateTime date;
            if (!PersianDateTime.TryParse(dateStr, out date))
                throw new FormatException("Invalid Date Format");
            return new PersianDateTime(date);
        }

        public static bool TryParse(string dateStr, out DateTime date)
        {
            date = DateTime.Now;
            int Y;
            int M;
            int D;
            int hour;
            int minute;
            int second;
            if (!PersianDateTimeConverter.PersianDateParse(dateStr, out Y, out M, out D, out hour, out minute, out second))
                return false;
            date = PersianDateTimeConverter.FromPersianDate(Y, M, D, hour, minute, second);
            return true;
        }

        public static PersianDateTime Now
        {
            get
            {
                return new PersianDateTime(DateTime.Now);
            }
        }

        public static PersianDateTime Today
        {
            get
            {
                return new PersianDateTime(DateTime.Today);
            }
        }

        public static bool IsLeapYear(int year)
        {
            return PersianDateTimeConverter.IsLeapYear((long)year);
        }

        public static int DaysInMonth
        {
            get
            {
                return new PersianCalendar().GetDaysInMonth(PersianDateTime.Today.Year, PersianDateTime.Today.Month);
            }
        }

        public static PersianDateTime FirstPersianDateOfMonth
        {
            get
            {
                return PersianDateTime.Now.AddDays((double)(-1 * (PersianDateTime.Now.Day - 1)));
            }
        }

        public static string FirstPersianDateOfMonthStr
        {
            get
            {
                return PersianDateTime.FirstPersianDateOfMonth.ToDateString();
            }
        }

        public static PersianDateTime LastPersianDateOfMonth
        {
            get
            {
                return new PersianDateTime(PersianDateTime.Today.Year, PersianDateTime.Today.Month, PersianDateTime.DaysInMonth);
            }
        }

        public static string LastPersianDateOfMonthStr
        {
            get
            {
                return PersianDateTime.LastPersianDateOfMonth.ToDateString();
            }
        }
    }
}
