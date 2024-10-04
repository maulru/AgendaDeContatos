using BuscaDddAPI.Controllers;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BuscaDddAPI.Tests
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
        public async Task BuscaDDD_DeveConsultarDDD_QuandoInputValido()
        {
            var options = GetSqlServerDbContextOptions();
            using var context = new ApplicationDbContext(options, _IconnectionString);

            // Arrange
            string codigoDDD = "11";

            // mock
            var mockDDDRepository = new Mock<IDDDRepository>();
            mockDDDRepository.Setup(repo => repo.ValidarExistenciaDDD(codigoDDD)).ReturnsAsync(true);

            // controller
            var controller = new BuscaDDDController(mockDDDRepository.Object);

            // Act
            var result = await controller.ConsultarDDD(codigoDDD) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}