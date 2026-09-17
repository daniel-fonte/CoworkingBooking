# syntax=docker/dockerfile:1
# Build context: backend/
# docker build -f CoworkingBooking.Workers/Dockerfile -t cwb-workers .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Camada de restore: apenas os manifestos, para o cache nao invalidar a cada mudanca de codigo
COPY CoworkingBooking.slnx ./
COPY CoworkingBooking.Shared/CoworkingBooking.Shared.csproj                 CoworkingBooking.Shared/
COPY CoworkingBooking.Core/CoworkingBooking.Core.csproj                     CoworkingBooking.Core/
COPY CoworkingBooking.Contracts/CoworkingBooking.Contracts.csproj           CoworkingBooking.Contracts/
COPY CoworkingBooking.Application/CoworkingBooking.Application.csproj       CoworkingBooking.Application/
COPY CoworkingBooking.Infrastructure/CoworkingBooking.Infrastructure.csproj CoworkingBooking.Infrastructure/
COPY CoworkingBooking.Workers/CoworkingBooking.Workers.csproj               CoworkingBooking.Workers/
RUN dotnet restore CoworkingBooking.Workers/CoworkingBooking.Workers.csproj

COPY . .
RUN dotnet publish CoworkingBooking.Workers/CoworkingBooking.Workers.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Production
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "CoworkingBooking.Workers.dll"]
