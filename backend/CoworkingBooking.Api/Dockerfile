# syntax=docker/dockerfile:1
# Build context: backend/
# docker build -f CoworkingBooking.Api/Dockerfile -t cwb-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Camada de restore: apenas os manifestos, para o cache nao invalidar a cada mudanca de codigo
COPY CoworkingBooking.slnx ./
COPY CoworkingBooking.Shared/CoworkingBooking.Shared.csproj                 CoworkingBooking.Shared/
COPY CoworkingBooking.Core/CoworkingBooking.Core.csproj                     CoworkingBooking.Core/
COPY CoworkingBooking.Contracts/CoworkingBooking.Contracts.csproj           CoworkingBooking.Contracts/
COPY CoworkingBooking.Application/CoworkingBooking.Application.csproj       CoworkingBooking.Application/
COPY CoworkingBooking.Infrastructure/CoworkingBooking.Infrastructure.csproj CoworkingBooking.Infrastructure/
COPY CoworkingBooking.Api/CoworkingBooking.Api.csproj                       CoworkingBooking.Api/
RUN dotnet restore CoworkingBooking.Api/CoworkingBooking.Api.csproj

COPY . .
RUN dotnet publish CoworkingBooking.Api/CoworkingBooking.Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_gcServer=1
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "CoworkingBooking.Api.dll"]
