# ReactTSWithNetCoreTemplate

React TypeScript with .NET Core Web API template

docker-compose -p reacttswithnetcoretemplate down ; docker-compose -p reacttswithnetcoretemplate up --build -d

dotnet ef migrations add InitialCreate -p src\ReactTSWithNetCoreTemplate.Infrastructure -s src\ReactTSWithNetCoreTemplate.API
