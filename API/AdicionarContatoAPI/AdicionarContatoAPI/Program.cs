using AdicionarContatoAPI.Services;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;


#region Tags do appsettings
const string ChaveConnectionString = "ConnectionString";
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString(ChaveConnectionString));
}, ServiceLifetime.Scoped);

builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
builder.Services.AddScoped<ITelefoneRepository, TelefoneRepository>();

// Consumers
builder.Services.AddHostedService<RabbitMQAddContactConsumerCS>();
builder.Services.AddHostedService<RabbitMQAddPhoneConsumerCS>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
