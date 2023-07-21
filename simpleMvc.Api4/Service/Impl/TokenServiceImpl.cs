using Microsoft.IdentityModel.Tokens;
using simpleMvc.Api4.Dto;
using simpleMvc.Api4.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace simpleMvc.Api4.Service.Impl
{
    public class TokenServiceImpl : TokenService
    {
        private TokenRepository tokenRepository;
        public TokenServiceImpl() { 
            this.tokenRepository = new TokenRepository();
        }
        public string GenerateAccessToken(string username, List<RoleRespone> roles)
        {
            var secret = ConfigurationManager.AppSettings["JwtSecret"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];
            var expires = ConfigurationManager.AppSettings["JwtExpires"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, username));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.roleName));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(expires)),
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public User GetUserFromRefreshToken(string refreshToken)
        {
            var user = tokenRepository._context.Users.FirstOrDefault(x => x.refreshToken.ToString() == refreshToken);
            return user;
        }

        public void StoreRefreshToken(string refreshToken, string username, int userId)
        {
            var user = tokenRepository._context.Users.Where(x => x.userId == userId).FirstOrDefault();
            if (user != null)
            {
                user.refreshToken = refreshToken;
                tokenRepository._context.SaveChanges();
            }
        }
    }
}