using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using gdm5._0.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace gdm5._0.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;
        private readonly DataContext _context;

        public TokenService(IConfiguration configuration, DataContext dataContext) {
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JwtSettings");
            _context = dataContext;
        }

        public TokenApiDTO RefreshToken(TokenApiDTO tokenApi)
        {
            string accessToken = tokenApi.AccessToken;
           string refreshToken = tokenApi.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            //var username = principal.Identity.Name;

            int currentUserId = GetUserId(principal.Claims);
            var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

            var roleName = this._context.Roles.FirstOrDefault(el => el.Id == user.RoleId)?.Name;

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new ApplicationException("Invalid client request");

            var newAccessToken = this.GenerateAccessToken(principal.Claims);
            var newRefreshToken = GenerateTokenString();
            
            user.RefreshToken = newRefreshToken;
            this._context.SaveChanges();

            return new TokenApiDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserRole = roleName
            };
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.GetSection("expiryInMinutes").Value)),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
                // issuer: _jwtSettings.GetSection("validIssuer").Value,
                //audience: _jwtSettings.GetSection("validAudience").Value,
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new ApplicationException("Invalid token");
            return principal;
        }


        private int GetUserId(IEnumerable<Claim> claims)
        {
            if (!claims.Any())
                return 0;

            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimsConstants.UserId);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        private string GetUserName(IEnumerable<Claim> claims)
        {
            var userNameClaim = claims.FirstOrDefault(c => c.Type == "UserName");
            return string.IsNullOrEmpty(userNameClaim.Value) ? "" : userNameClaim.Value;
        }

        private string GetUserRole(IEnumerable<Claim> claims)
        {
            var userNameClaim = claims.FirstOrDefault(c => c.Type == "Role");
            return string.IsNullOrEmpty(userNameClaim.Value) ? "" : userNameClaim.Value;
        }

        public void RevokeRefreshToken(string username)
        {
            var user = this._context.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null)
                throw new ApplicationException("" + username + " does not exist");

            user.RefreshToken = null;
            this._context.SaveChanges();
        }

        private static string GenerateTokenString()
        {
            return CreateRandomString(25) + Guid.NewGuid();
        }

        private static string CreateRandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var charsLastIndex = chars.Length - 1;
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(charsLastIndex)]).ToArray());
        }
    }

   
}
