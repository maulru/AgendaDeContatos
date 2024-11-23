using AdicionarContatoAPI.Data.DTO;
using Core.Entity;
using Core.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AdicionarContatoAPI.Consumer
{
    public class RabbitMQAddPhoneConsumer : IDisposable
    {
        #region Propriedades
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private const string queueName = "addPhone_queue";
        #endregion

        #region Construtores
        public RabbitMQAddPhoneConsumer(IContatoRepository contatoRepository, ITelefoneRepository telefoneRepository,
            IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq-service" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            _contatoRepository = contatoRepository;
            _telefoneRepository = telefoneRepository;
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

                var AddPhoneDTO = JsonConvert.DeserializeObject<AdicionarTelefoneDTO>(message);

                await HandleMessage(AddPhoneDTO, ea);
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private async Task HandleMessage(AdicionarTelefoneDTO model, BasicDeliverEventArgs ea)
        {
            try
            {
                Telefone telefone = new Telefone()
                {
                    ContatoId = model.ContatoId,
                    DDDId = model.DDDId,
                    NumeroTelefone = model.NumeroTelefone
                };

                await _telefoneRepository.Cadastrar(telefone);
                var responseBytes = Encoding.UTF8.GetBytes("200");

                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                _channel.BasicPublish(exchange: "",
                                      routingKey: ea.BasicProperties.ReplyTo,
                                      basicProperties: replyProps,
                                      body: responseBytes);
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
