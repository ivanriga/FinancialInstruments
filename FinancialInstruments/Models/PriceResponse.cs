﻿namespace FinancialInstrumentsApi.Models
{
    public class PriceResponse
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
