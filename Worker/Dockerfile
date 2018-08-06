FROM microsoft/dotnet:2.1-sdk as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -f netcoreapp2.1 -c Release -o Output

FROM microsoft/dotnet:2.1-runtime 
ENV SCHEDULER_IP scheduler
ENV SCHEDULER_PORT 1337
WORKDIR /app
COPY --from=build /app/Output ./
ENTRYPOINT [ "dotnet", "Worker.dll"]