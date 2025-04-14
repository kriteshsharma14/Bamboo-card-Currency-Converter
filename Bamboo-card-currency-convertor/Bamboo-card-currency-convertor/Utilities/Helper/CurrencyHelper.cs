namespace Bamboo_card_currency_convertor.Utilities.Helper
{
    public static class CurrencyHelper
    {
        public static bool IsRestricted(string currency)
        {
            return Constant.RestrictedCurrency.Contains(currency);
        }
    }
}
