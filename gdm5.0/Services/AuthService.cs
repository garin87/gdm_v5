using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var claims = this._tokenService.GetClaims(user);
            var accessToken = this._tokenService.GenerateAccessToken(claims);
            var refreshToken = this._tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            this._context.SaveChanges();

            var roleName = this._context.Roles.FirstOrDefault(el => el.Id == user.RoleId)?.Name;

           
            return new TokenApiDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserRole = roleName
            };
        }
    }
}
