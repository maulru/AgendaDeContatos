using Core.Input;
using Infrastructure.Consumer;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;


namespace Infrastructure
{
    public class RabbitMQConsumerService : IHostedService
    {
        private RabbitMQConsumer _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQConsumerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            // Cria um novo escopo
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Resolve o UsuarioService do escopo
                var contatosService = scope.ServiceProvider.GetRequiredService<BuscaService>();
                var serviceScopeFactory = scope.ServiceProvider.GetService<IServiceScopeFactory>();

                _consumer = new RabbitMQConsumer(contatosService, serviceScopeFactory);
                _consumer.StartListening();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Adicione lógica de limpeza se necessário, como fechar conexões RabbitMQ
            return Task.CompletedTask;
        }
    }
}
