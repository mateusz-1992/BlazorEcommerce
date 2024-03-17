
using System.Net;
using System.Net.Mail;

namespace BlazorEcommerce.Server.Service.EmailService
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailConfig;

        public MailService(MailSettings mailConfig)
        {
            _mailConfig = mailConfig;
        }
        public async Task<ServiceResponse<bool>> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(_mailConfig.FromEmail);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = htmlBody;
            smtp.Port = _mailConfig.Port;
            smtp.Host = _mailConfig.Host;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_mailConfig.UserName, _mailConfig.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                await smtp.SendMailAsync(message);
                var res = new ServiceResponse<bool>();
                res.Success = true;
                res.Message = "Mail was send successfully.";
                return res;
            }
            catch (Exception ex)
            {
                var res = new ServiceResponse<bool>();
                res.Success = false;
                res.Message = ex.Message.ToString();
                return res;
            }
        }
    }
}
