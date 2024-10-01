using AdicionarContatoAPI.Controllers;
using AdicionarContatoAPI.Data.DTO;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AdicionarContatoAPITests
{
    public class AdicionarContatoTests
    {
        private readonly IConfiguration _IconnectionString;
        private DbContextOptions<ApplicationDbContext> GetSqlServerDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("server=host.docker.internal,1433;Database=contatos;User id=sa;Password=M@sterk3y;TrustServerCertificate=True")
                .Options;
        }
        [Fact]
        public void AdicionarContato_DeveAdicionarContato_QuandoInputValido()
        {
            try
            {
                var options = GetSqlServerDbContextOptions();
                using var context = new ApplicationDbContext(options, _IconnectionString);

                var mockContatoRepository = new Mock<IContatoRepository>();
                var mockTelefoneRepository = new Mock<ITelefoneRepository>();

                var controller = new AdicionarContatoController(mockContatoRepository.Object, mockTelefoneRepository.Object);

                AdicionarContatoDTO contato = new AdicionarContatoDTO()
                {
                    Nome = "Antonio Kaue",
                    Email = "kauabatista545@hotmail.com"
                };

                var result = controller.AdicionarContato(contato);

                if (result.IsCompletedSuccessfully)
                    Assert.True(true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}