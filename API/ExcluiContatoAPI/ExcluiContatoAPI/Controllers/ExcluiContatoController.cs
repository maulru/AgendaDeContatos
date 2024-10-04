using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExcluiContatoAPI.Controllers
{
    public class ExcluiContatoController : Controller, IHealthCheck
    {

        #region Propriedades
        private readonly IContatoRepository _contatoRepository;
        private Counter contatosExcluidos = Metrics.CreateCounter("contatos_excluidos", "Contatos Excluidos");
        private Counter homeExibicoes = Metrics.CreateCounter("home_exibicoes", "Exibições da lista de contatos");
        private static readonly Histogram RequestDurationHome = Metrics
            .CreateHistogram("request_duration_home", "Duração das requisições para a página inicial em segundos",
             new HistogramConfiguration
             {
                 Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
             });
        private static readonly Histogram RequestDurationExcluir = Metrics
            .CreateHistogram("request_duration_excluir", "Duração das requisições para exclusão de contatos em segundos",
             new HistogramConfiguration
             {
                 Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
             });

        #endregion


        public ExcluiContatoController(IContatoRepository contatoRepository)
        {
            _contatoRepository = contatoRepository;
        }

        [Authorize]
        [HttpPost]
        public JsonResult ExcluirContato(int id)
        {
            using (RequestDurationExcluir.NewTimer())
            {
                try
                {
                    _contatoRepository.Deletar(id);
                    contatosExcluidos.Inc();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
