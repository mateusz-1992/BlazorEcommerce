namespace BlazorEcommerce.Server.Service.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<bool> UserExists(string email);
        Task<ServiceResponse<string>> Login(string email, string password);
        Task<ServiceResponse<bool>> ChangePassword(int userId, string newpassword);
        int GetUserId();
        string GetUserEmail();
        Task<User> GetUserByEmail(string email);
        Task<ServiceResponse<bool>> ResetPassword(string Email, string NewPassword, string ResetToken);
        Task<ServiceResponse<string>> CreateResetToken(User request);
    }
}
