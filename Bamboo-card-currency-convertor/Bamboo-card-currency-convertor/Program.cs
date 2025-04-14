using AspNetCoreRateLimit;
using Bamboo_card_currency_convertor.Factory;
using Bamboo_card_currency_convertor.Factory.Interface;
using Bamboo_card_currency_convertor.Provider;
using Bamboo_card_currency_convertor.Services;
using Bamboo_card_currency_convertor.Services.Interface;
using Bamboo_card_currency_convertor.Utilities.Policies;

// Program.cs (partial setup)
var builder = WebApplication.CreateBuilder(args);
builder.AddSerilogLogging();
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwtAuth();

builder.Services.AddResilienceAndCaching();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddOpenTelemetryTracing(builder.Configuration);
builder.Services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<FrankfurterCurrencyProvider>();
builder.Services.AddScoped<ITokenService, TokenService>();


builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Use Swagger to generate API docs
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Converter API v1");
    }); // Enable Swagger UI at /swagger
}
app.UseIpRateLimiting();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLogging();
app.MapControllers();
app.Run();