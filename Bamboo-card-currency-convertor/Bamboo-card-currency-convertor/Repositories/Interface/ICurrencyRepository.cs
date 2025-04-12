using Bamboo_card_currency_convertor.Models;

namespace Bamboo_card_currency_convertor.Repositories.Interface
{
    public interface ICurrencyRepository
    {
        Task<CurrencyConversionResponse> GetLatestConversion(string from, List<string> to);
    }
}
