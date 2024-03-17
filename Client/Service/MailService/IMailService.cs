namespace BlazorEcommerce.Client.Service.MailService
{
    public interface IMailService
    {
        Task<ServiceResponse<bool>> SendEmail(SendMail request);
    }
}
