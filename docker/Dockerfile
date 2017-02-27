#
# FSharpCoreWebApi Dockerfile for creating a streamlined image using the artifacts created by Dockerfile.build
#
# https://github.com/lrakai/FSharpCoreWebApi
#

FROM microsoft/aspnetcore:1.0.3

MAINTAINER lrakai

ENV ASPNETCORE_ENVIRONMENT="Production"

WORKDIR /FSharpWebApi

COPY src/FSharpWebApi/bin/Publish .

ENTRYPOINT ["dotnet", "FSharpWebApi.dll"]

EXPOSE 5000
