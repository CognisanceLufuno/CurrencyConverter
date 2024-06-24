namespace CurrencyConverterApi.DTOs
{
    public class ConversionDTO
    {
        public required string Currency { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateAt { get; set; }  = DateTime.Now;
        public long ElapsedMilliSeconds { get; set; }
    }
}
