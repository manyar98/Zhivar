namespace OMF.Common.Mail
{
    public class MailOptions
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string SecurePassword { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public int TimeOut { get; set; } = 100000;
    }
}
