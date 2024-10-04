using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Core.Input;
using Newtonsoft.Json;

namespace Infrastructure.Consumer
{
    public class RabbitMQExcluiConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _props;
        private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
        private readonly string _correlationId;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ExcluiService _excluiService;

        public RabbitMQExcluiConsumer(ExcluiService excluiService, IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "delete_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _serviceScopeFactory = serviceScopeFactory;
            _excluiService = excluiService;
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "delete_queue", basicProperties: _props, body: messageBytes);
            return _respQueue.Take();
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var request = JsonConvert.DeserializeObject<ContatoInput>(message);

                // Criação do escopo para injetar serviços
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var excluiService = scope.ServiceProvider.GetRequiredService<ExcluiService>();

                    // Chama o método ExcluirContato
                    var resultado = await excluiService.ExcluirContato(request);

                    // Serializa a resposta, garantindo que não haja loop de referências
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    var response = JsonConvert.SerializeObject(resultado, settings);

                    // Propriedades de resposta
                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    var responseBytes = Encoding.UTF8.GetBytes(response);

                    // Envia a resposta para a fila de resposta
                    _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProps, body: responseBytes);
                }
            };

            // Consumir mensagens da fila "delete_queue"
            _channel.BasicConsume(queue: "delete_queue", autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
