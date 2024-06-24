using CurrencyConverterApi.Models;
using CurrencyConverterApi.Repository.Persistence.Interfaces;

namespace CurrencyConverterApi.Repository.Interfaces
{
    public interface IConversionsHistoryRepository : IPersistenceRepository<ConversionsHistory>
    {
    }
}
