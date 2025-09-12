#!/usr/bin/env bash

dotnet publish src/Presentation/Presentation.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    -o ./bin/publish

dotnet ef migrations bundle \
    --project src/Infrastructure/Infrastructure.csproj \
    --startup-project src/Presentation/Presentation.csproj \
    --self-contained \
    -r linux-x64 \
    --output ./bin/publish/Migrations
