using Core.Repository;
using Infrastructure;
using Infrastructure.Middleware;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

void ConfigureServices(IServiceCollection services)
{
    // Habilitar autenticação JWT
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0D9S0A9E03940392034@APSODPASD12#")),
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero

        };
    });

    services.AddControllersWithViews();
   // services.AddControllers();
    services.AddHostedService<MetricsService>();


}

ConfigureServices(builder.Services);

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

app.UseMiddleware<JwtCookieMiddleware>();
app.UseAuthentication();
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

app.MapControllerRoute(
    name: "Home",
    pattern: "{controller=Home}/{action=Index}");

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