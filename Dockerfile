FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source


# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

# copy and publish app and libraries
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENV BotToken=test
ENV LavaLink=test
ENV LavalinkPassword=test
ENV Polly.AccessKey=test
ENV Polly.SecretKey=test
ENV VoicePath=/etc/voice/
# COPY Settings.json .
ENTRYPOINT ["./JustinBot"]