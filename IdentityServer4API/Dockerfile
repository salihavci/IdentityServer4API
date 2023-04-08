FROM mcr.microsoft.com/dotnet/sdk:5.0 as DockerAspNetSdk
WORKDIR /app
EXPOSE 80
COPY IdentityServer4API/*.csproj IdentityServer4API/
COPY IdentityServer4API/*.csproj IdentityServer4API/
RUN dotnet restore IdentityServer4API/*.csproj
COPY . .
RUN dotnet publish IdentityServer4API/*.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=DockerAspNetSdk /app/out .
ENTRYPOINT [ "dotnet", "IdentityServer4API.dll" ]
