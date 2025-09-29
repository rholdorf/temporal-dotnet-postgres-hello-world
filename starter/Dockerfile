# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copia o csproj e sln para cachear o restore
COPY temporal-dotnet-hello.sln ./
COPY worker/Worker.csproj worker/
COPY starter/Starter.csproj starter/

RUN dotnet restore starter/Starter.csproj

# agora, cacheado o restore, copia o c√digo
COPY worker/ worker/
COPY starter/ starter/

WORKDIR /src/starter
RUN dotnet publish -c Release -o /app

# runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Starter.dll"]

