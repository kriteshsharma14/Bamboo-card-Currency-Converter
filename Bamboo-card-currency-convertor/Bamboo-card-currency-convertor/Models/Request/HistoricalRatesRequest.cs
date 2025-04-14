namespace Bamboo_card_currency_convertor.Models.Request
{
    public class HistoricalRatesRequest
    {
        public string BaseCurrency { get; set; } = "EUR";
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
