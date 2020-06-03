using System.Threading.Tasks;

namespace OMF.Common.Sms
{
    public interface ISmsProviderAsync : ISmsProvider
    {
        Task<SmsResponse> SendSmsAsync(SmsData smsData);

        Task<SmsResponse> ReSendSmsAsync(string referenceId);
    }
}
