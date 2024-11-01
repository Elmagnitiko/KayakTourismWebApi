using System.Security.Claims;

namespace KayakTourismWebApi.HelpersNS
{
    public static class ClaimsExtensions
    {
        public static string Getusername(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;
        }
    }
}
