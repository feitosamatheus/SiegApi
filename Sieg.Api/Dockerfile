# ----------------------------------------------------------------
# ETAPA 1: BUILD
# Usa o SDK para compilar e publicar o projeto.
# ----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# 1. Copia o arquivo de solução (.sln) e os arquivos .csproj
COPY *.sln ./
COPY ./Sieg.Application/Sieg.Api.Application.csproj ./Sieg.Application/
COPY ./Sieg.Domain/Sieg.Api.Domain.csproj ./Sieg.Domain/
COPY ./Sieg.Infrastructure/Sieg.Api.Infrastructure.csproj ./Sieg.Infrastructure/
COPY ./Sieg.IoC/Sieg.Api.IoC.csproj ./Sieg.IoC/
COPY ./Sieg.API/Sieg.API.csproj ./Sieg.API/

# 2. Restaura as dependências do projeto principal (API)
RUN dotnet restore ./Sieg.API/Sieg.API.csproj

# 3. Copia o restante do código-fonte
COPY . ./

# 4. Publica o projeto da API
WORKDIR /app/Sieg.API
RUN dotnet publish -c Release -o /publish /p:UseAppHost=false


# ----------------------------------------------------------------
# ETAPA 2: RUNTIME
# Usa a imagem base leve do ASP.NET Core.
# ----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copia os arquivos publicados da etapa de build
COPY --from=build /publish .

# Expõe a porta padrão da aplicação
EXPOSE 8080

# Define o ponto de entrada da API
ENTRYPOINT ["dotnet", "Sieg.API.dll"]
