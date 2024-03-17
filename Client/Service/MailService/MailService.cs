
namespace BlazorEcommerce.Client.Service.MailService
{
    public class MailService : IMailService
    {
        private readonly HttpClient _httpClient;

        public MailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ServiceResponse<bool>> SendEmail(SendMail request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/mail/sendMail", request);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
    }
}
