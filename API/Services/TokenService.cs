using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(AppUser user)
        {
            // We create a list of claims
            var claims = new List<Claim>
            {
                // We can add any claims we want to the token. 
                new Claim(ClaimTypes.Name, user.UserName), // We add the username to the token
                new Claim(ClaimTypes.NameIdentifier, user.Id), // We add the user id to the token
                new Claim(ClaimTypes.Email, user.Email), // We add the user email to the token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:SecretForKey"])); // We create a key

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // We create the credentials

            var tokenDescriptor = new SecurityTokenDescriptor // We create the token descriptor
            {
                // We add the issuer
                Issuer = _config["Authentication:Issuer"],
                // We add the audience
                Audience = _config["Authentication:Audience"],
                Subject = new ClaimsIdentity(claims), // We add the claims
                Expires = DateTime.UtcNow.AddDays(7), // We set the expiration date
                SigningCredentials = creds, // We add the credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler(); // We create a token handler

            var token = tokenHandler.CreateToken(tokenDescriptor); // We create the token

            return tokenHandler.WriteToken(token); // We return the token
        }
    }
}