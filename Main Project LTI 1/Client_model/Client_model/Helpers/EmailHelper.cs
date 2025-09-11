using System;
using System.Configuration;
using System.Net.Mail;

namespace Client_model.Helpers
{
    public static class EmailHelper
    {
        public static bool IsEmailEnabled =>
            bool.TryParse(ConfigurationManager.AppSettings["EnableEmail"], out var v) && v;

        // Sends HTML email using system.net/mailSettings/smtp configured in Web.config.
        // Throws if email is disabled so callers may fallback and show codes to user.
        public static void SendEmail(string to, string subject, string body)
        {
            if (!IsEmailEnabled)
                throw new InvalidOperationException("Email disabled in configuration.");

            var section = ConfigurationManager.GetSection("system.net/mailSettings/smtp")
                          as System.Net.Configuration.SmtpSection;
            var from = section?.From ?? throw new InvalidOperationException("SMTP 'from' not configured.");

            using (var msg = new MailMessage(from, to))
            {
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    client.Timeout = 20000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true; // Gmail requires TLS
                    client.Send(msg);
                }
            }
        }
    }
}
