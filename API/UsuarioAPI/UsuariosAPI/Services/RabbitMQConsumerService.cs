using UsuariosAPI.Consumer;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace UsuariosAPI.Services
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
                var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
                var serviceScopeFactory = scope.ServiceProvider.GetService<IServiceScopeFactory>();

                _consumer = new RabbitMQConsumer(usuarioService,serviceScopeFactory);
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
