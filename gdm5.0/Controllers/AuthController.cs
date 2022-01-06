using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;


namespace gdm5._0.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IAuthService _authService;
       
        public AuthController(DataContext context, IAuthService authService, ITokenService tokenService,
                             IRegistrationService registrationService)
        {
            this._registrationService = registrationService;
            this._authService = authService;
        }

        // GET api/values
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] User userData)
        {
            if (userData == null)
                return BadRequest(new { Message = "Invalid client request" });
          
            try
            {
                TokenApiDTO result = this._authService.verifyUser(userData);

                
                //var cookieOptions = new CookieOptions() {
                //    HttpOnly = true,
                //    SameSite = SameSiteMode.Strict,
                //    Expires = DateTimeOffset.Now.AddMinutes(5)
                //};

                //Response.Cookies.Append("X-Access-Token", result.AccessToken);
                //Response.Cookies.Append("X-Refresh-Token", result.RefreshToken);
               // Response.Cookies.Append("X-User-role", result.UserRole, cookieOptions);



                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
           
        }

    

        [HttpPost("Registration")]
        public IActionResult RegisterUser([FromBody] UserDTO userData)
        {
            if (userData == null || !ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Incorrect user data" });

            try{
                var result = this._registrationService.CreateUser(userData);
                return Ok(new { IsSuccess = true, Message = "Success:" + userData.UserName + " registered"});
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message});
            }

        }

        [HttpGet, Route("logOut")]
        public IActionResult LogOut()
        {
            try
            {
                Response.Cookies.Delete("X-User-role");

                return Ok(new { IsSuccess = true });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }
    }
}
