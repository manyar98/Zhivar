namespace OMF.Common.Sms
{
    public interface ISmsProvider
    {
        SmsResponse SendSms(SmsData smsData);

        SmsResponse ReSendSms(string referenceId);
    }
}
