using KayakTourismWebApi.DatabaseServices;
using KayakTourismWebApi.ServiceExtensionsNS;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
//});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache(); // delete this

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

app.MapControllers();

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.ConfigureEntityFramework(config);
    services.RegisterDataAccess();
    services.ConfigureJsonOptions();
    services.ConfigureIdentity();
    services.ConfigureJwtAuthentication(config);
    services.ConfigureTokenService();
    services.ConfigureEmailSender(config);
    services.ConfigureTwoFactorAuthenticationService();
}
