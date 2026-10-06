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

// ============ LOG DE DIAGNÓSTICO ============
Console.WriteLine("=== DIAGNOSTICO DE CONFIGURACION ===");
Console.WriteLine($"Directorio actual: {Directory.GetCurrentDirectory()}");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"appsettings.json existe: {File.Exists("appsettings.json")}");
Console.WriteLine($"appsettings.Production.json existe: {File.Exists("appsettings.Production.json")}");
Console.WriteLine($"LocalSqlServer: {builder.Configuration.GetConnectionString("LocalSqlServer") ?? "NULL"}");
Console.WriteLine($"AzureSql: {builder.Configuration.GetConnectionString("AzureSql") ?? "NULL"}");
Console.WriteLine($"MongoDB: {builder.Configuration.GetConnectionString("MongoDB") ?? "NULL"}");
Console.WriteLine($"Jwt:Key: {builder.Configuration["Jwt:Key"] ?? "NULL"}");
Console.WriteLine($"Jwt:Issuer: {builder.Configuration["Jwt:Issuer"] ?? "NULL"}");
Console.WriteLine($"Jwt:Audience: {builder.Configuration["Jwt:Audience"] ?? "NULL"}");
Console.WriteLine($"DefaultBd: {builder.Configuration["DatabaseOptions:DefaultBd"] ?? "NULL"}");
Console.WriteLine("====================================");

// ============ SERVICIO PARA DETECTAR LA BD ============
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AplicacionDBContexto>(serviceProvider =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    // ✅ Leer el header X-Tipo-BD
    var tipoBdRaw = httpContextAccessor.HttpContext?.Request.Headers["X-Tipo-BD"].FirstOrDefault();

    // ✅ Normalizar: si es null, string vacío o espacios, usar default
    var tipoBd = (tipoBdRaw ?? "").Trim().ToLower();
    if (string.IsNullOrEmpty(tipoBd))
    {
        tipoBd = (configuration["DatabaseOptions:DefaultBd"] ?? "local").Trim().ToLower();
    }
    if (string.IsNullOrEmpty(tipoBd))
    {
        tipoBd = "local";
    }

    Console.WriteLine($"[DB Context] X-Tipo-BD raw: '{tipoBdRaw}' -> normalizado: '{tipoBd}'");

    // ✅ Leer connection string DIRECTAMENTE
    string? connectionString = null;

    if (tipoBd == "azure")
    {
        connectionString = configuration.GetConnectionString("AzureSql");
        Console.WriteLine($"[DB Context] AzureSql: {(string.IsNullOrEmpty(connectionString) ? "VACIO" : "OK")}");
    }

    // ✅ Si no es azure O azure está vacío, usar Local
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        connectionString = configuration.GetConnectionString("LocalSqlServer");
        Console.WriteLine($"[DB Context] LocalSqlServer: {(string.IsNullOrEmpty(connectionString) ? "VACIO" : "OK")}");
    }

    // ✅ Fallback: leer directo de la sección ConnectionStrings
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        connectionString = configuration["ConnectionStrings:LocalSqlServer"];
        Console.WriteLine($"[DB Context] Fallback ConnectionStrings:LocalSqlServer: {(string.IsNullOrEmpty(connectionString) ? "VACIO" : "OK")}");
    }

    // ✅ ÚLTIMO recurso: hardcodear
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        Console.WriteLine("!!! FALLBACK: usando connection string hardcodeada");
        connectionString = "Server=localhost;Database=SystemVentas;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    }

    Console.WriteLine($"[DB Context] Connection string final: {connectionString.Substring(0, Math.Min(60, connectionString.Length))}...");

    var optionsBuilder = new DbContextOptionsBuilder<AplicacionDBContexto>();
    optionsBuilder.UseSqlServer(connectionString);
    return new AplicacionDBContexto(optionsBuilder.Options);
});

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
builder.Services.AddScoped<DetaTelRepositorio, DetaTelRepositorioImpl>();

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
builder.Services.AddScoped<ReportePdfService>();

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
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API v1", Version = "v1" });
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

