FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copiar os arquivos de solução e do projeto
COPY *.sln ./
COPY Meus_Contatos/*.csproj ./Meus_Contatos/
COPY Core/*.csproj ./Core/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY MeusContatos_Testes/*.csproj ./MeusContatos_Testes/

# Restaurar dependências
RUN dotnet restore

# Copiar o restante dos arquivos
COPY . ./

# Compilar e publicar o projeto principal
RUN dotnet publish Meus_Contatos/Meus_Contatos.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

EXPOSE 8080

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT [ "dotnet", "Meus_Contatos.dll" ]
