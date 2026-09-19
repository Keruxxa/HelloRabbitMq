FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY ["src/NotificationService/NotificationService.csproj", "NotificationService/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "NotificationService/NotificationService.csproj"

COPY ./src .

RUN dotnet publish "NotificationService/NotificationService.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as final

WORKDIR /app
COPY --from=publish app/publish .
EXPOSE 8080
ENTRYPOINT ["dontet", "NotificationService.csproj"]