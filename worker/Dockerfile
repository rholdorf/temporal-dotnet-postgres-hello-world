# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# primeiro copia o csproj, para cachear o restore
COPY worker/Worker.csproj worker/
RUN dotnet restore worker/Worker.csproj

# depois copia o c√digo
COPY worker/ worker/

WORKDIR /src/worker/

RUN dotnet publish -c Release -o /app

# runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Worker.dll"]
