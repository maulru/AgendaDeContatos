using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Infrastructure.Repository;
using Core.Repository;
using Moq;
using ExcluiContatoAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ExcluiContatoAPI.Tests
{
    public class ExcluirTests
    {
        private readonly IConfiguration _IconnectionString;
        private DbContextOptions<ApplicationDbContext> GetSqlServerDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=contatos;User id=sa;Password=M@sterk3y;TrustServerCertificate=True")
                .Options;
        }
        [Fact]
        public void ExcluirContato_DeveExcluirContato_QuandoInputValido()
        {
            var options = GetSqlServerDbContextOptions();
            using var context = new ApplicationDbContext(options, _IconnectionString);

            var mockContatoRepository = new Mock<IContatoRepository>();
            var controller = new ExcluiContatoController(mockContatoRepository.Object);

            mockContatoRepository.Setup(repo => repo.Deletar(It.IsAny<int>())).Verifiable();

            var result = controller.ExcluirContato(10);

            var jsonString = JsonSerializer.Serialize(result.Value);
            var jsonData = JsonSerializer.Deserialize<JsonResultData>(jsonString);

            Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonData);
            Assert.True(jsonData.success);

            mockContatoRepository.Verify(repo => repo.Deletar(It.IsAny<int>()), Times.Once);
        }
    }
    public class JsonResultData
    {
        public bool success { get; set; }
    }
}