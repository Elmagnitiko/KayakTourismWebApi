using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.ModelsNS;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KayakTourismWebApi.TokenServiceNS
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }
        public string CreateToken(Customer customer)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, customer.Email),
                //new Claim(JwtRegisteredClaimNames.GivenName, customer.UserName),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(5),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); 
        }
    }
}
