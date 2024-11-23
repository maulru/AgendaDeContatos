using Core.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMQVerifyDDDConsumer : IDisposable
{
    #region Propriedades
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IDDDRepository _dddRepository;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private const string queueName = "verifyDDD_queue";
    #endregion

    #region Construtores
    public RabbitMQVerifyDDDConsumer(IDDDRepository dddRepository, IServiceScopeFactory serviceScopeFactory)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq-service" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: queueName,
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
        _dddRepository = dddRepository;
        _serviceScopeFactory = serviceScopeFactory;
    }
    #endregion

    #region Métodos
    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await HandleMessage(message, ea);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    private async Task HandleMessage(string codigoDDD, BasicDeliverEventArgs ea)
    {
        var responseMessage = await _dddRepository.RetornarDDDPorCodigo(codigoDDD);
        var responseJson = JsonSerializer.Serialize(responseMessage);
        var responseBytes = Encoding.UTF8.GetBytes(responseJson);

        var replyProps = _channel.CreateBasicProperties();
        replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

        _channel.BasicPublish(exchange: "",
                              routingKey: ea.BasicProperties.ReplyTo,
                              basicProperties: replyProps,
                              body: responseBytes);
    }

    // Implementar IDisposable para liberar recursos
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
    #endregion
}
