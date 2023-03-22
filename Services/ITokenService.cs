using APICatalogoMinimal.Models;

namespace APICatalogoMinimal.Services;

public interface ITokenService
{
    string GerarToken(string key, string issuer, string audience, UserModel user);
}
