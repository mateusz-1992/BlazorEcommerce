using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared
{
    public class UserPwResetModel
    {
        public string Email { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        [Required, StringLength(100, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
        [Compare("NewPassword", ErrorMessage = "The password do not match!!")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
