using OkalaRecruitmentTask.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ICurrencyRatesService, ExchangeRatesService>();
builder.Services.AddScoped<ICryptoPriceService, CoinMarketCapService>();
builder.Services.AddScoped<ICryptoQuoteService, CryptoQuoteService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
