using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsuariosAPI.Data.Dtos;
using UsuariosAPI.Services;

namespace UsuariosAPI.Consumer
{
    public class RabbitMQConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly UsuarioService _usuarioService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQConsumer(UsuarioService cadastroService, IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" }; 
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "auth_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _usuarioService = cadastroService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var loginModel = JsonConvert.DeserializeObject<LoginUsuarioDto>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
                    var token = await usuarioService.Login(loginModel);

                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    var responseBytes = Encoding.UTF8.GetBytes(token);
                    _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProps, body: responseBytes);
                }
            };

            _channel.BasicConsume(queue: "auth_queue", autoAck: true, consumer: consumer);
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
