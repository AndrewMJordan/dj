#!/usr/bin/env bash

docker run -it --rm -w /opt/app -v $PWD:/opt/app mcr.microsoft.com/dotnet/sdk:5.0 dotnet build
