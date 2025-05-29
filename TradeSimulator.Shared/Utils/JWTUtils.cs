using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradeSimulator.Shared.Models;

namespace TradeSimulator.Shared.Utils
{
    public static class JWTUtils
    {
        public static string WriteToken(WriteTokenOptions options)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
            var token = new JwtSecurityToken(
                claims: options.Claims,
                expires: options.ExpirationDate,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
