FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MenuConsumerService.Worker/MenuConsumerService.Worker.csproj", "MenuConsumerService.Worker/"]
COPY ["MenuConsumerService.Application/MenuConsumerService.Application.csproj", "MenuConsumerService.Application/"]
COPY ["MenuConsumerService.Domain/MenuConsumerService.Domain.csproj", "MenuConsumerService.Domain/"]
COPY ["MenuConsumerService.Infrastructure/MenuConsumerService.Infrastructure.csproj", "MenuConsumerService.Infrastructure/"]
RUN dotnet restore "MenuConsumerService.Worker/MenuConsumerService.Worker.csproj"
COPY . .
WORKDIR "/src/MenuConsumerService.Worker"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "MenuConsumerService.Worker.dll"]
