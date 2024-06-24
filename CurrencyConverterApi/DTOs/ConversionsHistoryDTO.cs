namespace CurrencyConverterApi.DTOs
{
    public class ConversionsHistoryDTO
    {
        public int Id { get; set; }
        public required string RateKey { get; set; }
        public required decimal ExchangeRate { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
