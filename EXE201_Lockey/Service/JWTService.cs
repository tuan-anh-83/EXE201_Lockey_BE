
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EXE201_Lockey.Service
{
  
        public class JWTService
        {
            private string secureKey = "this is a very secure key that is long enough to meet the requirement";

            public string Generate(int id, string role)
            {
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));
                var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim("role", role.ToString()),
                new Claim("accountId", id.ToString())
            };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    
}
