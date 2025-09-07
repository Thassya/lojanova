using LojaNova.Function.ProcessarPedido.Data;
using LojaNova.Function.ProcessarPedido.Repositories;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Adiciona o DB Context
builder.Services.AddDbContext<LojaNovaDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("LojaNovaDbConnection")));

// Adiciona o Repositório de Pedidos
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Configuration
            .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(builder.Environment.ContentRootPath, "local.settings.json"), optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
