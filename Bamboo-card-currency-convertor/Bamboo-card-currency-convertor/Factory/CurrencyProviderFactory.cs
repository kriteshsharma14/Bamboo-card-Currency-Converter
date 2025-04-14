using Bamboo_card_currency_convertor.Factory.Interface;
using Bamboo_card_currency_convertor.Provider;
using Bamboo_card_currency_convertor.Provider.Interface;

namespace Bamboo_card_currency_convertor.Factory
{
    public class CurrencyProviderFactory : ICurrencyProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrencyProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ICurrencyProvider GetProvider(string name)
        {
            return name.ToLower() switch
            {
                "frankfurter" => _serviceProvider.GetRequiredService<FrankfurterCurrencyProvider>(),
                _ => throw new ArgumentException("Unsupported provider")
            };
        }
    }
}
