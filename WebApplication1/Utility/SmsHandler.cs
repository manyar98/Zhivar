using Zhivar.DataLayer;
//sing SmsIrRestful;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Zhivar.Web.Utility
{
    public class SmsHandler
    {

        public string GetToken()
        {
            var secretKey = "it66)%#teBC!@&";
            var userApiKey = "a586e32caa7dfb2166e2d19f";
            // SmsIrRestful.Token tk = new SmsIrRestful.Token();
            // string result = tk.GetToken(userApiKey, secretKey);
            string result = "";
            return result;
        }

        public void verification()
        {
            var token = GetToken();

            //var restVerificationCode = new RestVerificationCode()
            //{
            //    Code = "1234567890",
            //    MobileNumber = "09186664327"
            //};

     //       var restVerificationCodeRespone = new VerificationCode().Send(token, restVerificationCode);

            //if (restVerificationCodeRespone.IsSuccessful)
            //{

            //}
            //else
            //{

            //}
        }

        public void Send()
        {
            var token = GetToken();
            var number = GetSmsLines();
            //var messageSendObject = new MessageSendObject()
            //{
            //    Messages = new List<string> { "تست" }.ToArray(),
            //    MobileNumbers = new List<string> { "09186350455" }.ToArray(),
            //    LineNumber = number.ToString(),
            //    SendDateTime = null,
            //    CanContinueInCaseOfError = true
            //};

          //  MessageSendResponseObject messageSendResponseObject = new MessageSend().Send(token, messageSendObject);

            //if (messageSendResponseObject.IsSuccessful)
            //{

            //}
            //else
            //{

            //}


        }

        public long GetSmsLines()
        {
            var token = GetToken();
            //SmsLineNumber credit = new SmsLine().GetSmsLines(token);

            //if (credit == null)
            //    throw new Exception($@"{nameof(credit) } is null");

            //if (credit.IsSuccessful)
            //{
            //    return credit.SMSLines.FirstOrDefault().LineNumber;
            //}
            //else
            //{
            //    return 0;
            //}

            return 0;
        }

        public bool Register(long mobile, string code)
        {
            var token = GetToken();
            //var ultraFastSend = new UltraFastSend()
            //{
            //    Mobile = mobile,
            //    TemplateId = 1224,
            //    ParameterArray = new List<UltraFastParameters>()
            //    {
            //        new UltraFastParameters()
            //        {
            //            Parameter = "VerificationCode" , ParameterValue = code,

            //        },

            //    }.ToArray()

            //};

            //UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            //if (ultraFastSendRespone.IsSuccessful)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }

    }

}