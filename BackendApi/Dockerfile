FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY publish/ .
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "BackendApi.dll"]
