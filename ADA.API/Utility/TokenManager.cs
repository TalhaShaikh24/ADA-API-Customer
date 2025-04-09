
using ADAClassLibrary.DTOLibraries;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace ADA.API.Utility
{
    public class TokenManager
    {
        public static Dictionary<string, string> GeneratedTokens = new Dictionary<string, string>();
        //static string keyText = "Reuters2020-AASA-BMC";
        //static string encryptionSecureKey = "-AASA-BMCReuters2020";
        private static string Secret = "J8c20cxkPZCC/0e0ZUcjrGocsk95gOAqjzJ09apAklM=";
        private static double TokenExpireTime = 3600.0;
        public static string GenerateToken(ClaimDTO obj)
        {

            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", Convert.ToString(obj.Id)),
                    new Claim("Username", Convert.ToString(obj.Username)),
                    new Claim("RegisterType", Convert.ToString(obj.RegisterType)),
                  
                }
                ),
                Issuer = "ADA",
                /*IssuedAt = DateTime.UtcNow.AddHours(Convert.ToDouble(ConfigurationManager.AppSettings["UTCTime"])),*/
                Expires = DateTime.Now.AddMinutes(TokenExpireTime),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            string value = string.Empty;
            bool keyExists = GeneratedTokens.TryGetValue(obj.Username, out value);

            if (keyExists)
            {
                GeneratedTokens.Remove(obj.Username);
                GeneratedTokens.Add(obj.Username, handler.WriteToken(token).ToString());
            }
            else
            {
                GeneratedTokens.Add(obj.Username, handler.WriteToken(token).ToString());
            }

            return handler.WriteToken(token);
        }
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static ClaimDTO ValidateToken(string token)
        {
            ClaimDTO obj = new ClaimDTO();
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim Id = identity.FindFirst("Id");
            Claim Username = identity.FindFirst("Username");
            Claim RegisterType = identity.FindFirst("RegisterType");
        
            obj.Id = Convert.ToInt32(Id.Value);
            obj.Username = Username.Value;
            obj.RegisterType = RegisterType.Value;
            return obj;
        }

        public static ClaimDTO GetValidateToken(HttpRequest httpRequest)
        {
            string value = string.Empty;
            if (!httpRequest.Headers.ContainsKey("Authorization"))
            {
                return null;
            }

            string authHeader = httpRequest.Headers["Authorization"];
            ClaimDTO claimDTO = null;
            string token = authHeader;
            //if (authHeader !=n)
            //{
            //    token = Request.Headers.Authorization.Scheme.ToString() + " " + Request.Headers.Authorization.Parameter.ToString();
            //}
            //else
            //{
            //    return null;
            //}
            //if (authHeader != null)
            //{
            //    token = authHeader.ToString().Split(" ")[1];
            //}

            //var token = Request.Headers.Authorization.Parameter.ToString(); //GetValues("Authorization").FirstOrDefault();
            //if (token == null)
            //{
            //    token = Request.Headers.GetValues("Authorization").FirstOrDefault();
            //    if(token==null)
            //        return null;
            //}



            // string[] headerValues = token.Split(' ');
            //string generatedToken = null;
            if (token != null)
            {
                var item = GeneratedTokens.FirstOrDefault(kvp => kvp.Value == token);
                //if (GeneratedTokens.TryGetValue(headerValues[2], out generatedToken))
                //{
                //    generatedToken=GeneratedTokens[headerValues[2]];
                //}
                if (!item.Equals(default(KeyValuePair<string, string>)))
                {
                    GeneratedTokens.Remove(item.Key);
                }
                
            }

            claimDTO = TokenManager.ValidateToken(token);
            if (claimDTO == null)
            {
                return null;
            }

            return claimDTO;
        }

        public static string RemoveToken(string token)
        {
            var item = GeneratedTokens.FirstOrDefault(kvp => kvp.Value == token);
            if (!item.Equals(default(KeyValuePair<string, string>)))
            {
                GeneratedTokens.Remove(item.Key);
                return null;
            }
            else
            {
                return null;
            }

        }

    }
}
