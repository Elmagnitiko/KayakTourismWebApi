using KayakData.ModelsNS;
using Microsoft.AspNetCore.Identity;

namespace KayakTourismWebApi.InterfacesNS
{
    public interface ITokenService
    {
        Task <string> CreateToken(Customer customer, UserManager<Customer> userManager);
    }
}
