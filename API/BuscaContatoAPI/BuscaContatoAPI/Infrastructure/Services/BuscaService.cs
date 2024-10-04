using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BuscaService : ControllerBase, IHealthCheck
    {

        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly ApplicationDbContext _context;
        private Counter homeExibicoes = Metrics.CreateCounter("home_exibicoes", "Exibições da lista de contatos");
        private static readonly Histogram RequestDurationHome = Metrics
            .CreateHistogram("request_duration_home", "Duração das requisições para a página inicial em segundos",
             new HistogramConfiguration
             {
                 Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10)
             });


        public BuscaService(ITelefoneRepository telefoneRepository, IContatoRepository contatoRepository, ApplicationDbContext context)
        {
            _telefoneRepository = telefoneRepository;
            _contatoRepository = contatoRepository;
            _context = context;
        }

        #region Metodos
        [HttpGet]
        public async Task<IActionResult> Contatos(string regiao = null, string ddd = null)
        {

            using (RequestDurationHome.NewTimer())
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

                    object retorno = new { success = true, data = contatos, totalContatos = contatos.Count };
                    homeExibicoes.Inc();
                    return Ok(contatos);
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
