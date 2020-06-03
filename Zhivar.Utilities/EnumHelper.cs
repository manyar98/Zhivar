using Zhivar.DomainClasses.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Zhivar.DomainClasses;

namespace Zhivar.Utilities
{
    public static class EnumHelper
    {
        public static TAttribute GetAttribute<TEnum, TAttribute>(object value) where TEnum : struct, IConvertible
        {
            return typeof(TEnum).GetField(EnumHelper.GetEnum<TEnum>(value).ToString()).GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault<TAttribute>();
        }

        public static string GetPersianTitle(Type enumType, object enumValue)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            PersianTitleAttribute persianTitleAttribute = ((IEnumerable<FieldInfo>)enumType.GetFields()).FirstOrDefault<FieldInfo>((Func<FieldInfo, bool>)(fInfo => fInfo.Name == enumValue.ToString())).GetCustomAttributes(false).OfType<PersianTitleAttribute>().FirstOrDefault<PersianTitleAttribute>();
            return persianTitleAttribute == null ? enumValue.ToString() : persianTitleAttribute.Title;
        }

        public static string GetPersianTitle<T>(object enumValue) where T : struct, IConvertible
        {
            return EnumHelper.GetPersianTitle(typeof(T), enumValue);
        }

        public static IEnumerable<string> GetValues<T>(bool isPersian = true, bool justBrowsable = true) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            if (!justBrowsable)
            {
                foreach (object obj in Enum.GetValues(typeof(T)))
                {
                    object item = obj;
                    yield return isPersian ? EnumHelper.GetPersianTitle<T>(item) : item.ToString();
                    item = (object)null;
                }
            }
            else
            {
                foreach (object obj in Enum.GetValues(typeof(T)))
                {
                    object item = obj;
                    FieldInfo finfo = typeof(T).GetField(item.ToString());
                    BrowsableAttribute attrib = finfo.GetCustomAttributes(false).OfType<BrowsableAttribute>().FirstOrDefault<BrowsableAttribute>();
                    if (attrib == null)
                        yield return isPersian ? EnumHelper.GetPersianTitle<T>(item) : item.ToString();
                    else if (attrib.Browsable)
                        yield return isPersian ? EnumHelper.GetPersianTitle<T>(item) : item.ToString();
                    finfo = (FieldInfo)null;
                    attrib = (BrowsableAttribute)null;
                    item = (object)null;
                }
            }
        }

        public static IEnumerable<KeyValuePair<int, string>> GetKeyValues<T>(bool isPersian = true, bool justBrowsable = true) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            if (!justBrowsable)
            {
                foreach (object obj in Enum.GetValues(typeof(T)))
                {
                    object item = obj;
                    yield return isPersian ? new KeyValuePair<int, string>((int)item, EnumHelper.GetPersianTitle<T>(item)) : new KeyValuePair<int, string>((int)item, item.ToString());
                    item = (object)null;
                }
            }
            else
            {
                foreach (object obj in Enum.GetValues(typeof(T)))
                {
                    object item = obj;
                    FieldInfo finfo = ((IEnumerable<FieldInfo>)typeof(T).GetFields()).FirstOrDefault<FieldInfo>((Func<FieldInfo, bool>)(fInfo => fInfo.Name == item.ToString()));
                    BrowsableAttribute attrib = finfo.GetCustomAttributes(false).OfType<BrowsableAttribute>().FirstOrDefault<BrowsableAttribute>();
                    if (attrib == null)
                        yield return isPersian ? new KeyValuePair<int, string>((int)item, EnumHelper.GetPersianTitle<T>(item)) : new KeyValuePair<int, string>((int)item, item.ToString());
                    else if (attrib.Browsable)
                        yield return isPersian ? new KeyValuePair<int, string>((int)item, EnumHelper.GetPersianTitle<T>(item)) : new KeyValuePair<int, string>((int)item, item.ToString());
                    finfo = (FieldInfo)null;
                    attrib = (BrowsableAttribute)null;
                }
            }
        }
        //public static IEnumerable<KeyValuePair<Enums.HasShakhsInDatabase, string>> GetNameValues<T>(bool isPersian = true, bool justBrowsable = true) where T : struct, IConvertible
        //{
        //    if (!typeof(T).IsEnum)
        //        throw new ArgumentException("T must be an enumerated type");
        //    if (!justBrowsable)
        //    {
        //        foreach (object obj in Enum.GetValues(typeof(T)))
        //        {
        //            object item = obj;
        //            yield return isPersian ? new KeyValuePair<Enums.HasShakhsInDatabase, string>(item.ToString(), EnumHelper.GetPersianTitle<T>(item)) : new KeyValuePair<Enums.HasShakhsInDatabase, string>(item, item.ToString());
        //            item = (object)null;
        //        }
        //    }
        //    else
        //    {
        //        foreach (object obj in Enum.GetValues(typeof(T)))
        //        {
        //            object item = obj;
        //            FieldInfo finfo = ((IEnumerable<FieldInfo>)typeof(T).GetFields()).FirstOrDefault<FieldInfo>((Func<FieldInfo, bool>)(fInfo => fInfo.Name == item.ToString()));
        //            BrowsableAttribute attrib = finfo.GetCustomAttributes(false).OfType<BrowsableAttribute>().FirstOrDefault<BrowsableAttribute>();
        //            if (attrib == null)
        //                yield return isPersian ? new KeyValuePair<Enums.HasShakhsInDatabase, string>(item.ToString(), EnumHelper.GetPersianTitle<T>(item)) : new KeyValuePair<Enums.HasShakhsInDatabase, string>(item.ToString(), item.ToString());
        //            else if (attrib.Browsable)
        //                yield return isPersian ? new KeyValuePair<Enums.HasShakhsInDatabase, string>(item.ToString(), EnumHelper.GetPersianTitle<T>(item)) : new KeyValuePair<Enums.HasShakhsInDatabase, string>(item.ToString(), item.ToString());
        //            finfo = (FieldInfo)null;
        //            attrib = (BrowsableAttribute)null;
        //        }
        //    }
        //}

        public static TEnum GetEnum<TEnum>(object value) where TEnum : struct, IConvertible
        {
            return (TEnum)EnumHelper.GetEnum(typeof(TEnum), value);
        }

        public static object GetEnum(Type enumType, object value)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("TEnum must be an enumerated type");
            string name = value.ToString();
            int result;
            if (int.TryParse(name, out result))
                name = Enum.GetName(enumType, (object)result);
            return Enum.Parse(enumType, name);
        }
    }
}
