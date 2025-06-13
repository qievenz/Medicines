# Medicines

React TypeScript with .NET Core Web API template

docker-compose -p medicines down ; docker-compose -p medicines up --build -d

dotnet ef migrations add InitialCreate -p src\Medicines.Infrastructure -s src\Medicines.API
