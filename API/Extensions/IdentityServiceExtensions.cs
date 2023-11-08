using System.Text;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // We add the Identity services
            services.AddIdentityCore<AppUser>(options =>
            {
                // We configure the password requirements
                options.Password.RequireNonAlphanumeric = false; // We don't require non alphanumeric characters
                options.User.RequireUniqueEmail = true; // We require unique emails
            })
            .AddEntityFrameworkStores<DataContext>();

            // We set the issuer signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Authentication:SecretForKey"]));

            // We add the authentication services with JWT bearer tokens as the default scheme and we set the token validation parameters
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    // We set the validation parameters
                    ValidateIssuerSigningKey = true, // We validate the issuer signing key
                    IssuerSigningKey = key, // We set the issuer signing key
                    ValidateIssuer = true, // We validate the issuer
                    ValidIssuer = config["Authentication:Issuer"], // We set the issuer
                    ValidateAudience = true, // We validate the audience
                    ValidAudience = config["Authentication:Audience"], // We set the audience
                };
            });

            services.AddScoped<TokenService>();

            return services;
        }
    }
}