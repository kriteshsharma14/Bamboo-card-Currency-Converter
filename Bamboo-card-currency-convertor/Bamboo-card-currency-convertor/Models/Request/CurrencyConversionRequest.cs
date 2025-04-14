namespace Bamboo_card_currency_convertor.Models.Request
{
    public class CurrencyConversionRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string Provider { get; set; } = "Frankfurter";
    }
}
