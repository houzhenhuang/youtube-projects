# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ContentPlatform.Presentation/ContentPlatform.Presentation.csproj", "ContentPlatform.Presentation/"]
RUN dotnet restore "./ContentPlatform.Presentation/ContentPlatform.Presentation.csproj"
COPY . .
WORKDIR "/src/ContentPlatform.Presentation"
RUN dotnet build "./ContentPlatform.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ContentPlatform.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Final stage
FROM nginx:alpine3.23 AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ContentPlatform.Presentation/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80