﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProgEntLib.DTO;
using ProgEntLib.Models;

namespace ProgEntLib.Service
{
    public class UtenteService
    {
        private readonly IMongoCollection<Utente> _utentiCollection;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UtenteService> _logger;

        public UtenteService(IMongoCollection<Utente> utenteCollection, IConfiguration configuration, ILogger<UtenteService> logger)
        {
            _utentiCollection = utenteCollection;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Utente> CreaUtenteAsync(DTOUtente utente)
        {
            var existingUser = await _utentiCollection.Find(u => u.Email == utente.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return null;
            }

            utente.Password = BCrypt.Net.BCrypt.HashPassword(utente.Password);

            var trueUser = new Utente
            {
                Nome = utente.Nome,
                Cognome = utente.Cognome,
                Email = utente.Email,
                Password = utente.Password
            };

            await _utentiCollection.InsertOneAsync(trueUser);
            return trueUser;
        }

        public async Task<string?> AutenticaUtenteAsync(DTOLogin login)
        {
            var user = await _utentiCollection.Find(u => u.Email == login.Email).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var TokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Nome),
                    new Claim(ClaimTypes.GivenName, user.Cognome),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(TokenDescriptor);
            var trueToken = tokenHandler.WriteToken(token);
            _logger.LogInformation("Generated token : {Token}", trueToken);
            return tokenHandler.WriteToken(token);
        }
    }
}
