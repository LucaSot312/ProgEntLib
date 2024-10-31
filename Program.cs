
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProgEntLib.Properties;
using ProgEntLib.Service;

namespace ProgEntLib
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.AddSingleton<IMongoClient>(s =>
            {
                var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
                return new MongoClient(settings.ConnectionString);
            });

            builder.Services.AddScoped(s =>
            {
                var client = s.GetRequiredService<IMongoClient>();
                var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
                return client.GetDatabase(settings.DatabaseName);
            });

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddScoped<UtenteService>();
            builder.Services.AddScoped<CategoriaService>();
            builder.Services.AddScoped<LibroService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
