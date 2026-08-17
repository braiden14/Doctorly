# Build from the solution directory:
# docker build -f SchedulingAssist.Api/Dockerfile -t schedulingassist-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SchedulingAssist.Api/SchedulingAssist.Api.csproj", "SchedulingAssist.Api/"]
COPY ["SchedulingAssist.Application/SchedulingAssist.Application.csproj", "SchedulingAssist.Application/"]
COPY ["SchedulingAssist.Domain/SchedulingAssist.Domain.csproj", "SchedulingAssist.Domain/"]
COPY ["SchedulingAssist.Infrastructure/SchedulingAssist.Infrastructure.csproj", "SchedulingAssist.Infrastructure/"]
RUN dotnet restore "SchedulingAssist.Api/SchedulingAssist.Api.csproj"

COPY . .
WORKDIR "/src/SchedulingAssist.Api"
RUN dotnet publish "SchedulingAssist.Api.csproj" --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
RUN mkdir -p /app/logs && chown -R "$APP_UID:$APP_UID" /app
USER $APP_UID

ENTRYPOINT ["dotnet", "SchedulingAssist.Api.dll"]
