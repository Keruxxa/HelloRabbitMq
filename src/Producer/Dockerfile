FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
WORKDIR /src
COPY ["src/Producer/Producer.csproj", "Producer/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Producer/Producer.csproj"

COPY ./src .

RUN dotnet publish "Producer/Producer.csproj" -c Release -o app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as final
EXPOSE 8080
WORKDIR /app
COPY --from=publish app/publish .
ENTRYPOINT ["dotnet", "Producer.dll"]