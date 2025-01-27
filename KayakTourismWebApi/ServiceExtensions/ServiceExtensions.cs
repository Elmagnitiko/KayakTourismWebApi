using KayakData.DataNS;
using KayakData.ModelsNS;
using KayakTourismWebApi.HelpersNS;
using KayakTourismWebApi.InterfacesNS;
using KayakTourismWebApi.TokenServiceNS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
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
                options.Password.RequiredLength = 10;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

            })
            .AddTokenProvider<DataProtectorTokenProvider<Customer>>("Default")
            .AddTokenProvider<EmailTokenProvider<Customer>>("Email")
            .AddDefaultTokenProviders()
            //.AddTokenProvider<PhoneNumberTokenProvider<Customer>>("Phone")
            .AddEntityFrameworkStores<ApplicationDBContext>();
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
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

        public static void ConfigureEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender, EmailSender>(provider =>
                new EmailSender(
                    configuration["EmailSettings:SmtpServer"],
                    int.Parse(configuration["EmailSettings:SmtpPort"]),
                    configuration["EmailSettings:SmtpUser"],
                    configuration["EmailSettings:SmtpPass"]
                ));
        }
    }
}
