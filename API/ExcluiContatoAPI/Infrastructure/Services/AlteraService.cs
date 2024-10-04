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

        public AlteraService(ITelefoneRepository telefoneRepository, IContatoRepository contatoRepository, ApplicationDbContext context)
        {
            _telefoneRepository = telefoneRepository;
            _contatoRepository = contatoRepository;
            _context = context;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #region Metodos

        [HttpPost]
        public async Task<IActionResult> EditarContato(ContatoInput input)
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
                Thread.Sleep(1000);
                return Ok(contato);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }


    #endregion
}

