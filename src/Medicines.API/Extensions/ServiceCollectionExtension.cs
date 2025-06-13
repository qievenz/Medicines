using FluentValidation;
using Medicines.API.Authentication;
using Medicines.API.Middlewares;
using Medicines.API.Swagger;
using Medicines.Application.Handlers.Audit;
using Medicines.Application.Handlers.Ingestion;
using Medicines.Application.Handlers.Medicines;
using Medicines.Application.Services;
using Medicines.Application.Validators;
using Medicines.Core.DTOs.Ingestion;
using Medicines.Core.Repositories;
using Medicines.Core.Services;
using Medicines.Core.Settings;
using Medicines.Infrastructure.Persistence;
using Medicines.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Medicines.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settingsTypes = Assembly.GetAssembly(typeof(AppSettings)).GetTypes()
                .Where(w => w.Name.EndsWith("Settings"))
                .ToList();

            foreach (var settingsType in settingsTypes)
            {
                var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethod("Configure", new[] { typeof(IServiceCollection), typeof(IConfiguration) })
                    .MakeGenericMethod(settingsType);

                configureMethod.Invoke(null, new object[] { services, configuration.GetSection(settingsType.Name) });
            }

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Medicines API", Version = "v1" });
                c.OperationFilter<IFormFileOperationFilter>();
                c.AddSecurityDefinition(ApiKeyAuthOptions.SchemeName, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = $"Ingrese su token en el campo de texto de abajo. Ejemplo: \"Bearer ABC123XYZ\""
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    if (!isDevelopment)
                        context.ProblemDetails.Detail = null;
                };
            });
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddApiKeyAuthentication(configuration);
            services.AddAuthorization();

            services.AddScoped<IMedicineRepository, MedicineRepository>();
            services.AddScoped<IIngestionProcessRepository, IngestionProcessRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();

            services.AddScoped<IValidator<MedicineCsvDto>, MedicineCsvDtoValidator>();
            services.AddScoped<IValidator<MedicineJsonDto>, MedicineJsonDtoValidator>();
            services.AddScoped<IMedicineValidationService, MedicineValidationService>();

            services.AddScoped<UploadDataCommandHandler>();
            services.AddScoped<GetIngestionStatusQueryHandler>();
            services.AddScoped<GetPagedMedicinesQueryHandler>();
            services.AddScoped<GetMedicineHistoryQueryHandler>();

            return services;
        }
    }
}
