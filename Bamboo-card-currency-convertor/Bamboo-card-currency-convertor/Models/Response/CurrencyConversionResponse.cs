﻿namespace Bamboo_card_currency_convertor.Models.Response
{
    public class CurrencyConversionResponse
    {
        public decimal Amount { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
