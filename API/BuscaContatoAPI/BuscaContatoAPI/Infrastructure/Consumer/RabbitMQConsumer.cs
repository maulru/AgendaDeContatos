using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;
using Core.Input;
using Azure;

namespace Infrastructure.Consumer
{
    public class RabbitMQConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _props;
        private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
        private readonly string _correlationId;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BuscaService _buscaService;

        public RabbitMQConsumer(BuscaService buscaService, IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "read_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _buscaService = buscaService;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "read_queue", basicProperties: _props, body: messageBytes);
            return _respQueue.Take();
        }


        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var request = JsonConvert.DeserializeObject<BuscaInput>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var buscaService = scope.ServiceProvider.GetRequiredService<BuscaService>();
                    var resultado = await buscaService.Contatos(request.Regiao, request.Ddd);

                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    var response = JsonConvert.SerializeObject(resultado, settings);


                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProps, body: responseBytes);
                }
            };

            _channel.BasicConsume(queue: "read_queue", autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
