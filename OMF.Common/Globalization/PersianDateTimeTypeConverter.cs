using System;
using System.ComponentModel;
using System.Globalization;

namespace OMF.Common.Globalization
{
    public class PersianDateTimeTypeConverter : TypeConverter
    {
        public string Format;

        public PersianDateTimeTypeConverter()
        {
            this.Format = "yyyy/MM/dd";
        }

        public PersianDateTimeTypeConverter(string format)
        {
            this.Format = format;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(typeof(DateTime)) || destinationType.Equals(typeof(string)))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destinationType)
        {
            if (value != null && value is PersianDateTime)
            {
                if (destinationType.Equals(typeof(DateTime)))
                    return (object)(DateTime)((PersianDateTime)value);
                if (destinationType.Equals(typeof(string)))
                    return (object)((PersianDateTime)value).ToString(this.Format);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.Equals(typeof(DateTime)) || sourceType.Equals(typeof(string)))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(
          ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            if (value is DateTime)
                return (object)new PersianDateTime((DateTime)value);
            if (value is string)
                return (object)PersianDateTime.Parse(value.ToString());
            return base.ConvertFrom(context, culture, value);
        }
    }
}
