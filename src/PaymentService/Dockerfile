FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/PaymentService/PaymentService.csproj", "PaymentService/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "PaymentService/PaymentService.csproj"

COPY /src .

RUN dotnet publish "PaymentService/PaymentService.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as final
WORKDIR /app
EXPOSE 8080
COPY --from=publish app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]