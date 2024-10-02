using Infrastructure.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class RabbitMQExcluiConsumerService : IHostedService
    {
        private RabbitMQExcluiConsumer _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQExcluiConsumerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var excluiService = scope.ServiceProvider.GetRequiredService<ExcluiService>();
                var serviceScopeFactory = scope.ServiceProvider.GetService<IServiceScopeFactory>();

                _consumer = new RabbitMQExcluiConsumer(excluiService, serviceScopeFactory);
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
