using System.Collections.Generic;

namespace OMF.Common
{
    public class Constants
    {
        public static string NewLine = "<br />";
        public static List<string> InvalidWebInputs = new List<string>()
    {
      "--",
      "<",
      ">",
      "</",
      "'"
    };
        public const string EncryptionKey = "BPJCryptoHelperTest";
        public const string ConnectionStringName = "BPJ.App.ConstructionString";
        public const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";
        public const string DateFormat = "yyyy/MM/dd";
        public const string TimeFormat = "hh:mm:ss";
        public const string UserTokenSessionKey = "__CurrentUserToken__";
        public const string UserContextSessionKey = "__CurrentUserContext__";
        public const string CurrentOTPUserContext = "__CurrentOTPUserContext__";
    }
}
