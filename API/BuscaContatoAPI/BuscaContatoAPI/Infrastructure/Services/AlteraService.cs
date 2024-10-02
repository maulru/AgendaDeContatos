using Core.Entity;
using Core.Input;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Infrastructure.Services
{
    public class AlteraService : ControllerBase, IHealthCheck
    {

        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly ApplicationDbContext _context;
        private Counter contatosAdicionados = Metrics.CreateCounter("contatos_adicionados", "Contatos Adicionados");
        private Counter contatosAlterados = Metrics.CreateCounter("contatos_alterados", "Contatos Alterados");
        private Counter buscasComFiltro = Metrics.CreateCounter("buscas_filtro", "Buscas com Filtro");
        private Counter errosAdicionar = Metrics.CreateCounter("erros_adicionar_contato", "Erros ao adicionar contato");
        private Counter errosAlterar = Metrics.CreateCounter("erros_alterar_contato", "Erros ao alterar contato");

        private static readonly Histogram RequestDurationAdicionarContato = Metrics
   .CreateHistogram("request_duration_add", "Duração das requisições para adicionarContatos em segundos",
    new HistogramConfiguration
    {
        Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
    });

        private static readonly Histogram RequestDurationEditarContato = Metrics
    .CreateHistogram("request_duration_alterar", "Duração das requisições para editarContatos em segundos",
     new HistogramConfiguration
     {
         Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
     });

        private static readonly Histogram RequestDurationBuscaFiltro = Metrics
 .CreateHistogram("request_duration_busca_filtro", "Duração das requisições para buscaComFiltro em segundos",
  new HistogramConfiguration
  {
      Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
  });

        public AlteraService(ITelefoneRepository telefoneRepository, IContatoRepository contatoRepository, ApplicationDbContext context)
        {
            _telefoneRepository = telefoneRepository;
            _contatoRepository = contatoRepository;
            _context = context;
        }


        #region Metodos

        [HttpPost]
        public async Task<IActionResult> EditarContato(ContatoInput input)
        {
            using (RequestDurationEditarContato.NewTimer())
            {
                try
                {
                    var contato = _contatoRepository.ObterPorId(input.Id);
                    if (contato == null)
                    {
                        return NoContent();
                        //   return Json(new { success = false, message = "Contato não encontrado." });
                    }

                    var ddd = _context.DDD.FirstOrDefault(d => d.Codigo == input.NumeroDDD);
                    if (ddd == null)
                    {
                        return NotFound("DDD não encontrado");
                    }

                    contato.Nome = input.Nome;
                    contato.Email = input.Email;
                    _contatoRepository.Alterar(contato);

                    var telefone = contato.Telefones.FirstOrDefault();
                    if (telefone != null)
                    {
                        telefone.DDDId = ddd.Id;
                        telefone.NumeroTelefone = input.NumeroTelefone;
                        _telefoneRepository.Alterar(telefone);
                    }
                    else
                    {
                        telefone = new Telefone()
                        {
                            ContatoId = contato.Id,
                            DDDId = ddd.Id,
                            NumeroTelefone = input.NumeroTelefone
                        };
                        _telefoneRepository.Cadastrar(telefone);
                    }
                    contatosAlterados.Inc();
                    Thread.Sleep(1000);
                    return Ok(contato);
                }
                catch (Exception ex)
                {
                    errosAlterar.Inc();
                    return BadRequest(ex);
                }
            }
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
