using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProceduresIniciais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var procEstados = @"
            CREATE PROCEDURE SeedEstados
            AS
            BEGIN
                SET NOCOUNT ON;
                IF NOT EXISTS (SELECT 1 FROM Estado)
                BEGIN
                    INSERT INTO Estado (Nome) VALUES 
                    ('Acre'),
                    ('Alagoas'),
                    ('Amapá'),
                    ('Amazonas'),
                    ('Bahia'),
                    ('Ceará'),
                    ('Distrito Federal'),
                    ('Espírito Santo'),
                    ('Goiás'),
                    ('Maranhão'),
                    ('Mato Grosso'),
                    ('Mato Grosso do Sul'),
                    ('Minas Gerais'),
                    ('Pará'),
                    ('Paraíba'),
                    ('Paraná'),
                    ('Pernambuco'),
                    ('Piauí'),
                    ('Rio de Janeiro'),
                    ('Rio Grande do Norte'),
                    ('Rio Grande do Sul'),
                    ('Rondônia'),
                    ('Roraima'),
                    ('Santa Catarina'),
                    ('São Paulo'),
                    ('Sergipe'),
                    ('Tocantins');
                END
            END";

            migrationBuilder.Sql(procEstados);

            var procRegioes = @"CREATE PROCEDURE SeedRegioes
            AS
            BEGIN
                SET NOCOUNT ON;
                IF NOT EXISTS (SELECT 1 FROM Regiao)
                BEGIN
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Norte', (SELECT Id FROM Estado WHERE Nome = 'Acre'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Norte', (SELECT Id FROM Estado WHERE Nome = 'Amapá'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Norte', (SELECT Id FROM Estado WHERE Nome = 'Amazonas'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Norte', (SELECT Id FROM Estado WHERE Nome = 'Pará'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Norte', (SELECT Id FROM Estado WHERE Nome = 'Rondônia'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Norte', (SELECT Id FROM Estado WHERE Nome = 'Roraima'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Norte', (SELECT Id FROM Estado WHERE Nome = 'Tocantins'));

                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Alagoas'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Bahia'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Ceará'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Maranhão'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Paraíba'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Pernambuco'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Piauí'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Rio Grande do Norte'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Nordeste', (SELECT Id FROM Estado WHERE Nome = 'Sergipe'));

                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Centro-Oeste', (SELECT Id FROM Estado WHERE Nome = 'Goiás'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Centro-Oeste', (SELECT Id FROM Estado WHERE Nome = 'Mato Grosso'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Centro-Oeste', (SELECT Id FROM Estado WHERE Nome = 'Mato Grosso do Sul'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Centro-Oeste', (SELECT Id FROM Estado WHERE Nome = 'Distrito Federal'));

                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Sudeste', (SELECT Id FROM Estado WHERE Nome = 'Espírito Santo'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Sudeste', (SELECT Id FROM Estado WHERE Nome = 'Minas Gerais'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Sudeste', (SELECT Id FROM Estado WHERE Nome = 'Rio de Janeiro'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Sudeste', (SELECT Id FROM Estado WHERE Nome = 'São Paulo'));

                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Sul', (SELECT Id FROM Estado WHERE Nome = 'Paraná'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Sul', (SELECT Id FROM Estado WHERE Nome = 'Rio Grande do Sul'));
                    INSERT INTO Regiao (Nome, EstadoId) VALUES ('Sul', (SELECT Id FROM Estado WHERE Nome = 'Santa Catarina'));
                END
            END";

            migrationBuilder.Sql(procRegioes);

            var procDDD = @"CREATE PROCEDURE SeedDDD
            AS
            BEGIN
                SET NOCOUNT ON;
                IF NOT EXISTS (SELECT 1 FROM DDD)
                BEGIN
                -- Norte
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('68', (SELECT Id FROM Regiao WHERE Nome = 'Norte' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Acre')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('96', (SELECT Id FROM Regiao WHERE Nome = 'Norte' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Amapá')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('92', (SELECT Id FROM Regiao WHERE Nome = 'Norte' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Amazonas')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('91', (SELECT Id FROM Regiao WHERE Nome = 'Norte' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Pará')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('69', (SELECT Id FROM Regiao WHERE Nome = 'Norte' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Rondônia')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('95', (SELECT Id FROM Regiao WHERE Nome = 'Norte' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Roraima')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('63', (SELECT Id FROM Regiao WHERE Nome = 'Norte' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Tocantins')));

                -- Nordeste
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('82', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Alagoas')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('71', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Bahia')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('85', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Ceará')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('98', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Maranhão')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('83', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Paraíba')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('81', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Pernambuco')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('86', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Piauí')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('84', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Rio Grande do Norte')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('79', (SELECT Id FROM Regiao WHERE Nome = 'Nordeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Sergipe')));

                -- Centro-Oeste
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('62', (SELECT Id FROM Regiao WHERE Nome = 'Centro-Oeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Goiás')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('65', (SELECT Id FROM Regiao WHERE Nome = 'Centro-Oeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Mato Grosso')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('67', (SELECT Id FROM Regiao WHERE Nome = 'Centro-Oeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Mato Grosso do Sul')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('61', (SELECT Id FROM Regiao WHERE Nome = 'Centro-Oeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Distrito Federal')));

                -- Sudeste
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('27', (SELECT Id FROM Regiao WHERE Nome = 'Sudeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Espírito Santo')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('31', (SELECT Id FROM Regiao WHERE Nome = 'Sudeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Minas Gerais')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('21', (SELECT Id FROM Regiao WHERE Nome = 'Sudeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Rio de Janeiro')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('11', (SELECT Id FROM Regiao WHERE Nome = 'Sudeste' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'São Paulo')));

                -- Sul
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('41', (SELECT Id FROM Regiao WHERE Nome = 'Sul' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Paraná')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('51', (SELECT Id FROM Regiao WHERE Nome = 'Sul' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Rio Grande do Sul')));
                INSERT INTO DDD (Codigo, RegiaoId) VALUES ('48', (SELECT Id FROM Regiao WHERE Nome = 'Sul' AND EstadoId = (SELECT Id FROM Estado WHERE Nome = 'Santa Catarina')));

                END
            END";

            migrationBuilder.Sql(procDDD);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SeedEstados");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SeedRegioes");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SeedDDD");
        }
    }
}
