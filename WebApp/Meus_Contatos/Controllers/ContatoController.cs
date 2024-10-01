using Core.Entity;
using Core.Input;
using Core.Repository;
using Core.Response;
using Infrastructure.Repository;
using Infrastructure.Services;
using Meus_Contatos.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Prometheus;

namespace Meus_Contatos.Controllers
{
    public class ContatoController : Controller, IHealthCheck
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

        /// <summary>
        /// Injeção de dependência
        /// </summary>
        /// <param name="telefoneRepository"></param>
        /// <param name="contatoRepository"></param>
        /// <param name="context"></param>
        public ContatoController(ITelefoneRepository telefoneRepository, IContatoRepository contatoRepository, ApplicationDbContext context)
        {
            _telefoneRepository = telefoneRepository;
            _contatoRepository = contatoRepository;
            _context = context;
        }

        /// <summary>
        /// Retornar View
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult AdicionarContato()
        {
            return View();
        }

        /// <summary>
        /// Método para adicionar contato
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult AdicionarContato(ContatoInput input)
        {
            using (RequestDurationAdicionarContato.NewTimer())
            {
                try
                {

                    // Obtenção do DDD
                    RetornoDDD retornoDDD = new RetornoDDD();

                    // Realizar validação do DDD
                    using (var rabbitMQClient = new RabbitMQClient("verifyDDD_queue"))
                    {
                        var payloadMessage = input.NumeroDDD;
                        var responseJson = rabbitMQClient.Call(payloadMessage);
                        retornoDDD = JsonConvert.DeserializeObject<RetornoDDD>(responseJson);
                    }

                    if (retornoDDD == null)
                        return Json(new { success = false, message = "DDD inválido." });

                    // Criação do contato
                    var contato = new AdicionarContatoDTO()
                    {
                        Nome = input.Nome,
                        Email = input.Email,
                        NumeroTelefone = input.NumeroTelefone,
                        DDDId = retornoDDD.Id
                    };

                    string idContatoCadastrado = string.Empty;
                    using (var rabbitMQClient = new RabbitMQClient("addContact_queue"))
                    {
                        var payloadMessage = JsonConvert.SerializeObject(contato);
                        idContatoCadastrado = rabbitMQClient.Call(payloadMessage);
                    }

                    AdicionarTelefoneDTO adicionarTelefoneDTO = new AdicionarTelefoneDTO()
                    {
                        ContatoId = Convert.ToInt32(idContatoCadastrado),
                        DDDId = retornoDDD.Id,
                        NumeroTelefone = input.NumeroTelefone
                    };

                    using (var rabbitMQClient = new RabbitMQClient("addPhone_queue"))
                    {
                        var payloadMessage = JsonConvert.SerializeObject(adicionarTelefoneDTO);
                        var responseJson = rabbitMQClient.Call(payloadMessage);
                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    errosAdicionar.Inc();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        /// <summary>
        /// Action Result para Edição de contato
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult EditarContato(int id)
        {
            var contato = _contatoRepository.ObterPorId(id);
            if (contato == null)
            {
                return NotFound();
            }

            var telefone = contato.Telefones.FirstOrDefault();
            var input = new ContatoInput
            {
                Id = contato.Id,
                Nome = contato.Nome,
                Email = contato.Email,
                NumeroDDD = telefone?.DDD?.Codigo,
                NumeroTelefone = telefone?.NumeroTelefone
            };

            return View(input);
        }

        /// <summary>
        /// Método para editar Contato
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult EditarContato(ContatoInput input)
        {
            using (RequestDurationEditarContato.NewTimer())
            {
                try
                {
                    // Criação da mensagem para o RabbitMQ
                    using (var rabbitMQClient = new RabbitMQClient("update_queue"))
                    {
                        var payloadMessage = JsonConvert.SerializeObject(input);

                        // Envia o payload para o RabbitMQ e espera a resposta
                        var responseJson = rabbitMQClient.Call(payloadMessage);

                        // Deserializar a resposta para o formato ApiResponse<Contato>
                        var response = JsonConvert.DeserializeObject<EncapsulatedResponse<Core.Response.Contato>>(responseJson);

                        // Verifica se a resposta foi bem-sucedida
                        if (response?.Value == null)
                        {
                            errosAlterar.Inc();
                            return Json(new { success = false, message = "Erro ao obter o contato atualizado." });
                        }

                        // Incrementa o contador de contatos alterados
                        contatosAlterados.Inc();

                        // Retorna o contato alterado (response.Data)
                        return Json(new { success = true, data = response.Value });
                    }
                }
                catch (Exception ex)
                {
                    errosAlterar.Inc();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }



        /// <summary>
        /// Método para retornar contatos de acordo com o filtro de Região e/ou DDD
        /// </summary>
        /// <param name="regiao"></param>
        /// <param name="ddd"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ContatosFiltrados(string regiao, string ddd)
        {
            using (RequestDurationBuscaFiltro.NewTimer())
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
                return Json(new { success = true, data = contatos, totalContatos = contatos.Count });
            }
        }

        /// <summary>
        /// Método para carregar o modal na tela de cadastro de usuário
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult LoadUserCreatedModal() //ActionResult para exibir o ModalDialog
        {
            return PartialView("Components/UserCreatedModal");
        }

        /// <summary>
        /// Método para carregar o modal na tela de edição de usuário
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult LoadUserChangedModal() //ActionResult para exibir o ModalDialog
        {
            return PartialView("Components/UserChangedModal");
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }

    }
}
