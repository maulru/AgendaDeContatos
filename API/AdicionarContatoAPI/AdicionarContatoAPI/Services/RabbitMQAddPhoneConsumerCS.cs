﻿using AdicionarContatoAPI.Consumer;
using Core.Repository;

namespace AdicionarContatoAPI.Services
{
    public class RabbitMQAddPhoneConsumerCS : IHostedService
    {
        #region Propriedades
        private RabbitMQAddPhoneConsumer _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IServiceScope _scope;
        #endregion

        #region Construtores
        public RabbitMQAddPhoneConsumerCS(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        #endregion

        #region Métodos


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _scope = _serviceScopeFactory.CreateScope();
            var contatoRepository = _scope.ServiceProvider.GetRequiredService<IContatoRepository>();
            var telefoneRepository = _scope.ServiceProvider.GetRequiredService<ITelefoneRepository>();
            var serviceScopeFactory = _scope.ServiceProvider.GetService<IServiceScopeFactory>();

            _consumer = new RabbitMQAddPhoneConsumer(contatoRepository, telefoneRepository, serviceScopeFactory);
            _consumer.StartConsuming();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer?.Dispose();
            _scope?.Dispose();
            return Task.CompletedTask;
        }
        #endregion
    }
}
