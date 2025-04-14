using Bamboo_card_currency_convertor.Models;

namespace Bamboo_card_currency_convertor.Services.Interface
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