// --- Autenticación JWT ---
builder.Services.AddScoped<AuthService>();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    Console.WriteLine("!!! ERROR: Jwt:Key es null o vacío. Usando clave por defecto.");
    jwtKey = "una-clave-secreta-muy-larga-de-al-menos-32-caracteres-para-desarrollo";
}

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProyectoVentas",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ProyectoVentasUsuarios",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        };
    });

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4321", "http://localhost:4321")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ===== MONGODB =====
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
Console.WriteLine($"[MongoDB] Connection string: {mongoConnectionString}");
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase("LogsDB");

builder.Services.AddSingleton<IMongoCollection<CompraHistorial>>(mongoDatabase.GetCollection<CompraHistorial>("compras_historial"));
builder.Services.AddSingleton<IMongoCollection<PedidoHistorial>>(mongoDatabase.GetCollection<PedidoHistorial>("pedidos_historial"));
builder.Services.AddSingleton<IMongoCollection<OfertaHistorial>>(mongoDatabase.GetCollection<OfertaHistorial>("ofertas_historial"));
builder.Services.AddSingleton<IMongoCollection<OrdenHistorial>>(mongoDatabase.GetCollection<OrdenHistorial>("ordenes_historial"));
builder.Services.AddSingleton<IMongoCollection<CancelacionHistorial>>(mongoDatabase.GetCollection<CancelacionHistorial>("cancelaciones_historial"));
builder.Services.AddSingleton<IMongoCollection<AdjudicacionHistorial>>(mongoDatabase.GetCollection<AdjudicacionHistorial>("adjudicaciones_historial"));
builder.Services.AddSingleton<IMongoCollection<PedidoActualizacionHistorial>>(mongoDatabase.GetCollection<PedidoActualizacionHistorial>("pedidos_actualizaciones_historial"));
builder.Services.AddSingleton<IMongoCollection<OfertaActualizacionHistorial>>(mongoDatabase.GetCollection<OfertaActualizacionHistorial>("ofertas_actualizaciones_historial"));

builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

// ===== MASSTRANSIT + RABBITMQ =====
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CompraRegistradaConsumer>();
    x.AddConsumer<PedidoCreadoConsumer>();
    x.AddConsumer<OfertaRegistradaConsumer>();
    x.AddConsumer<OrdenCreadaConsumer>();
    x.AddConsumer<PedidoCanceladoConsumer>();
    x.AddConsumer<AdjudicacionCreadaConsumer>();
    x.AddConsumer<PedidoActualizadoConsumer>();
    x.AddConsumer<OfertaActualizadaConsumer>();

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

// ===== MIDDLEWARE GLOBAL DE EXCEPCIONES CON LOG COMPLETO =====
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        Console.WriteLine("");
        Console.WriteLine("========== ERROR EN API ==========");
        Console.WriteLine($"Path: {context.Request.Path}");
        Console.WriteLine($"Method: {context.Request.Method}");
        Console.WriteLine($"Exception: {exception?.GetType().FullName}");
        Console.WriteLine($"Message: {exception?.Message}");
        Console.WriteLine($"Stack Trace:");
        Console.WriteLine(exception?.StackTrace);

        if (exception?.InnerException != null)
        {
            Console.WriteLine($"--- Inner Exception ---");
            Console.WriteLine($"Type: {exception.InnerException.GetType().FullName}");
            Console.WriteLine($"Message: {exception.InnerException.Message}");
            Console.WriteLine($"Stack: {exception.InnerException.StackTrace}");

            if (exception.InnerException.InnerException != null)
            {
                Console.WriteLine($"--- Inner Inner Exception ---");
                Console.WriteLine($"Type: {exception.InnerException.InnerException.GetType().FullName}");
                Console.WriteLine($"Message: {exception.InnerException.InnerException.Message}");
            }
        }
        Console.WriteLine("==================================");
        Console.WriteLine("");

        context.Response.StatusCode = exception is InvalidOperationException ? 400 : 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            mensaje = exception?.Message ?? "Error interno del servidor",
            tipo = exception?.GetType().Name ?? "Error",
            detalle = exception?.InnerException?.Message ?? "",
            stack = exception?.StackTrace ?? ""
        });
    });
});

// ===== MIDDLEWARES =====
app.UseCors("AngularApp");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

Console.WriteLine("=== SERVIDOR LISTO ===");
app.Run();