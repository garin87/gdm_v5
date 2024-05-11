using gdm5._0.Models;
using System.Collections.Generic;
using System.Security.Claims;


namespace gdm5._0.Services.Interfaces
{
    public interface ITokenService
    {
        TokenApiDTO RefreshToken(TokenApiDTO tokenApi);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
     //   List<Claim> GetClaims(User user);
        void RevokeRefreshToken(string username);

    }
}
