using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyConverterApi.Models
{
    [Table("ConversionsHistory")]
    public class ConversionsHistory: HasId
    {
        [MaxLength(6)]
        public required string RateKey { get; set; }
        public required decimal ExchangeRate { get; set; }
        public DateTime CreateAt { get; set; }
    }
}