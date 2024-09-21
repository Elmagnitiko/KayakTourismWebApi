using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.ModelsNS;
using KayakTourismWebApi.TokenServiceNS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace KayakTourismWebApi.ServiceExtensionsNS
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services) 
        {
            services.AddIdentity<Customer, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;

            }).AddEntityFrameworkStores<ApplicationDBContext>();
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"]))
                    };
                });

            services.AddAuthorization();
        }

        public static void ConfigureJsonOptions(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }

        public static void ConfigureTokenService(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
        }

    }
}
