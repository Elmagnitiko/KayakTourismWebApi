using Microsoft.Extensions.Caching.Memory;

namespace KayakTourismWebApi.InterfacesNS
{
    public interface ITwoFactorAuthenticationService
    {
        public void SaveToken(string userId, string token);
        public string GetToken(string userId);
        public void InvalidateToken(string userId);
    }
}
