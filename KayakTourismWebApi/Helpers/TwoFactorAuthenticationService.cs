using KayakTourismWebApi.InterfacesNS;
using Microsoft.Extensions.Caching.Memory;

namespace KayakTourismWebApi.HelpersNS
{
    public class TwoFactorAuthenticationService : ITwoFactorAuthenticationService
    {
        private readonly IMemoryCache _cache;

        public TwoFactorAuthenticationService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SaveToken(string userId, string token)
        {
            _cache.Set(userId, token, TimeSpan.FromMinutes(7));
        }

        public string GetToken(string userId)
        {
            _cache.TryGetValue(userId, out string? token);
            return token;
        }

        public void InvalidateToken(string userId)
        {
            _cache.Remove(userId);
        }
    }
}
