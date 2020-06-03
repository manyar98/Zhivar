using System;
using System.Configuration;

namespace OMF.Common.Configuration
{
    internal class OTPCodeSettings : ConfigurationElement
    {
        [ConfigurationProperty("enable")]
        public bool? Enable
        {
            get
            {
                try
                {
                    if (this["enable"] == null)
                        return new bool?();
                    return new bool?(Convert.ToBoolean(this["enable"]));
                }
                catch
                {
                    return new bool?();
                }
            }
            set
            {
                this["enable"] = (object)value;
            }
        }

        [ConfigurationProperty("tryNo")]
        public short TryNo
        {
            get
            {
                try
                {
                    short int16 = Convert.ToInt16(this["tryNo"]);
                    return int16 == (short)0 ? (short)3 : int16;
                }
                catch
                {
                    return 3;
                }
            }
        }

        [ConfigurationProperty("length")]
        public int Length
        {
            get
            {
                try
                {
                    int int32 = Convert.ToInt32(this["length"]);
                    return int32 == 0 ? 6 : int32;
                }
                catch
                {
                    return 6;
                }
            }
        }

        [ConfigurationProperty("withCharacter")]
        public bool WithCharacter
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this["withCharacter"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        [ConfigurationProperty("codeExpireTime")]
        public TimeSpan? CodeExpireTime
        {
            get
            {
                try
                {
                    return new TimeSpan?(TimeSpan.Parse(this["codeExpireTime"].ToString()));
                }
                catch
                {
                    return new TimeSpan?();
                }
            }
        }
    }
}
