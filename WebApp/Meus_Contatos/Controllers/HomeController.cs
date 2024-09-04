using Core.Repository;
using Meus_Contatos.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            using (RequestDurationHome.NewTimer())
            {
                var contatos = _contatoRepository.ObterTodos();
                homeExibicoes.Inc();
                return View(contatos);
            }
        }



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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
