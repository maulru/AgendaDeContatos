using UsuariosAPI.Consumer;

namespace UsuariosAPI.Services
{
    public class RabbitMQAddUserCS : IHostedService
    {
        private RabbitMQAddUserConsumer _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQAddUserCS(IServiceScopeFactory serviceScopeFactory)
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

                _consumer = new RabbitMQAddUserConsumer(usuarioService, serviceScopeFactory);
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
