using Microsoft.Data.SqlClient;
using PruebaTecnica.Neonet.Api.Configuration;
using PruebaTecnica.Neonet.Api.Repositories;
using PruebaTecnica.Neonet.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//Conexión DB
builder.Services.AddSingleton<SqlConnectionFactory>();
//Productos 
builder.Services.AddScoped<ProductoRepository>();
builder.Services.AddScoped<ProductoService>();

//Clientes
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ClienteService>();

//Ventas y sus detalles
builder.Services.AddScoped<VentaRepository>();
builder.Services.AddScoped<DetalleVentaRepository>();
builder.Services.AddScoped<VentaService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Probar conexión a la base de datos 
using (var scope = app.Services.CreateScope())
{
    var connectionFactory =
        scope.ServiceProvider.GetRequiredService<SqlConnectionFactory>();

    try
    {
        using SqlConnection connection = connectionFactory.CreateConnection();
        connection.Open();

        Console.WriteLine("Conexión a SQL Server realizada correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error al conectar con SQL Server:");
        Console.WriteLine(ex.Message);
    }
}

app.Run();
