using Aplicacion.Interfaz;
using Aplicacion.Mappings;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Infraestructura.Datos;
using Infraestructura.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Aplicacion.Mensajeria.Consumidores;
using Aplicacion.Mensajeria.Eventos;

using Aplicacion.Modelos;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AplicacionDBContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

// --- Entidades simples ---
builder.Services.AddScoped<IArticuloService, ArticuloServicio>();
builder.Services.AddScoped<ArticuRepositorio, ArticuloRepositorio>();

builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<SucursalRepositorio, SucurRepositorio>();

builder.Services.AddScoped<IAdjudicacionService, AdjudicacionService>();
builder.Services.AddScoped<AdjuRepositorio, AdRepositorio>();

builder.Services.AddScoped<IDepartaentoService, DepartamentoService>();
builder.Services.AddScoped<DepartaRepositorio, DepaRepositorio>();

builder.Services.AddScoped<IDireccionService, DireccionService>();
builder.Services.AddScoped<DireccionRepositorio, DireRepositorio>();

builder.Services.AddScoped<IOfeProveService, OfertaProveedorService>();
builder.Services.AddScoped<OferProvRepositorio, OfeProRepositorio>();

builder.Services.AddScoped<IOrdComService, OrdenCompraService>();
builder.Services.AddScoped<OrdComRepositorio, OrCoRepositorio>();

builder.Services.AddScoped<IPedIntService, PedidoInternoService>();
builder.Services.AddScoped<PedIntRepositorio, PeInRepositorio>();

builder.Services.AddScoped<IPermisosService, PermisosService>();
builder.Services.AddScoped<PermisosRepositorio, PermRepositorio>();

builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<ProveedorRepositorio, ProveRepositorio>();

builder.Services.AddScoped<IRelacionService, RelacionService>();
builder.Services.AddScoped<RelacionRepositorio, RelaRepositorio>();

builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<RolesRepositorio, RolRepositorio>();

builder.Services.AddScoped<IRubroService, RubroService>();
builder.Services.AddScoped<RubroRepositorio, RubrRepositorio>();

builder.Services.AddScoped<ITelefonoService, TelefonoService>();
builder.Services.AddScoped<TelefonoRepositorio, TelRepositorio>();

builder.Services.AddScoped<ITipoOrdenService, TipoOrdeService>();
builder.Services.AddScoped<TipoOrRepositorio, TiOrRepositorio>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<UsuarioRepositorio, UsuarRepositorio>();

builder.Services.AddScoped<IProvRubService, ProveeRubroService>();
builder.Services.AddScoped<ProveRubRepositorio, ProvRubRepositorio>();

// --- Entidades de clave compuesta ---
builder.Services.AddScoped<IDetAdjuService, DetalleAdjudicacionService>();
builder.Services.AddScoped<DetaAdjRepositorio, DeARepositorio>();

builder.Services.AddScoped<IDetPedidoService, DetallePedidoService>();
builder.Services.AddScoped<DetaPedRepositorio, DetPedRepositorio>();

builder.Services.AddScoped<IDetTelefonoService, DetalleTelefonoService>();
builder.Services.AddScoped<DetTelRepositorio, DeTeRepositorio>();

builder.Services.AddScoped<IProvArtService, ProveeArticService>();
builder.Services.AddScoped<ProveArtRepositorio, ProArRepositorio>();

builder.Services.AddScoped<IRolPermiService, RolPermisoService>();
builder.Services.AddScoped<RolPerRepositorio, RolPeRepositorio>();

builder.Services.AddScoped<IUsuarioRolesService, UsuariosRolesService>();
builder.Services.AddScoped<UsRolRepositorio, UsRoRepositorio>();
builder.Services.AddScoped<ReporteRepositorio>();

builder.Services.AddAutoMapper(cfg => { }, typeof(ArticuloPerfil).Assembly);
builder.Services.AddControllers();

builder.Services.AddScoped<ICategoriaProveedorService, CategoriaProveedorService>();
builder.Services.AddScoped<CatProvRepositorio, CatProvRepositorioImpl>();

builder.Services.AddScoped<IUnidadMedidaService, UnidadMedidaService>();
builder.Services.AddScoped<UnidadMedidaRepositorio, UniMedRepositorio>();


// ===== SWAGGER CON JWT =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mi API v1",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT (sin la palabra 'Bearer')"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddScoped<RolPerRepositorio, RolPeRepositorio>();
builder.Services.AddScoped<PermRepositorio, PermRepositorio>();

// --- Autenticación JWT ---
builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        };
    });

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ===== MONGODB =====
// ===== MONGODB =====
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase("LogsDB");

// Registrar las 5 colecciones
builder.Services.AddSingleton<IMongoCollection<CompraHistorial>>(
    mongoDatabase.GetCollection<CompraHistorial>("compras_historial"));

builder.Services.AddSingleton<IMongoCollection<PedidoHistorial>>(
    mongoDatabase.GetCollection<PedidoHistorial>("pedidos_historial"));

builder.Services.AddSingleton<IMongoCollection<OfertaHistorial>>(
    mongoDatabase.GetCollection<OfertaHistorial>("ofertas_historial"));

builder.Services.AddSingleton<IMongoCollection<OrdenHistorial>>(
    mongoDatabase.GetCollection<OrdenHistorial>("ordenes_historial"));

builder.Services.AddSingleton<IMongoCollection<CancelacionHistorial>>(
    mongoDatabase.GetCollection<CancelacionHistorial>("cancelaciones_historial"));

builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

// ===== MASSTRANSIT + RABBITMQ =====
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CompraRegistradaConsumer>();
    x.AddConsumer<PedidoCreadoConsumer>();        
    x.AddConsumer<OfertaRegistradaConsumer>();   
    x.AddConsumer<OrdenCreadaConsumer>();       
    x.AddConsumer<PedidoCanceladoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// ===== BUILD =====
var app = builder.Build();  

// ===== MIDDLEWARE GLOBAL DE EXCEPCIONES =====
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        context.Response.StatusCode = exception is InvalidOperationException ? 400 : 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            mensaje = exception?.Message ?? "Error interno del servidor",
            tipo = exception?.GetType().Name ?? "Error"
        });
    });
});

// ===== MIDDLEWARES =====
app.UseCors("AngularApp");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
    });
}

// app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();
app.Run();