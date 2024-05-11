using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using gdm5._0.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gdm5._0.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AuthService(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public TokenApiDTO verifyUser(User userData)
        {
            var user = this._context.Users.FirstOrDefault(u => (u.UserName == userData.UserName) &&
                                                               (u.Password == userData.Password));

            if (user == null)
                throw new ApplicationException("User " + userData.UserName + " does not exist");

            var claims = GetClaims(user);
            var accessToken = this._tokenService.GenerateAccessToken(claims);
            var refreshToken = this._tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(21);
            this._context.SaveChanges();

            var roleName = this._context.Roles.FirstOrDefault(el => el.Id == user.RoleId)?.Name;
            if (roleName is null)
                roleName = "User";

            return new TokenApiDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserRole = roleName,
                UserId = userData.Id
            };
        }

        private List<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
                {
                      new Claim("UserName", user.UserName),
                      new Claim("Role", "Admin"), // by default 
                      new Claim(ClaimsConstants.UserId, user.Id.ToString()),
                };

            return claims;
        }
    }
}
