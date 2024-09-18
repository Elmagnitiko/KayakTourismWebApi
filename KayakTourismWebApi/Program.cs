using KayakTourismWebApi.DatabaseServicesNS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigureServices(builder.Services, builder.Configuration);

//builder.Services.AddDbContext<ApplicationDBContext>(options =>
//{
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration config)
{
    services.ConfigureEntityFramework(config);
    services.RegisterDataAccess();
    services.ConfigureJsonOptions(); //? Проверить в первую очередь
}
