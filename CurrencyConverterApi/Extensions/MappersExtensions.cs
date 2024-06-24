using AutoMapper;
using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Models;

namespace CurrencyConverterApi.Extensions
{
    public class MappersExtensions : Profile
    {
        public MappersExtensions()
        {
            CreateMap<ConversionsHistory, ConversionsHistoryDTO>().ReverseMap();
        }
    }
}
