using KayakTourismWebApi.ModelsNS;

namespace KayakTourismWebApi.InterfacesNS
{
    public interface ITokenService
    {
        string CreateToken(Customer customer);
    }
}
