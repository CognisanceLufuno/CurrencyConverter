using AutoMapper;
using CurrencyConverterApi.Repository;
using CurrencyConverterApi.Repository.Interfaces;
using CurrencyConverterApi.Repository.Persistence.Interfaces;
using CurrencyConverterApi.Repository.Persistence;
using CurrencyConverterApi.Services;
using CurrencyConverterApi.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using StackExchange.Redis;

namespace CurrencyConverterApi.Extensions
{
    public static class DomainExtensions
    {
        public static void ConverterDomain(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DBContext>(options =>
            {
                options.UseMySQL(configuration.GetSection("connectionString").Value).UseLazyLoadingProxies();
            });
            services.AddScoped(typeof(IPersistenceRepository<>), typeof(PersistenceRepository<>));
            services.AddScoped<IConversionsHistoryRepository, ConversionsHistoryRepository>();
            services.AddScoped<IConversionsService, ConversionsService>();
            services.AddScoped<IConversionsHistoryService, ConversionsHistoryService>();
            services.AddScoped<ICacheService, CacheService>();

            services.AddScoped<ICurrencyExchangeApiService, CurrencyExchangeApiService>();

            ConfigurationOptions option = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { "localhost" },
                ConnectTimeout = 30000,
                ResponseTimeout = 30000
            };
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(option));
            services.AddMemoryCache();
            services.AddHttpClient<CurrencyExchangeApiService>();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappersExtensions());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
