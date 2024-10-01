using AdicionarContatoAPI.Data.DTO;
using Core.Entity;
using Core.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AdicionarContatoAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class AdicionarContatoController : ControllerBase
    {
        #region Propriedades
        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
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

                return Ok(model);
            }
            catch (Exception)
            {
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
        #endregion
    }
}
