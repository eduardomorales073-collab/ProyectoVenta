using Aplicacion.Interfaz;
using Aplicacion.Mappings;
using Aplicacion.Repositorio;
using Aplicacion.Servicios;
using Infraestructura.Datos;
using Infraestructura.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;  // ← CAMBIO: sin .Models
using System.Text;

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

// ===== SWAGGER CON JWT (OpenApi 2.x) =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mi API v1",
        Version = "v1"
    });

    // 1. Definición de seguridad
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT (sin la palabra 'Bearer')"
    });

    // 2. Requerimiento de seguridad (NUEVA SINTAXIS .NET 10)
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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
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

var app = builder.Build();

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

// HTTPS redirection deshabilitado en desarrollo
// app.UseHttpsRedirection();

app.MapControllers();
app.Run();