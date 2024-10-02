using Core.Repository;
using Core.Response;
using Infrastructure.Services;
using Meus_Contatos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Prometheus;
using System.Diagnostics;

namespace Meus_Contatos.Controllers
{

    public class HomeController : Controller
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

        public HomeController(IContatoRepository contatoRepository)
        {
            _contatoRepository = contatoRepository;
        }

        [Authorize]
        public IActionResult Index()
        {
            using (RequestDurationHome.NewTimer())
            {
                using (var rabbitMQClient = new RabbitMQClient("read_queue"))
                {
                    var listaContatosRequest = JsonConvert.SerializeObject(new { });

                    var response = rabbitMQClient.Call(listaContatosRequest);

                    var contatoResponse = JsonConvert.DeserializeObject<ApiResponse<Core.Response.Contato>>(response);

                    if (contatoResponse != null && contatoResponse.Value != null)
                    {
                        homeExibicoes.Inc();
                        var contatosEntity = contatoResponse.Value.Select(c => new Core.Entity.Contato
                        {
                            Nome = c.Nome,
                            Email = c.Email,
                            Telefones = c.Telefones.Select(t => new Core.Entity.Telefone
                            {
                                ContatoId = t.ContatoId,
                                DDDId = t.DDDId,
                                NumeroTelefone = t.NumeroTelefone,
                                DDD = new Core.Entity.DDD
                                {
                                    Id = t.DDD.Id,
                                    Codigo = t.DDD.Codigo,
                                    RegiaoId = t.DDD.RegiaoId,
                                    Regiao = new Core.Entity.Regiao
                                    {
                                        Nome = t.DDD.Regiao.Nome,
                                        EstadoId = t.DDD.Regiao.EstadoId
                                    }
                                }
                            }).ToList(),
                            Id = c.Id
                        }).ToList();

                        return View(contatosEntity); 
                    }
                    else
                    {
                        return View(new List<Core.Response.Contato>());
                    }
                }
            }
        }



        [Authorize]
        [HttpPost]
        public JsonResult ExcluirContato(int id)
        {
            using (RequestDurationExcluir.NewTimer())
            {
                try
                {
                    using (var rabbitMQClient = new RabbitMQClient("delete_queue"))
                    {

                        var excluirContatoRequest = JsonConvert.SerializeObject(new { Id = id });

                        var response = rabbitMQClient.Call(excluirContatoRequest);
                
                        var excluirResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(response);
                        
                        if (excluirResponse != null && excluirResponse.Value != null && excluirResponse.Value.Any() && excluirResponse.Value.First())
                        {
                            contatosExcluidos.Inc();  
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Falha ao excluir contato via RabbitMQ." });
                        }

                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
