using Application.Activities;
using Application.Core;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    // We use this class to add our services to the Program.cs. This will clean up the code in the Program.cs
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            // Cors Policy For React App
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
                });
            });

            // We register the MediatR services and tell it to look for the handlers in the Application project
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(List.Handler).Assembly));

            // We register Automapper services and tell it to look for the profiles in the Application project
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            return services;
        }
    }
}