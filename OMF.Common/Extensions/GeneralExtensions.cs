using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Configuration;

namespace OMF.Common.Extensions
{
    public static class GeneralExtensions
    {
        private static char[] _persianDigits = new char[10]
        {
      '۰',
      '۱',
      '۲',
      '۳',
      '۴',
      '۵',
      '۶',
      '۷',
      '۸',
      '۹'
        };

        public static TOutput ConvertTo<TOutput>(this object objValue)
        {
            object obj = objValue.ConvertTo(typeof(TOutput));
            if (obj == null)
                return default(TOutput);
            return (TOutput)obj;
        }

        public static object ConvertTo(this object objValue, Type type)
        {
            try
            {
                if (objValue == null)
                    return (object)null;
                object obj;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type underlyingType = Nullable.GetUnderlyingType(type);
                    obj = !underlyingType.IsEnum ? (!(underlyingType == typeof(DateTime)) ? (string.IsNullOrWhiteSpace(objValue.ToString()) ? (object)null : Convert.ChangeType(objValue, underlyingType, (IFormatProvider)CultureInfo.InvariantCulture)) : (object)Convert.ToDateTime(objValue)) : Convert.ChangeType(EnumHelper.GetEnum(underlyingType, objValue), underlyingType, (IFormatProvider)CultureInfo.InvariantCulture);
                }
                else
                    obj = !type.IsEnum ? (!(type == typeof(DateTime)) ? (string.IsNullOrWhiteSpace(objValue.ToString()) ? (object)null : Convert.ChangeType(objValue, type, (IFormatProvider)CultureInfo.InvariantCulture)) : (object)Convert.ToDateTime(objValue)) : EnumHelper.GetEnum(type, objValue);
                return obj;
            }
            catch (Exception ex)
            {
                throw new ConvertTypeException(ex.Message, ex);
            }
        }

        public static bool IsInvalidWebInput(this string str)
        {
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str) && Constants.InvalidWebInputs.Any<string>((Func<string, bool>)(invalidInput => str.Contains(invalidInput)));
        }

        public static void Guard(this object obj, string message)
        {
            if (obj == null)
                throw new ArgumentNullException(message);
        }

        public static void Guard(this string str, string message)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException(message);
        }

        public static MemberInfo GetMember(this LambdaExpression expression)
        {
            return GeneralExtensions.RemoveUnary(expression.Body)?.Member;
        }

        public static PropertyInfo GetPropertyInfo<T>(
          this Expression<Func<T, object>> expression)
        {
            MemberExpression memberExpression = GeneralExtensions.RemoveUnary(expression.Body);
            if (memberExpression == null)
                return (PropertyInfo)null;
            if (memberExpression.Member is PropertyInfo)
                return memberExpression.Member as PropertyInfo;
            throw new ArgumentException("Cannot pass null to GetPropertyInfo");
        }

        private static MemberExpression RemoveUnary(Expression toUnwrap)
        {
            if (toUnwrap is UnaryExpression)
                return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
            return toUnwrap as MemberExpression;
        }

        public static string GetPersianTitle(this Enum enumeration)
        {
            return EnumHelper.GetPersianTitle(enumeration.GetType(), (object)enumeration);
        }

        public static string ToPersianNumber(this string input)
        {
            if (input.Trim() == "")
                return "";
            char[] charArray = input.ToCharArray();
            for (int index = 0; index < charArray.Length; ++index)
            {
                if (charArray[index] <= '9' && charArray[index] >= '0')
                    charArray[index] = GeneralExtensions._persianDigits[(int)charArray[index] - 48];
            }
            return new string(charArray);
        }

        public static string ToEnglishNumber(this string input)
        {
            if (input.Trim() == "")
                return "";
            input = input.Replace("۰", "0");
            input = input.Replace("۱", "1");
            input = input.Replace("۲", "2");
            input = input.Replace("۳", "3");
            input = input.Replace("۴", "4");
            input = input.Replace("۵", "5");
            input = input.Replace("۶", "6");
            input = input.Replace("۷", "7");
            input = input.Replace("۸", "8");
            input = input.Replace("۹", "9");
            return input;
        }

        public static string ToP1(this string input)
        {
            if (input.Trim() == "")
                return "";
            return Encoding.GetEncoding("iso-8859-1").GetString(Encoding.GetEncoding(1256).GetBytes(input));
        }

        public static string ToUnicode1256(this string input)
        {
            if (input.Trim() == "")
                return "";
            return Encoding.GetEncoding(1256).GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(input));
        }

        public static byte[] ConvertToHexByteArray(this string s)
        {
            int length = s.Length;
            byte[] numArray = new byte[length / 2];
            for (int startIndex = 0; startIndex < length; startIndex += 2)
                numArray[startIndex / 2] = Convert.ToByte(s.Substring(startIndex, 2), 16);
            return numArray;
        }

        public static string ConvertToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static void AsUnicode1256<T>(this T input) where T : class
        {
            if (!ConfigurationController.P1Enabled)
                return;
            foreach (PropertyInfo property in input.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    if (property.Name.EndsWith("P1") && !((IEnumerable<ParameterInfo>)property.GetIndexParameters()).Any<ParameterInfo>())
                    {
                        string input1 = property.GetValue((object)input)?.ToString();
                        if (!string.IsNullOrEmpty(input1))
                            property.SetValue((object)input, (object)input1.ToUnicode1256());
                    }
                }
                else if (property.PropertyType.Name == typeof(List<>).Name)
                {
                    List<object> objectList = property.GetValue((object)input, (object[])null) as List<object>;
                    if (objectList != null)
                    {
                        foreach (object input1 in objectList)
                            input1.AsUnicode1256<object>();
                    }
                }
            }
        }

        public static void AsP1<T>(this T input) where T : class
        {
            if (!ConfigurationController.P1Enabled)
                return;
            foreach (PropertyInfo property in input.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    if (property.Name.EndsWith("P1") && !((IEnumerable<ParameterInfo>)property.GetIndexParameters()).Any<ParameterInfo>())
                    {
                        string input1 = property.GetValue((object)input)?.ToString();
                        if (!string.IsNullOrEmpty(input1))
                            property.SetValue((object)input, (object)input1.ToP1());
                    }
                }
                else if (property.PropertyType.Name == typeof(List<>).Name)
                {
                    List<object> objectList = property.GetValue((object)input, (object[])null) as List<object>;
                    if (objectList != null)
                    {
                        foreach (object input1 in objectList)
                            input1.AsP1<object>();
                    }
                }
            }
        }
    }
}
