namespace Bamboo_card_currency_convertor.Models.Response
{
    public class ExchangeRateResponse
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
        public decimal? ConvertedAmount { get; set; }
    }
}
