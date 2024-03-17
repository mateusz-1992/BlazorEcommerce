namespace BlazorEcommerce.Server.Service.EmailService
{
    public interface IMailService
    {
        Task<ServiceResponse<bool>> SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
