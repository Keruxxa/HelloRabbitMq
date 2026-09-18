FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY ["src/NotificationConsumer/NotificationConsumer.csproj", "NotificationConsumer/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "NotificationConsumer/NotificationConsumer.csproj"

COPY ./src .

RUN dotnet publish "NotificationConsumer/NotificationConsumer.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as final

WORKDIR /app
COPY --from=publish app/publish .
EXPOSE 8080
ENTRYPOINT ["dontet", "NotificationConsumer.csproj"]