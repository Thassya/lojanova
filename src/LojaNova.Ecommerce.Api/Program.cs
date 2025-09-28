using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using LojaNova.Ecommerce.Api.Data;
using LojaNova.Ecommerce.Api.Repositories;
using LojaNova.Ecommerce.Api.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var aiConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    ConnectionString = aiConnectionString
});

// Configura��o do DB Context
builder.Services.AddDbContext<LojaNovaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LojaNovaDbConnection")));

// Configura��o de Reposit�rios
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Configura��o de Servi�os
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Configura��o do Azure Storage Service
builder.Services.AddScoped<IAzureStorageService, AzureStorageService>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("LojaNovaAzureStorage");
    var orderQueueName = builder.Configuration["AzureStorageSettings:OrderQueueName"];
    var productImagesShareName = builder.Configuration["AzureStorageSettings:ProductImagesShareName"];

    var queueClient = new QueueClient(connectionString, orderQueueName);
    var blobServiceClient = new BlobServiceClient(connectionString);
    var shareClient = new ShareClient(connectionString, productImagesShareName);

    // Criar fila e share se n�o existirem (idealmente feito no provisionamento ou migra��o)
    queueClient.CreateIfNotExists();
    //shareClient.GetDirectoryClient("/").CreateIfNotExists(); // Cria o diret�rio raiz do share

    return new AzureStorageService(queueClient, blobServiceClient, shareClient);
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Aplica migra��es pendentes ao iniciar em desenvolvimento
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<LojaNovaDbContext>();
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
