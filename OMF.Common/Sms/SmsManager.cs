// Decompiled with JetBrains decompiler
// Type: BPJ.Common.Sms.SmsManager
// Assembly: BPJ.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 242F3906-1FA7-46B7-9A75-AC2D1F5E9F32
// Assembly location: D:\Projects\VezartBehdasht\VBSSolution\VBS.Web\bin\BPJ.Common.dll

using OMF.Common.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Common.Sms
{
    public class SmsManager
    {
        private static ISmsProviderAsync provider;

        private static ISmsProviderAsync SmsProvider
        {
            get
            {
                return SmsManager.provider;
            }
        }

        public static void InitiateSmsProvider(ISmsProviderAsync smsProvider)
        {
            SmsManager.provider = smsProvider;
        }

        public static SmsResponse SendSms(
          string mobileNo,
          string messageBody,
          string entityID,
          string entityName)
        {
            return SmsManager.SendSms(mobileNo, messageBody, entityID, entityName, SmsReceiverType.Unknown, new int?());
        }

        public static SmsResponse SendSms(
          string mobileNo,
          string messageBody,
          string entityID,
          string entityName,
          SmsReceiverType receiverType,
          int? receiverID)
        {
            return SmsManager.SendSms(new SmsData()
            {
                MessageBody = messageBody,
                MobileNo = mobileNo,
                EntityID = entityID,
                EntityName = entityName,
                RecieverID = receiverID,
                SmsReceiverType = receiverType
            });
        }

        public static SmsResponse SendSms(SmsData smsData)
        {
            if (string.IsNullOrWhiteSpace(smsData.MobileNo) || !new Regex("^9[0|1|2|3|4|9][0-9]{8}$").IsMatch(smsData.MobileNo))
                return new SmsResponse()
                {
                    Status = SmsResponseStatus.Failed,
                    Code = "-203",
                    Message = ConfigurationController.ApplicationLanguage == AppLanguage.Farsi ? "شماره همراه معتبر نیست" : "Mobile Number is invalid."
                };
            if (SmsManager.provider != null)
                return SmsManager.provider.SendSms(smsData);
            return new SmsResponse()
            {
                Status = SmsResponseStatus.Failed,
                Code = "-404",
                Message = ConfigurationController.ApplicationLanguage == AppLanguage.Farsi ? "فراهم کننده ارسال پیامک تعریف نشده است" : "No sms provider has been initialized."
            };
        }

        public static SmsResponse ReSendSms(string referenceId)
        {
            if (SmsManager.provider != null)
                return SmsManager.provider.ReSendSms(referenceId);
            return new SmsResponse()
            {
                Status = SmsResponseStatus.Failed,
                Code = "-404",
                Message = ConfigurationController.ApplicationLanguage == AppLanguage.Farsi ? "فراهم کننده ارسال پیامک تعریف نشده است" : "No sms provider has been initialized."
            };
        }

        public static async Task<SmsResponse> SendSmsAsync(
          string mobileNo,
          string messageBody,
          string entityID,
          string entityName)
        {
            SmsResponse smsResponse = await SmsManager.SendSmsAsync(mobileNo, messageBody, entityID, entityName, SmsReceiverType.Unknown, new int?());
            return smsResponse;
        }

        public static async Task<SmsResponse> SendSmsAsync(
          string mobileNo,
          string messageBody,
          string entityID,
          string entityName,
          SmsReceiverType receiverType,
          int? receiverID)
        {
            SmsData smsData = new SmsData()
            {
                MessageBody = messageBody,
                MobileNo = mobileNo,
                EntityID = entityID,
                EntityName = entityName,
                RecieverID = receiverID,
                SmsReceiverType = receiverType
            };
            SmsResponse smsResponse = await SmsManager.SendSmsAsync(smsData);
            return smsResponse;
        }

        public static async Task<SmsResponse> SendSmsAsync(SmsData smsData)
        {
            if (string.IsNullOrWhiteSpace(smsData.MobileNo) || !new Regex("^9[0|1|2|3|4|9][0-9]{8}$").IsMatch(smsData.MobileNo))
                return new SmsResponse()
                {
                    Status = SmsResponseStatus.Failed,
                    Code = "-203",
                    Message = ConfigurationController.ApplicationLanguage == AppLanguage.Farsi ? "شماره همراه معتبر نیست" : "Mobile Number is invalid."
                };
            if (SmsManager.SmsProvider != null)
            {
                SmsResponse smsResponse = await SmsManager.SmsProvider.SendSmsAsync(smsData);
                return smsResponse;
            }
            return new SmsResponse()
            {
                Status = SmsResponseStatus.Failed,
                Code = "-404",
                Message = ConfigurationController.ApplicationLanguage == AppLanguage.Farsi ? "فراهم کننده ارسال پیامک تعریف نشده است" : "No sms provider has been initialized."
            };
        }

        public static async Task<SmsResponse> ReSendSmsAsync(string referenceId)
        {
            if (SmsManager.SmsProvider != null)
            {
                SmsResponse smsResponse = await SmsManager.SmsProvider.ReSendSmsAsync(referenceId);
                return smsResponse;
            }
            return new SmsResponse()
            {
                Status = SmsResponseStatus.Failed,
                Code = "-404",
                Message = ConfigurationController.ApplicationLanguage == AppLanguage.Farsi ? "فراهم کننده ارسال پیامک تعریف نشده است" : "No sms provider has been initialized."
            };
        }
    }
}
