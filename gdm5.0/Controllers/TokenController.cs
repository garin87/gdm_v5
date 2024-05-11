using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace gdm5._0.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService tokenService)
        {
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiDTO tokenApi)
        {
            if (tokenApi is null)
                return BadRequest(new { Message = "Invalid client request" });

            try
            {
                var result = this._tokenService.RefreshToken(tokenApi);

                //Response.Cookies.Append("X-Access-Token", result.AccessToken);
                //Response.Cookies.Append("X-Refresh-Token", result.RefreshToken);
   
                return new ObjectResult(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [HttpPost, Authorize]
        [Route("revokeToken")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            try
            {
                this._tokenService.RevokeRefreshToken(username);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
            
        }
    }
}
