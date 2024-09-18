using Core.Entity;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace BuscaContatoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BuscaContatoController : ControllerBase
    {

        #region Propriedades
        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly ApplicationDbContext _context;
        private Counter contatosAdicionados = Metrics.CreateCounter("contatos_adicionados", "Contatos Adicionados");
        private Counter contatosAlterados = Metrics.CreateCounter("contatos_alterados", "Contatos Alterados");
        private Counter buscasComFiltro = Metrics.CreateCounter("buscas_filtro", "Buscas com Filtro");
        private Counter errosAdicionar = Metrics.CreateCounter("erros_adicionar_contato", "Erros ao adicionar contato");
        private Counter errosAlterar = Metrics.CreateCounter("erros_alterar_contato", "Erros ao alterar contato");
        private Counter contatosExcluidos = Metrics.CreateCounter("contatos_excluidos", "Contatos Excluidos");
        private Counter homeExibicoes = Metrics.CreateCounter("home_exibicoes", "Exibições da lista de contatos");


        private static readonly Histogram RequestDurationHome = Metrics
            .CreateHistogram("request_duration_home", "Duração das requisições para a página inicial em segundos",
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
        
        private static readonly Histogram RequestDurationExcluir = Metrics
            .CreateHistogram("request_duration_excluir", "Duração das requisições para exclusão de contatos em segundos",
             new HistogramConfiguration
             {
                 Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
             });
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

        #endregion
    

        #region Construtor
        public BuscaContatoController(ITelefoneRepository telefoneRepository, IContatoRepository contatoRepository, ApplicationDbContext context)
        {
            _telefoneRepository = telefoneRepository;
            _contatoRepository = contatoRepository;
            _context = context;
        }

        #endregion

        #region Metodos
        [HttpGet]
        public async Task<IActionResult> Contatos(string regiao = null, string ddd = null)
        {
            using (RequestDurationBuscaFiltro.NewTimer())
            {
                if (string.IsNullOrEmpty(regiao) && string.IsNullOrEmpty(ddd))
                {
                    var contatos = _contatoRepository.ObterTodos();
                    homeExibicoes.Inc();
                    return Ok(contatos);
                }
                else
                {
                    var query = _context.Contato.Include(c => c.Telefones).ThenInclude(t => t.DDD).ThenInclude(r => r.Regiao).AsQueryable();

                    if (!string.IsNullOrEmpty(regiao))
                    {
                        query = query.Where(c => c.Telefones.Any(t => t.DDD.Regiao.Nome.Contains(regiao)));
                    }

                    if (!string.IsNullOrEmpty(ddd))
                    {
                        query = query.Where(c => c.Telefones.Any(t => t.DDD.Codigo == ddd));
                    }

                    var contatos = query.ToList();
                    buscasComFiltro.Inc();

                    object retorno = new { success = true, data = contatos, totalContatos = contatos.Count };
                    return Ok(retorno);
                }
            }
        }


        #endregion

        #region Metricas


        #endregion





    }
}
