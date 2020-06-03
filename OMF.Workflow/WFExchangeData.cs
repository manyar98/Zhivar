using System;
using System.Collections.Generic;
using System.Text;

namespace OMF.Workflow
{
    [Serializable]
    public class WFExchangeData : Dictionary<string, string>
    {
        public override string ToString()
        {
            return (string)this;
        }

        public static explicit operator WFExchangeData(string exchangeDataStrValue)
        {
            WFExchangeData wfExchangeData = new WFExchangeData();
            if (exchangeDataStrValue == null)
                return wfExchangeData;
            string str1 = exchangeDataStrValue;
            string[] separator1 = new string[1] { ";;;" };
            foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] separator2 = new string[1] { ":::::" };
                string[] strArray = str2.Split(separator2, StringSplitOptions.None);
                if (strArray.Length == 2)
                    wfExchangeData.Add(strArray[0], strArray[1]);
            }
            return wfExchangeData;
        }

        public static explicit operator string(WFExchangeData exchangeData)
        {
            if (exchangeData == null)
                return "";
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>)exchangeData)
                stringBuilder.AppendFormat("{0}:::::{1};;;", (object)keyValuePair.Key, (object)keyValuePair.Value);
            return stringBuilder.ToString();
        }
    }
}
