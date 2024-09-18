using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Prometheus;


namespace Infrastructure
{
    public class MetricsService : BackgroundService
    {
        private readonly Gauge _cpuUsageGauge = Metrics.CreateGauge("cpu_usage_percent", "Porcentagem de uso da CPU");
        private readonly Gauge _memoryUsageGauge = Metrics.CreateGauge("memory_usage_bytes", "Bytes em uso da memória RAM");

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var process = Process.GetCurrentProcess();
            var lastCpuTime = TimeSpan.Zero;
            var lastSampleTime = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                var currentCpuTime = process.TotalProcessorTime;
                var currentTime = DateTime.UtcNow;
                var elapsedCpuTime = (currentCpuTime - lastCpuTime).TotalMilliseconds;
                var elapsedTime = (currentTime - lastSampleTime).TotalMilliseconds;
                var cpuUsage = (elapsedCpuTime / (Environment.ProcessorCount * elapsedTime)) * 100;

                lastCpuTime = currentCpuTime;
                lastSampleTime = currentTime;

                var usedMemory = process.WorkingSet64 /(1024 * 1024);

                _cpuUsageGauge.Set(cpuUsage);
                _memoryUsageGauge.Set(usedMemory);

                await Task.Delay(5000, stoppingToken); // Esperar 5 segundos antes da próxima coleta


            }
        }
    }
}
