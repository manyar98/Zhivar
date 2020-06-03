using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OMF.Common.Serialization
{
    public class CustomSerializer
    {
        public byte[] Serialize(object obj)
        {
            List<string> stringList = new List<string>()
      {
        "ObjectState",
        "ActionsToLog",
        "PropertyMaps",
        "EntityImportItemList",
        "LogData",
        "Password",
        "PlainPassword"
      };
            PropertyInfo[] properties = obj.GetType().GetProperties();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!((IEnumerable<ParameterInfo>)propertyInfo.GetIndexParameters()).Any<ParameterInfo>() && !stringList.Contains(propertyInfo.Name) && propertyInfo.CanRead)
                {
                    object obj1 = propertyInfo.GetValue(obj);
                    stringBuilder.AppendFormat(string.Format("{0}:;:{1},,;,,", (object)EntityConfigManager.GetPropertyPersianTitle(obj.GetType(), propertyInfo.Name), obj1 == null ? (object)"NULL" : (object)obj1.ToString().Replace('{', '(').Replace('}', ')')));
                }
            }
            return Encoding.Unicode.GetBytes(stringBuilder.ToString());
        }

        public byte[] Serialize(Dictionary<string, string> dictionary)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                stringBuilder.AppendFormat(string.Format("{0}:;:{1},,;,,", (object)EntityConfigManager.GetPropertyPersianTitle(dictionary.GetType(), keyValuePair.Key), keyValuePair.Value == null ? (object)"" : (object)keyValuePair.Value.Replace('{', '(').Replace('}', ')')));
            return Encoding.Unicode.GetBytes(stringBuilder.ToString());
        }

        public List<Tuple<string, string>> Deserialize(byte[] byteArray)
        {
            string[] strArray1 = Encoding.Unicode.GetString(byteArray).Split(new string[1]
            {
        ",,;,,"
            }, StringSplitOptions.RemoveEmptyEntries);
            List<Tuple<string, string>> tupleList = new List<Tuple<string, string>>();
            foreach (string str in strArray1)
            {
                string[] separator = new string[1] { ":;:" };
                string[] strArray2 = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (strArray2.Length == 2)
                    tupleList.Add(new Tuple<string, string>(strArray2[0], strArray2[1]));
            }
            return tupleList;
        }
    }
}
