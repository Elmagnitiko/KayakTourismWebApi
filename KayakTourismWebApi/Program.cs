using KayakData.DatabaseServicesNS;
using KayakTourismWebApi.ServiceExtensionsNS;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Demo API", 
        Version = "v1" 
    });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

await app.SeedDataAsync();

app.MapControllers();

await app.RunAsync();

static void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.ConfigureEntityFramework(config);
    services.RegisterDataAccess();
    services.ConfigureJsonOptions();
    services.ConfigureIdentity();
    services.ConfigureJwtAuthentication(config);
    services.ConfigureTokenService();
    services.ConfigureEmailSender(config);
}