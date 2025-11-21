FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PerfisdeInvestimento.API/PerfisdeInvestimento.API.csproj", "PerfisdeInvestimento.API/"]
COPY ["PerfisdeInvestimento.Application/PerfisdeInvestimento.Application.csproj", "PerfisdeInvestimento.Application/"]
COPY ["PerfisdeInvestimento.Domain/PerfisdeInvestimento.Domain.csproj", "PerfisdeInvestimento.Domain/"]
COPY ["PerfisdeInvestimento.Infrastructure/PerfisdeInvestimento.Infrastructure.csproj", "PerfisdeInvestimento.Infrastructure/"]
RUN dotnet restore "PerfisdeInvestimento.API/PerfisdeInvestimento.API.csproj"
COPY . .
WORKDIR "/src/PerfisdeInvestimento.API"
RUN dotnet build "PerfisdeInvestimento.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PerfisdeInvestimento.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends locales locales-all && \
    rm -rf /var/lib/apt/lists/*

ENV LANG=pt_BR.UTF-8
ENV LANGUAGE=pt_BR:pt
ENV LC_ALL=pt_BR.UTF-8

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

RUN mkdir -p Data
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PerfisdeInvestimento.API.dll"]
