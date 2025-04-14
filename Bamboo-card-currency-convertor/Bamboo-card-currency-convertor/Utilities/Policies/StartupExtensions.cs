using AspNetCoreRateLimit;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Text;


namespace Bamboo_card_currency_convertor.Utilities.Policies
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddResilienceAndCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient<FrankfurterCurrencyProvider>();
            return services;
        }
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
                options.AddPolicy("UserOnly", policy => policy.RequireRole(Roles.User));
                options.AddPolicy("ManagerOnly", policy => policy.RequireRole(Roles.Manager));
            });

            return services;
        }

        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            return services;
        }

        public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, lc) => lc
           .ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .Enrich.WithProperty("Application", "CurrencyConversionApi")
           .WriteTo.Console());
            return builder;
        }

        public static IServiceCollection AddOpenTelemetryTracing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                .WithTracing(builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri(configuration["OpenTelemetry:OtlpEndpoint"]!);
                    })
                );

            return services;
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RequestLogger");
                var stopwatch = Stopwatch.StartNew();

                var correlationId = context.TraceIdentifier;
                context.Response.Headers["X-Correlation-ID"] = correlationId;
                using (LogContext.PushProperty("CorrelationId", correlationId))
                {
                    await next.Invoke();

                    stopwatch.Stop();
                    var userId = context.User?.FindFirst("clientId")?.Value ?? "anonymous";
                    var ip = context.Connection.RemoteIpAddress?.ToString();
                    var method = context.Request.Method;
                    var path = context.Request.Path;
                    var statusCode = context.Response.StatusCode;

                    logger.LogInformation("{Method} {Path} responded {StatusCode} in {Elapsed}ms | IP: {IP} | ClientId: {ClientId} | CorrelationId: {CorrelationId}",
                        method, path, statusCode, stopwatch.ElapsedMilliseconds, ip, userId, correlationId);
                }
            });
        }

        public static IServiceCollection AddSwaggerWithJwtAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Currency Converter API",
                    Version = "v1",
                    Description = "API for currency conversion using Frankfurter"
                });

                // Configure JWT Bearer authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your JWT token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        // FrankfurterCurrencyProvider.cs (manual retry + circuit breaker)
        public class FrankfurterCurrencyProvider
        {
            private readonly HttpClient _httpClient;
            private static int _failureCount = 0;
            private static DateTime _circuitOpenedUntil = DateTime.MinValue;

            public FrankfurterCurrencyProvider(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<string?> GetRatesAsync(string url)
            {
                if (DateTime.UtcNow < _circuitOpenedUntil)
                {
                    throw new Exception("Circuit is open. Try again later.");
                }

                int maxRetries = 3;
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        var response = await _httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            _failureCount = 0;
                            return await response.Content.ReadAsStringAsync();
                        }

                        if ((int)response.StatusCode >= 500)
                            throw new HttpRequestException("Server error");

                        return null;
                    }
                    catch
                    {
                        _failureCount++;

                        if (_failureCount >= 5)
                        {
                            _circuitOpenedUntil = DateTime.UtcNow.AddSeconds(30);
                        }

                        if (attempt == maxRetries)
                            throw;

                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                    }
                }

                return null;
            }
        }
    }
}