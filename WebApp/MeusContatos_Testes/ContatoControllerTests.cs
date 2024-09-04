using Core.Entity;
using Core.Input;
using Core.Repository;
using Infrastructure.Repository;
using Meus_Contatos.Controllers;
using MeusContatos_Testes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;

namespace MeusContatos.Tests
{
    public class ContatoControllerTests : ContatoControllerTestBase
    {

        private readonly IConfiguration _IconnectionString;
        private DbContextOptions<ApplicationDbContext> GetSqlServerDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
        }

        [Fact]
        public void AdicionarContato_DeveAdicionarContato_QuandoInputValido()
        {
            var options = GetSqlServerDbContextOptions();
            using var context = new ApplicationDbContext(options, _IconnectionString);

            var mockContatoRepository = new Mock<IContatoRepository>();
            var mockTelefoneRepository = new Mock<ITelefoneRepository>();

            var controller = new ContatoController(mockTelefoneRepository.Object, mockContatoRepository.Object, context);

            var input = new ContatoInput
            {
                Nome = "John Doe",
                Email = "john@example.com",
                NumeroDDD = "11",
                NumeroTelefone = "123456789"
            };

            var result = controller.AdicionarContato(input) as JsonResult;

            Assert.NotNull(result);
            var jsonString = JsonSerializer.Serialize(result.Value);
            var jsonData = JsonSerializer.Deserialize<JsonResultData>(jsonString);

            Assert.NotNull(jsonData);
            Assert.True(jsonData.success);

            mockContatoRepository.Verify(repo => repo.Cadastrar(It.IsAny<Contato>()), Times.Once);
            mockTelefoneRepository.Verify(repo => repo.Cadastrar(It.IsAny<Telefone>()), Times.Once);
        }

        [Fact]
        public void AdicionarContato_DeveRetornarErro_QuandoDDDInvalido()
        {
            var options = GetSqlServerDbContextOptions();
            using var context = new ApplicationDbContext(options, _IconnectionString);

            var mockContatoRepository = new Mock<IContatoRepository>();
            var mockTelefoneRepository = new Mock<ITelefoneRepository>();

            var controller = new ContatoController(mockTelefoneRepository.Object, mockContatoRepository.Object, context);

            var input = new ContatoInput
            {
                Nome = "John Doe",
                Email = "john@example.com",
                NumeroDDD = "99",
                NumeroTelefone = "123456789"
            };

            var result = controller.AdicionarContato(input) as JsonResult;

            Assert.NotNull(result);
            var jsonString = JsonSerializer.Serialize(result.Value);
            var jsonData = JsonSerializer.Deserialize<JsonResultData>(jsonString);

            Assert.NotNull(jsonData);
            Assert.False(jsonData.success);

            mockContatoRepository.Verify(repo => repo.Cadastrar(It.IsAny<Contato>()), Times.Never);
            mockTelefoneRepository.Verify(repo => repo.Cadastrar(It.IsAny<Telefone>()), Times.Never);
        }

        [Fact]
        public void EditarContato_DeveRetornarNotFound_QuandoContatoNaoExiste()
        {
            var options = GetSqlServerDbContextOptions();
            using var context = new ApplicationDbContext(options, _IconnectionString);

            var mockContatoRepository = new Mock<IContatoRepository>();
            var mockTelefoneRepository = new Mock<ITelefoneRepository>();

            var controller = new ContatoController(mockTelefoneRepository.Object, mockContatoRepository.Object, context);

            mockContatoRepository.Setup(repo => repo.ObterPorId(It.IsAny<int>())).Returns((Contato)null);

            var result = controller.EditarContato(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void EditarContato_DeveAtualizarContatoEAdicionarTelefone_QuandoInputValido()
        {
            var options = GetSqlServerDbContextOptions();
            using var context = new ApplicationDbContext(options, _IconnectionString);

            var contato = new Contato { Id = 10, Nome = "Mauro Roberto", Email = "mauro@gmail.com" };
            var telefone = new Telefone { Id = 10, ContatoId = 10, NumeroTelefone = "11902135414" };
            contato.Telefones = new[] { telefone };

            var mockContatoRepository = new Mock<IContatoRepository>();
            var mockTelefoneRepository = new Mock<ITelefoneRepository>();

            mockContatoRepository.Setup(repo => repo.ObterPorId(It.IsAny<int>())).Returns(contato);

            var controller = new ContatoController(mockTelefoneRepository.Object, mockContatoRepository.Object, context);

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

        public class JsonResultData
        {
            public bool success { get; set; }
        }
    }
}
