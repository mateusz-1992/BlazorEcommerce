using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IAuthService _authService;

        public MailController(IMailService mailService, IAuthService authService)
        {
            _mailService = mailService;
            _authService = authService;
        }

        [HttpPost("sendMail")]
        public async Task<ActionResult<ServiceResponse<bool>>> SendEmail(SendMail request)
        {
            var res = await _mailService.SendEmailAsync(request.ToEmail, request.Subject, request.HTMLBody);
            if(!res.Success)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}
