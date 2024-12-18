﻿using System.Text;
using JWT.Builder;
using JWT.Algorithms;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Models
{
    public static class TokenManager
    {

        public static readonly string _secret = "Superlongsupersecretwithlongwordandmanystuff!";

        public static string GenerateJwtToken(IConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("jsonCopy:Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration.GetSection("jsonCopy:Jwt:Issuer").Value,
                audience: configuration.GetSection("jsonCopy:Jwt:Audience").Value,
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static IDictionary<string, object> VerifyToken(string token , IConfiguration configuration)
        {
            return new JwtBuilder()
                 .WithSecret(configuration.GetSection("jsonCopy:Jwt:Key").Value!)
                 .MustVerifySignature()
                 .Decode<IDictionary<string, object>>(token);
        }
        public static string GenerateAccessToken(string username , IConfiguration configuration)
        {
            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(Encoding.ASCII.GetBytes(configuration.GetSection("jsonCopy:Jwt:Key").Value!))
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
                .AddClaim("username", username)
                .Encode();
        }
        /*
        public static (string key, string jwt) GenerateRefreshToken(string username)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                Convert.ToBase64String(randomNumber);
            }

            var key = System.Text.Encoding.ASCII.GetString(randomNumber);

            string jwt = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secret)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeSeconds())
                .AddClaim("refresh", randomString)
                .AddClaim("username", username)
                .Encode();

            return (key, jwt);
        }
        */

    }
}
