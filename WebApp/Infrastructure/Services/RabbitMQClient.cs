using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public RabbitMQClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _replyQueueName = _channel.QueueDeclare(queue: "auth_queue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
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

            _channel.BasicConsume(consumer: _consumer, queue: _replyQueueName, autoAck: true);
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "auth_queue", basicProperties: _props, body: messageBytes);
            return _respQueue.Take(); // Aguarda o token ser recebido
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
