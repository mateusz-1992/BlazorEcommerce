namespace BlazorEcommerce.Client.Service.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegister request);
        Task<ServiceResponse<string>> Login(UserLogin request);
        Task<ServiceResponse<bool>> ChangePassword (UserChangePassword request);
        Task<bool> IsUserAuthenticated();
        Task<ServiceResponse<bool>> ResetPassword(UserPwResetModel request);
        Task<ServiceResponse<string>> CreateResetToken(User request);
    }
}
