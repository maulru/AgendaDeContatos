using Core.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BuscaDddAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class BuscaDDDController : ControllerBase
    {
        #region Propriedades
        private readonly IDDDRepository _dddRepository;
        #endregion

        #region Construtores
        public BuscaDDDController(IDDDRepository dddRepository)
        {
            _dddRepository = dddRepository;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Método responsável por validar a existência do DDD
        /// </summary>
        /// <param name="codigoDDD"></param>
        /// <returns></returns>
        [HttpGet("ConsultarDDD/{codigoDDD}")]
        public async Task<IActionResult> ConsultarDDD([FromRoute] string codigoDDD)
        {
            if (await _dddRepository.ValidarExistenciaDDD(codigoDDD))
                return Ok();

            return BadRequest();
        }
        #endregion
    }
}
