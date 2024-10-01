using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;
using Core.Input;

public class RabbitMQAlteraConsumer : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _replyQueueName;
    private readonly EventingBasicConsumer _consumer;
    private readonly IBasicProperties _props;
    private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
    private readonly string _correlationId;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AlteraService _alteraService;

    public RabbitMQAlteraConsumer(AlteraService alteraService, IServiceScopeFactory serviceScopeFactory)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "update_queue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        _alteraService = alteraService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public string Call(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "update_queue", basicProperties: _props, body: messageBytes);
        return _respQueue.Take();
    }

    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Deserialize the message into a ContatoInput object
            var request = JsonConvert.DeserializeObject<ContatoInput>(message);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var alteraService = scope.ServiceProvider.GetRequiredService<AlteraService>();

                // Pass the deserialized ContatoInput object to EditarContato
                var resultado = await alteraService.EditarContato(request);

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

        _channel.BasicConsume(queue: "update_queue", autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
