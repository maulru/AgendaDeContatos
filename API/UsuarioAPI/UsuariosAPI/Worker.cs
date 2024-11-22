
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UsuariosAPI.Models;

namespace UsuariosAPI
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "rabbitmq-service",
                    UserName = "guest",
                    Password = "guest",
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "fila",
                         durable: false,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

                        var consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (sender, eventArgs) =>
                        {
                            var body = eventArgs.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var usuario = JsonSerializer.Deserialize<Usuario>(message);
                            Console.WriteLine(usuario?.ToString());
                        };

                        channel.BasicConsume(queue: "fila",
                            autoAck: true,
                            consumer: consumer);
                    }
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }

    }
}
