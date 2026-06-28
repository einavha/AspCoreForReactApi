using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;

namespace ReactApp1.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string clientCorsPolicy = "ClientCorsPolicy";

            var builder = WebApplication.CreateBuilder(args);

            var configuredOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

            var environmentOrigins = (builder.Configuration["CORS_ALLOWED_ORIGINS"] ?? string.Empty)
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var allowedOrigins = new[]
                {
                    "http://localhost:5173",
                    "https://localhost:5173",
                    "http://localhost:5174",
                    "https://localhost:5174"
                }
                .Concat(configuredOrigins)
                .Concat(environmentOrigins)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(clientCorsPolicy, policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            var app = builder.Build();

            var dataDirectory = Path.Combine(app.Environment.ContentRootPath, "Data");
            var databasePath = Path.Combine(dataDirectory, "app.db");
            if (!File.Exists(databasePath))
            {
                Directory.CreateDirectory(dataDirectory);
                using var _ = File.Create(databasePath);
            }

            // Configure the HTTP request pipeline.
            /*
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            */

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            var assetsPath = Path.Combine(app.Environment.ContentRootPath, "Assets");
            if (!Directory.Exists(assetsPath))
            {
                assetsPath = Path.Combine(app.Environment.ContentRootPath, "assets");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(assetsPath),
                RequestPath = "/assets"
            });

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("{Time} Incoming request: {Method} {Path}{QueryString}",
                    DateTime.Now,
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString);

                await next();

                logger.LogInformation("{Time} Response: {StatusCode} for {Method} {Path}{QueryString}",
                    DateTime.Now,
                    context.Response.StatusCode,
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString);
            });

            app.UseCors(clientCorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
