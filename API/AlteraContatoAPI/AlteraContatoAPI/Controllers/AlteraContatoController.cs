using Core.Entity;
using Core.Input;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using System.Diagnostics.Metrics;

namespace AlteraContatoAPI.Controllers
{
    public class AlteraContatoController : Controller, IHealthCheck
    {
        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly ApplicationDbContext _context;
        private Counter contatosAlterados = Metrics.CreateCounter("contatos_alterados", "Contatos Alterados");
        private Counter errosAlterar = Metrics.CreateCounter("erros_alterar_contato", "Erros ao alterar contato");


        private static readonly Histogram RequestDurationEditarContato = Metrics
    .CreateHistogram("request_duration_alterar", "Duração das requisições para editarContatos em segundos",
     new HistogramConfiguration
     {
         Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
     });



        /// <summary>
        /// Injeção de dependência
        /// </summary>
        /// <param name="telefoneRepository"></param>
        /// <param name="contatoRepository"></param>
        /// <param name="context"></param>
        public AlteraContatoController(ITelefoneRepository telefoneRepository, IContatoRepository contatoRepository, ApplicationDbContext context)
        {
            _telefoneRepository = telefoneRepository;
            _contatoRepository = contatoRepository;
            _context = context;
        }



        [HttpPost]
        public JsonResult EditarContato(ContatoInput input)
        {
            using (RequestDurationEditarContato.NewTimer())
            {
                try
                {
                    var contato = _contatoRepository.ObterPorId(input.Id);
                    if (contato == null)
                    {
                        return Json(new { success = false, message = "Contato não encontrado." });
                    }

                    var ddd = _context.DDD.FirstOrDefault(d => d.Codigo == input.NumeroDDD);
                    if (ddd == null)
                    {
                        return Json(new { success = false, message = "DDD inválido." });
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
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    errosAlterar.Inc();
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
