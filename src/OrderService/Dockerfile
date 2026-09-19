FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
WORKDIR /src
COPY ["src/OrderService/OrderService.csproj", "OrderService/"]
COPY ["src/OrderService.Contracts/OrderService.Contracts.csproj", "OrderService.Contracts/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "OrderService/OrderService.csproj"

COPY ./src .

RUN dotnet publish "OrderService/OrderService.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as final
EXPOSE 8080
WORKDIR /app
COPY --from=publish app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]