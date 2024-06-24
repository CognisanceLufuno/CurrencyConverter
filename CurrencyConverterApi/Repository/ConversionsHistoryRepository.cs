using CurrencyConverterApi.Models;
using CurrencyConverterApi.Repository.Interfaces;
using CurrencyConverterApi.Repository.Persistence;

namespace CurrencyConverterApi.Repository
{    
    public class ConversionsHistoryRepository : PersistenceRepository<ConversionsHistory>, IConversionsHistoryRepository
    {
        public ConversionsHistoryRepository(DBContext context) : base(context)
        {

        }
    }
}
