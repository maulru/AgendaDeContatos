using AdicionarContatoAPI.Data.DTO;
using Core.Entity;
using Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace AdicionarContatoAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class AdicionarContatoController : ControllerBase, IHealthCheck
    {
        #region Propriedades
        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private Counter contatosAdicionados = Metrics.CreateCounter("contatos_adicionados", "Contatos Adicionados");
        private Counter errosAdicionar = Metrics.CreateCounter("erros_adicionar_contato", "Erros ao adicionar contato");

        private static readonly Histogram RequestDurationAdicionarContato = Metrics
        .CreateHistogram("request_duration_add", "Duração das requisições para adicionarContatos em segundos",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
        });
        #endregion

        #region Construtores
        public AdicionarContatoController(IContatoRepository contatoRepository, ITelefoneRepository telefoneRepository)
        {
            _contatoRepository = contatoRepository;
            _telefoneRepository = telefoneRepository;
        }
        #endregion

        #region Métodos
        [HttpPost("AdicionarContato/")]
        public async Task<IActionResult> AdicionarContato([FromBody] AdicionarContatoDTO model)
        {
            try
            {
                Contato contato = new Contato()
                {
                    Nome = model.Nome,
                    Email = model.Email,
                };
                await _contatoRepository.Cadastrar(contato);

                AdicionarTelefoneDTO adicionarTelefoneDTO = new AdicionarTelefoneDTO()
                {
                    ContatoId = contato.Id,
                    NumeroTelefone = model.NumeroTelefone,
                    DDDId = model.DDDId
                };

                AdicionarTelefone(adicionarTelefoneDTO);

                contatosAdicionados.Inc();
                return Ok(model);
            }
            catch (Exception)
            {
                errosAdicionar.Inc();
                return BadRequest();
                throw;
            }
        }

        [HttpPost("AdicionarTelefone/")]
        public async Task<IActionResult> AdicionarTelefone([FromBody] AdicionarTelefoneDTO model)
        {
            try
            {
                Telefone telefone = new Telefone()
                {
                    ContatoId = model.ContatoId,
                    DDDId = model.DDDId,
                    NumeroTelefone = model.NumeroTelefone
                };
                await _telefoneRepository.Cadastrar(telefone);

                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
