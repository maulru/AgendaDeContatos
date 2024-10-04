using BuscaContatoAPI.Controllers;
using Core.Entity;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BuscaContatoAPI.Tests
{
    public class BuscaTests
    {
        private readonly IConfiguration _IconnectionString;
        private DbContextOptions<ApplicationDbContext> GetSqlServerDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=contatos;User id=sa;Password=M@sterk3y;TrustServerCertificate=True")
                .Options;
        }
        [Fact]
        public async Task BuscaContato_DeveBuscarContato_QuandoNaoPàssadoFiltro()
        {
            var options = GetSqlServerDbContextOptions();
            using var context = new ApplicationDbContext(options, _IconnectionString);

            //arrange
            List<Contato> contatosFake = new List<Contato>()
            {
                new Contato {
                    Id = 1, 
                    Nome = "Antonio Kauã", 
                    Email = "kauabatista545@hotmail.com",
                    Telefones = new List<Telefone> {
                        new Telefone {
                            ContatoId = 1,
                            NumeroTelefone = "989286488" 
                        } }
                }
            };

            //mocks
            var mockTelefoneRepository = new Mock<ITelefoneRepository>();
            var mockContatoRepository = new Mock<IContatoRepository>();

            mockContatoRepository.Setup(repo => repo.ObterTodos()).Returns(contatosFake);

            var controller = new BuscaContatoController(mockTelefoneRepository.Object, mockContatoRepository.Object, context);
            var result = await controller.Contatos() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var contatos = result.Value as IEnumerable<Contato>;
            Assert.Equal(1, contatos.Count()); // Verifica se retornou 1 contato

        }
    }
}