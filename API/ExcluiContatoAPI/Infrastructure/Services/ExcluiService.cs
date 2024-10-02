using Core.Entity;
using Core.Input;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Prometheus;


namespace Infrastructure.Services
{
    public class ExcluiService : Controller, IHealthCheck
    {
        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly ApplicationDbContext _context;
        private Counter contatosAdicionados = Metrics.CreateCounter("contatos_adicionados", "Contatos Adicionados");
        private Counter contatosAlterados = Metrics.CreateCounter("contatos_alterados", "Contatos Alterados");
        private Counter buscasComFiltro = Metrics.CreateCounter("buscas_filtro", "Buscas com Filtro");
        private Counter errosAdicionar = Metrics.CreateCounter("erros_adicionar_contato", "Erros ao adicionar contato");
        private Counter errosAlterar = Metrics.CreateCounter("erros_alterar_contato", "Erros ao alterar contato");

                private static readonly Histogram RequestDurationExcluir = Metrics
           .CreateHistogram("request_duration_add", "Duração das requisições para adicionarContatos em segundos",
            new HistogramConfiguration
            {
                Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
            });

        public ExcluiService(IContatoRepository contatoRepository)
        {
            _contatoRepository = contatoRepository ?? throw new ArgumentNullException(nameof(contatoRepository));
        }




        [Authorize]
        [HttpPost]
        public async Task<JsonResult> ExcluirContato(ContatoInput contatoInput)
        {
            using (RequestDurationExcluir.NewTimer())
            {
                try
                {
                    _contatoRepository.Deletar(contatoInput.Id);

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
