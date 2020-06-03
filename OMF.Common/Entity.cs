using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using OMF.Common.Extensions;
using static OMF.Common.Enums;

namespace OMF.Common
{
    public class Entity : ViewEntity, IEntity, IViewEntity, IObjectState, ICloneable, IEquatable<Entity>
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public object this[string propName]
        {
            get
            {
                return this.GetType().GetProperty(propName).GetValue((object)this, (object[])null);
            }
            set
            {
                PropertyInfo property = this.GetType().GetProperty(propName);
                if (value is string && property.PropertyType.IsEnum)
                {
                    int result = 0;
                    if (int.TryParse(value as string, out result))
                    {
                        property.SetValue((object)this, (object)result, (object[])null);
                    }
                    else
                    {
                        object obj = Enum.Parse(property.PropertyType, value as string);
                        property.SetValue((object)this, obj, (object[])null);
                    }
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                    if (value is string && underlyingType.IsEnum)
                    {
                        string str = value as string;
                        int result = 0;
                        if (int.TryParse(value as string, out result))
                            str = Enum.GetName(underlyingType, (object)result);
                        object obj = Enum.Parse(underlyingType, str);
                        property.SetValue((object)this, Convert.ChangeType(obj, underlyingType, (IFormatProvider)CultureInfo.InvariantCulture), (object[])null);
                    }
                    else if (value is string)
                        property.SetValue((object)this, string.IsNullOrWhiteSpace(value as string) ? (object)null : Convert.ChangeType(value, underlyingType, (IFormatProvider)CultureInfo.InvariantCulture), (object[])null);
                    else
                        property.SetValue((object)this, value == null ? (object)null : Convert.ChangeType(value, underlyingType, (IFormatProvider)CultureInfo.InvariantCulture), (object[])null);
                }
                else if (value is string)
                    property.SetValue((object)this, string.IsNullOrWhiteSpace(value as string) ? (object)null : Convert.ChangeType(value, property.PropertyType, (IFormatProvider)CultureInfo.InvariantCulture), (object[])null);
                else
                    property.SetValue((object)this, value == null ? (object)null : Convert.ChangeType(value, property.PropertyType, (IFormatProvider)CultureInfo.InvariantCulture), (object[])null);
            }
        }

        public int ID { get; set; }

        public override object[] GetID()
        {
            return new object[1] { (object)this.ID };
        }

        public override void SetID(params object[] keyValues)
        {
            this.ID = Convert.ToInt32(keyValues[0]);
        }

        public virtual bool Equals(Entity other)
        {
            return EntityEqualityComparer.Equals<Entity>(other, this);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> propertySelector)
        {
            return propertySelector.GetPropertyInfo<T>().Name;
        }
    }
}
