using gdm5._0.Models;

namespace gdm5._0.Services.Interfaces
{
    public interface IAuthService
    {
        TokenApiDTO verifyUser(User userData);
    }
}
