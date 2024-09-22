using Core.Repository;

namespace Infrastructure.Services
{
    public class RabbitMQVerifyDDDCS : IHostedService
    {
        #region Propriedades
        private RabbitMQVerifyDDDConsumer _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IServiceScope _scope;
        #endregion

        #region Construtores
        public RabbitMQVerifyDDDCS(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        #endregion

        #region Métodos IHostedService
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Cria o escopo e mantém durante a vida útil do serviço
            _scope = _serviceScopeFactory.CreateScope();
            var dddRepository = _scope.ServiceProvider.GetRequiredService<IDDDRepository>();
            var serviceScopeFactory = _scope.ServiceProvider.GetService<IServiceScopeFactory>();

            _consumer = new RabbitMQVerifyDDDConsumer(dddRepository, serviceScopeFactory);
            _consumer.StartConsuming();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Finaliza o consumidor e libera recursos
            _consumer?.Dispose();
            _scope?.Dispose();
            return Task.CompletedTask;
        }
        #endregion
    }
}
