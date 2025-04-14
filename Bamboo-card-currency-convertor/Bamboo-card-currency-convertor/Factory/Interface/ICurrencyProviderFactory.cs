using Bamboo_card_currency_convertor.Provider.Interface;

namespace Bamboo_card_currency_convertor.Factory.Interface
{
    public interface ICurrencyProviderFactory
    {
        ICurrencyProvider GetProvider(string name);
    }
}
