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
