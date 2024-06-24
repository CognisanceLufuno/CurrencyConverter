using CurrencyConverterApi.App_Start;
using CurrencyConverterApi.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConverterDomain(builder.Configuration);
builder.Services
    .AddMvc(opt => opt.Filters.Add(typeof(TransactionalDBFilter)));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var assemblyName = typeof(Program).Assembly.GetName().Name;
var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");
builder.Services.AddSwaggerGen(x =>
{
    x.IncludeXmlComments(xmlPath, true);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
    options => { options.EnableTryItOutByDefault(); }
    );
}

app.UseAuthorization();

app.MapControllers();

app.Run();
