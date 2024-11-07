using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using ProgEntLib.Models;
using ProgEntLib.Properties;
using ProgEntLib.Service;

namespace ProgEntLib
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurazione di MongoDB
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
            builder.Services.AddScoped<IMongoCollection<Categoria>>(s =>
            {
                var database = s.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Categoria>("Categorie");
            });
            builder.Services.AddScoped<IMongoCollection<Libro>>(s =>
            {
                var database = s.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Libro>("Libri");
            });
            builder.Services.AddScoped<IMongoCollection<Utente>>(s =>
            {
                var database = s.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Utente>("Utenti");
            });
            builder.Services.AddScoped<UtenteService>();
            builder.Services.AddScoped<CategoriaService>();
            builder.Services.AddScoped<LibroService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configurazione JWT
            var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JWTSettings>(jwtSettingsSection);
            var jwtSettings = jwtSettingsSection.Get<JWTSettings>();

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
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    };
                });


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library Catalog API", Version = "v1" });

                // Configura l'autenticazione JWT per Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Inserisci il token JWT nel formato: Bearer <token>"
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
