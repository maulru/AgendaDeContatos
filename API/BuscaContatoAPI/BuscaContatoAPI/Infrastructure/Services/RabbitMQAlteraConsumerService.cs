using Core.Input;
using Infrastructure.Consumer;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;


namespace Infrastructure
{
    public class RabbitMQAlteraConsumerService : IHostedService
    {
        private RabbitMQAlteraConsumer _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQAlteraConsumerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var alteraService = scope.ServiceProvider.GetRequiredService<AlteraService>();
                var serviceScopeFactory = scope.ServiceProvider.GetService<IServiceScopeFactory>();

                _consumer = new RabbitMQAlteraConsumer(alteraService, serviceScopeFactory);
                _consumer.StartListening();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
