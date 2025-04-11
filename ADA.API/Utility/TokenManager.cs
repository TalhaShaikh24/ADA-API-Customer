using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using ADAClassLibrary.DTOLibraries;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;

public class TokenManager
{
    private readonly IConfiguration _configuration;
    private static readonly Dictionary<string, string> GeneratedTokens = new();

    public TokenManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(ClaimDTO obj)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

        var claims = new List<Claim>
        {
            new Claim("Id", obj.Id.ToString()),
            new Claim("Username", obj.Username),
            new Claim("RegisterType", obj.RegisterType)
        };
       
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                double.Parse(jwtSettings["ExpiryInMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}