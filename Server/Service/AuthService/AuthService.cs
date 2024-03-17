
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BlazorEcommerce.Server.Service.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public String GetUserEmail() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user == null)
            {
                response.Success = false;
                response.Message = "Wrong password or email.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password or email.";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            if (await UserExists(user.Email))
            {
                return new ServiceResponse<int> 
                { 
                    Success = false,
                    Message = "User already exists." 
                };
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<int> { Data = user.Id, Message = "Registration successful!" };
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(user => user.Email.ToLower()
            .Equals(email.ToLower())))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computerHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computerHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
                
        }

        public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newpassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            CreatePasswordHash(newpassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true , Message = "Password has been changed."};
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task<ServiceResponse<bool>> ResetPassword(string Email, string NewPassword, string ResetToken)
        {
            var res = new ServiceResponse<bool>();
            if (!await _context.Users.AnyAsync(user => user.Email.ToLower().Equals(Email.ToLower())))
            {
                res.Success = false;
                res.Message = "User with email " + Email + " not found!";
                return res;
            }
            else if(!await _context.Users.AnyAsync(user => user.UserPwResetToken.ToLower().Equals(ResetToken.ToLower())))
            {
                res.Success = false;
                res.Message = "Wrong reset token!";
                return res;
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == Email);
            if(user == null)
            {
                res.Success = false;
                res.Message = "User with email " + Email + " not found!";
                return res;
            }

            CreatePasswordHash(NewPassword, out byte[] passwordHas, out byte[] passwordSalt);
            user.PasswordHash = passwordHas;
            user.PasswordSalt = passwordSalt;
            user.UserPwResetToken = string.Empty;

            await _context.SaveChangesAsync();
            res.Success = true;
            res.Message = "The password has been resetted successfully!";
            return res;
        }

        public async Task<ServiceResponse<string>> CreateResetToken(User request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            var res = new ServiceResponse<string>();
            if(user == null)
            {
                res.Success = false;
                res.Message = "No user found with email address" + request.Email;
                return res;
            }

            if(!string.IsNullOrEmpty(user.UserPwResetToken))
            {
                res.Success = false;
                res.Message = "There is a open reset request already! Please check your Inbox.";
                return res;
            }

            var tok = Guid.NewGuid();
            user.UserPwResetToken = tok.ToString();
            await _context.SaveChangesAsync();
            res.Data = tok.ToString();
            res.Success = true;
            return res;
        }
    }
}
