using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BuscaService : ControllerBase
    {

        private readonly IContatoRepository _contatoRepository;
        private readonly ITelefoneRepository _telefoneRepository;
        private readonly ApplicationDbContext _context;

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


            if (string.IsNullOrEmpty(regiao) && string.IsNullOrEmpty(ddd))
            {
                var contatos = _contatoRepository.ObterTodos();

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
                return Ok(contatos);
            }

        }

        #endregion

    }
}
