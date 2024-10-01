using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace Infrastructure.Services
{
    public class RabbitMQClient : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _props;
        private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
        private readonly string _correlationId;
        private readonly string _queueName;

        public RabbitMQClient(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;

            // Declara a fila de requisições
            _channel.QueueDeclare(queue: _queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            // Fila de resposta temporária
            _replyQueueName = _channel.QueueDeclare().QueueName;

            _consumer = new EventingBasicConsumer(_channel);
            _correlationId = Guid.NewGuid().ToString();

            _props = _channel.CreateBasicProperties();
            _props.CorrelationId = _correlationId;
            _props.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == _correlationId)
                {
                    _respQueue.Add(response);
                }
            };

            // Consome da fila de resposta
            _channel.BasicConsume(consumer: _consumer, queue: _replyQueueName, autoAck: true);
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            // Publica a mensagem de requisição
            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: _props, body: messageBytes);

            // Aguarda a resposta da fila de resposta
            return _respQueue.Take();
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
