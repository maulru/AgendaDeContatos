using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsuariosAPI.Data.Dtos;
using UsuariosAPI.Services;

namespace UsuariosAPI.Consumer
{
    public class RabbitMQAddUserConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly UsuarioService _usuarioService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQAddUserConsumer(UsuarioService cadastroService, IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "addUser_queue",
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
                string responseMessage = string.Empty;
                var userModel = JsonConvert.DeserializeObject<CreateUsuarioDto>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
                    IdentityResult result = await usuarioService.CadastraAsync(userModel);

                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    if (result.Succeeded)
                    {
                        responseMessage = "Usuário cadastrado com sucesso!";
                    }
                    else
                    {
                        responseMessage = result.Errors.Select(e => e.Description).ToString();
                    }

                    var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                    _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProps, body: responseBytes);
                }
            };

            _channel.BasicConsume(queue: "addUser_queue", autoAck: true, consumer: consumer);
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
