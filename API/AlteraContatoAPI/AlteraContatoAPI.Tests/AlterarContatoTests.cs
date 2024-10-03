using AlteraContatoAPI.Controllers;
using Core.Entity;
using Core.Input;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;

namespace AlterarContatoTests
{
    public class AlterarContatoTests
    {
        private readonly IConfiguration _IconnectionString;
        private DbContextOptions<ApplicationDbContext> GetSqlServerDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=contatos;User id=sa;Password=M@sterk3y;TrustServerCertificate=True")
                .Options;
        }
        [Fact]
        public void AlterarContato_DeveAlterarContato_QuandoInputValido()
        {
            try
            {
                var options = GetSqlServerDbContextOptions();
                using var context = new ApplicationDbContext(options, _IconnectionString);

                var contato = new Contato { Id = 10, Nome = "Mauro Roberto", Email = "mauro@gmail.com" };
                var telefone = new Telefone { Id = 10, ContatoId = 10, NumeroTelefone = "11902135414" };
                contato.Telefones = new[] { telefone };

                var mockContatoRepository = new Mock<IContatoRepository>();
                var mockTelefoneRepository = new Mock<ITelefoneRepository>();

                mockContatoRepository.Setup(repo => repo.ObterPorId(It.IsAny<int>())).Returns(contato);

                var controller = new AlteraContatoController(mockTelefoneRepository.Object, mockContatoRepository.Object, context);

                var input = new ContatoInput
                {
                    Id = 10,
                    Nome = "Mauro Robert",
                    Email = "mauro@gmail.com",
                    NumeroDDD = "11",
                    NumeroTelefone = "902135416"
                };


                var result = controller.EditarContato(input) as JsonResult;

                Assert.NotNull(result);

                var jsonString = JsonSerializer.Serialize(result.Value);
                var jsonData = JsonSerializer.Deserialize<JsonResultData>(jsonString);

                Assert.NotNull(jsonData);
                Assert.True(jsonData.success);

                mockContatoRepository.Verify(repo => repo.Alterar(It.IsAny<Contato>()), Times.Once);
                mockTelefoneRepository.Verify(repo => repo.Alterar(It.IsAny<Telefone>()), Times.Once);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class JsonResultData
    {
        public bool success { get; set; }
    }
}