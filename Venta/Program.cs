using Aplicacion.Interfaz;
using Aplicacion.Mappings;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Infraestructura.Datos;
using Infraestructura.Repositorio;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AplicacionDBContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

builder.Services.AddScoped<IArticuloService, ArticuloServicio>();
builder.Services.AddScoped<ArticuRepositorio, ArticuloRepositorio>();

builder.Services.AddScoped<ISucursalRepositorio, SucursalService>();
builder.Services.AddScoped<SucursalRepositorio, SucurRepositorio>();

builder.Services.AddScoped<IAdjudicacionService, AdjudicacionService>(); 
builder.Services.AddScoped<AdjuRepositorio, AdRepositorio>();

builder.Services.AddScoped<IDepartaentoService, DepartamentoService>(); 
builder.Services.AddScoped<DepartaRepositorio, DepaRepositorio>();

builder.Services.AddScoped<IDireccionService, DireccionService>(); // idem
builder.Services.AddScoped<DireccionRepositorio, DireRepositorio>();

builder.Services.AddAutoMapper(cfg => { }, typeof(ArticuloPerfil).Assembly);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Mi API v1");
    });
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
