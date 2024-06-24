using Microsoft.AspNetCore.Mvc.Filters;
using CurrencyConverterApi.Repository.Persistence;

namespace CurrencyConverterApi.App_Start
{
    public class TransactionalDBFilter : IAsyncActionFilter
    {
        private readonly DBContext _dbContext;

        public TransactionalDBFilter(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var result = await next();
            var requestMethod = context.HttpContext.Request.Method;
            if (requestMethod == "POST" ||
                requestMethod == "PUT" ||
                requestMethod == "PATCH")
            {
                if (result.Exception == null || result.ExceptionHandled)
                {
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}