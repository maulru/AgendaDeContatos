using Core.Repository;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddHostedService<MetricsService>();
}

builder.Services.AddHostedService<MetricsService>();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
}, ServiceLifetime.Scoped);

builder.Services.AddScoped<IContatoRepository, ContatoRepository>();

builder.Services.AddScoped<ITelefoneRepository, TelefoneRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("host", context => context.Request.Host.Host);
});
app.UseMetricServer();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics(); 
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conta}/{action=Login}");

app.MapControllerRoute(
    name: "Login",
    pattern: "{controller=Conta}/{action=Login}");

app.MapControllerRoute(
    name: "Registrar",
    pattern: "{controller=Conta}/{action=Registrar}");

ApplyMigrations(app);
app.Run();

void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate(); // Aplica as migrações

        if (!dbContext.Estado.Any())
        {
            var connection = dbContext.Database.GetDbConnection();
            try
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC SeedEstados";
                    command.ExecuteNonQuery();

                    command.CommandText = "EXEC SeedRegioes";
                    command.ExecuteNonQuery();

                    command.CommandText = "EXEC SeedDDD";
                    command.ExecuteNonQuery();
                    
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}