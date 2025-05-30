using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.IdentityModel.Tokens;



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
