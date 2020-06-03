using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.Security;
using OMF.Common.Sms;
using OMF.Security.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Security.OTP
{
    public class OTPCodeHandler : IOTPCodeHandlerAsync, IOTPCodeHandler
    {
        public string CreateCode(UserContext userContext)
        {
            string str = string.Empty;
            char[] charArray = "AB0CD1EF2GH3IJ4KL5MN6OP7QR8ST9UV0WX1YZ".ToCharArray();
            if (ConfigurationController.OTPCodeWithCharacter)
            {
                for (int Seed1 = 0; Seed1 < (ConfigurationController.OTPCodeLength + 1) / 2; ++Seed1)
                {
                    int Seed2 = new Random(Seed1).Next(0, 24);
                    str += (string)(object)((int)charArray[new Random(Seed2).Next(0, 25)] + (int)charArray[new Random(Seed2).Next(0, 25)]);
                }
            }
            else
            {
                using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
                {
                    byte[] data = new byte[4];
                    cryptoServiceProvider.GetBytes(data);
                    str = BitConverter.ToUInt32(data, 0).ToString().Substring(0, ConfigurationController.OTPCodeLength);
                }
            }
            return str.Substring(0, ConfigurationController.OTPCodeLength);
        }

        public async Task<string> CreateCodeAsync(UserContext userContext)
        {
            string code = string.Empty;
            char[] alpha = "AB0CD1EF2GH3IJ4KL5MN6OP7QR8ST9UV0WX1YZ".ToCharArray();
            if (ConfigurationController.OTPCodeWithCharacter)
            {
                for (int i = 0; i < (ConfigurationController.OTPCodeLength + 1) / 2; ++i)
                {
                    int seed = new Random(i).Next(0, 24);
                    code += (string)(object)((int)alpha[new Random(seed).Next(0, 25)] + (int)alpha[new Random(seed).Next(0, 25)]);
                }
            }
            else
            {
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    byte[] byteArray = new byte[4];
                    rng.GetBytes(byteArray);
                    code = BitConverter.ToUInt32(byteArray, 0).ToString().Substring(0, ConfigurationController.OTPCodeLength);
                    byteArray = (byte[])null;
                }
            }
            return code.Substring(0, ConfigurationController.OTPCodeLength);
        }

        public bool NeedOTP(UserContext userContext)
        {
            int num;
            if (ConfigurationController.OTPCodeEnable.HasValue)
            {
                bool? nullable = ConfigurationController.OTPCodeEnable;
                if (nullable.Value)
                {
                    nullable = userContext.NeedOTP;
                    if (nullable.HasValue)
                    {
                        nullable = userContext.NeedOTP;
                        num = nullable.Value ? 1 : 0;
                        goto label_5;
                    }
                }
            }
            num = 0;
            label_5:
            return num != 0;
        }

        public bool NeedOTP(string userName)
        {
            if (!ConfigurationController.OTPCodeEnable.HasValue || !ConfigurationController.OTPCodeEnable.Value)
                return false;
            using (SecurityDbContext securityDbContext = new SecurityDbContext())
            {
                bool? nullable = securityDbContext.Users.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.UserName == userName)).Select<UserInfo, bool?>((Expression<Func<UserInfo, bool?>>)(u => u.NeedOTP)).FirstOrDefault<bool?>();
                return ConfigurationController.OTPCodeEnable.HasValue && (ConfigurationController.OTPCodeEnable.Value && nullable.HasValue) && nullable.Value;
            }
        }

        public async Task<bool> NeedOTPAsync(UserContext userContext)
        {
            bool? nullable = ConfigurationController.OTPCodeEnable;
            int num;
            if (nullable.HasValue)
            {
                nullable = ConfigurationController.OTPCodeEnable;
                if (nullable.Value)
                {
                    nullable = userContext.NeedOTP;
                    if (nullable.HasValue)
                    {
                        nullable = userContext.NeedOTP;
                        num = nullable.Value ? 1 : 0;
                        goto label_5;
                    }
                }
            }
            num = 0;
            label_5:
            return num != 0;
        }

        public async Task<bool> NeedOTPAsync(string userName)
        {
            bool? nullable1 = ConfigurationController.OTPCodeEnable;
            int num1;
            if (nullable1.HasValue)
            {
                nullable1 = ConfigurationController.OTPCodeEnable;
                num1 = !nullable1.Value ? 1 : 0;
            }
            else
                num1 = 1;
            if (num1 != 0)
                return false;
            using (SecurityDbContext dbContext = new SecurityDbContext())
            {
                nullable1 = await dbContext.Users.Where<UserInfo>((Expression<Func<UserInfo, bool>>)(u => u.UserName == userName)).Select<UserInfo, bool?>((Expression<Func<UserInfo, bool?>>)(u => u.NeedOTP)).FirstOrDefaultAsync<bool?>();
                bool? nullable = nullable1;
                bool? userNeedOTP = nullable;
                nullable = new bool?();
                nullable1 = ConfigurationController.OTPCodeEnable;
                int num2;
                if (nullable1.HasValue)
                {
                    nullable1 = ConfigurationController.OTPCodeEnable;
                    if (nullable1.Value && userNeedOTP.HasValue)
                    {
                        num2 = userNeedOTP.Value ? 1 : 0;
                        goto label_11;
                    }
                }
                num2 = 0;
                label_11:
                return num2 != 0;
            }
        }

        public OTPSendResponse SendCode(OTPSendRequest request)
        {
            if (SmsManager.SendSms(new SmsData()
            {
                MessageBody = string.Format("کد فعال سازی روزانه: {0}", (object)request.Code),
                MobileNo = request.UserMobile,
                EntityID = request.UserId.ToString(),
                EntityName = "OTPSendRequest",
                RecieverID = new int?(request.UserId),
                SmsReceiverType = SmsReceiverType.User
            }).Status == SmsResponseStatus.Failed)
                return new OTPSendResponse()
                {
                    Status = OTPSendStatus.Failed
                };
            return new OTPSendResponse()
            {
                Status = OTPSendStatus.Succeed
            };
        }

        public async Task<OTPSendResponse> SendCodeAsync(OTPSendRequest request)
        {
            SmsData smsData = new SmsData()
            {
                MessageBody = string.Format("کد فعال سازی روزانه: {0}", (object)request.Code),
                MobileNo = request.UserMobile,
                EntityID = request.UserId.ToString(),
                EntityName = "OTPSendRequest",
                RecieverID = new int?(request.UserId),
                SmsReceiverType = SmsReceiverType.User
            };
            SmsResponse response = await SmsManager.SendSmsAsync(smsData);
            if (response.Status == SmsResponseStatus.Failed)
                return new OTPSendResponse()
                {
                    Status = OTPSendStatus.Failed
                };
            return new OTPSendResponse()
            {
                Status = OTPSendStatus.Succeed
            };
        }

        public bool VerifyCode(UserContext userContext, string code)
        {
            return userContext.OTPCode == code;
        }

        public async Task<bool> VerifyCodeAsync(UserContext userContext, string code)
        {
            return userContext.OTPCode == code;
        }
    }
}
