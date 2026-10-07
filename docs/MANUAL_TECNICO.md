
## 📄 ARCHIVO 4: `docs/MANUAL_TECNICO.md`

**Crea el archivo `C:\Users\eduar\source\repos\ProyectoVentas\docs\MANUAL_TECNICO.md`:**

```markdown
# Manual Técnico - SistemaVentas

Documentación técnica para desarrolladores.

## 📋 Índice

1. [Arquitectura](#arquitectura)
2. [Estructura del Proyecto](#estructura-del-proyecto)
3. [API Endpoints](#api-endpoints)
4. [Base de Datos](#base-de-datos)
5. [Reportes (QuestPDF)](#reportes-questpdf)
6. [Autenticación JWT](#autenticación-jwt)
7. [Mensajería (RabbitMQ)](#mensajería-rabbitmq)
8. [Auditoría (MongoDB)](#auditoría-mongodb)
9. [Empaquetado (Electron)](#empaquetado-electron)
10. [Despliegue](#despliegue)

## 🏗️ Arquitectura

### Capas

┌─────────────────────────────────────────┐
│ Venta (API) │ ← Controladores
├─────────────────────────────────────────┤
│ Aplicacion (Servicios) │ ← Lógica de negocio
│ - Servicios (AuthService, etc.) │
│ - DTOs │
│ - Interfaces │
│ - Reportes (QuestPDF) │
├─────────────────────────────────────────┤
│ Infraestructura (Repositorios) │ ← Acceso a datos
│ - Repositorios │
│ - DbContext │
├─────────────────────────────────────────┤
│ Electron (Desktop) │ ← Frontend
│ - Angular │
│ - main.js │
│ - preload.js │
└─────────────────────────────────────────┘


### Flujo de una Petición

[Angular] → [HTTP :5000] → [Venta API] → [Aplicacion] → [Infraestructura] → [SQL Server]
↓
[Header: X-Tipo-BD: azure]
↓
[Program.cs: selecciona connection string]


## 📂 Estructura del Proyecto

ProyectoVentas/
├── Aplicacion/ # Capa de negocio
│ ├── DTO/ # Data Transfer Objects
│ ├── Interfaces/ # Contratos de servicios
│ ├── Mappings/ # AutoMapper
│ ├── Mensajeria/ # Consumidores MassTransit
│ ├── Modelos/ # Entidades
│ ├── Repositorio/ # Interfaces de repos
│ ├── Servicios/ # Lógica de negocio
│ │ ├── AuthService.cs
│ │ ├── PedidoInternoService.cs
│ │ ├── ReportePdfService.cs # ← QuestPDF
│ │ └── ...
│ └── Aplicacion.csproj
│
├── Infraestructura/ # Capa de datos
│ ├── Datos/
│ │ └── AplicacionDBContexto.cs
│ ├── Repositorio/ # Implementación de repos
│ └── Infraestructura.csproj
│
├── Venta/ # API
│ ├── Controllers/ # Endpoints
│ │ ├── AuthControlador.cs
│ │ ├── PedidoInternoControlador.cs
│ │ ├── ReporteControlador.cs # ← Endpoints PDF
│ │ └── ...
│ ├── appsettings.json # Configuración
│ ├── Program.cs # Startup
│ └── Venta.csproj
│
├── proyecto-ventas-front/ # Angular
│ ├── src/
│ │ ├── app/
│ │ │ ├── pages/ # Componentes
│ │ │ ├── services/ # Servicios HTTP
│ │ │ ├── interceptors/ # Interceptores
│ │ │ ├── guards/ # Guards de rutas
│ │ │ └── app.routes.ts
│ │ └── environments/
│ ├── package.json
│ └── angular.json
│
├── electron/ # Electron
│ ├── main.js # Proceso principal
│ ├── preload.js # API segura
│ ├── package.json
│ ├── browser/ # Angular compilado
│ ├── temp_asar/ # Contenido del asar
│ └── dist/ # .exe portable
│
└── docs/ # Documentación
├── MANUAL_USUARIO.md
├── MANUAL_INSTALACION.md
├── MANUAL_TECNICO.md
└── script.sql # Script de migración


## 🔌 API Endpoints

### Auth

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/AuthControlador/login` | Login con JWT |

### Pedidos Internos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/PedidoInternoControlador` | Listar todos |
| GET | `/api/PedidoInternoControlador/{id}` | Obtener uno |
| POST | `/api/PedidoInternoControlador` | Crear |
| PUT | `/api/PedidoInternoControlador` | Actualizar |
| DELETE | `/api/PedidoInternoControlador/{id}` | Eliminar |
| GET | `/api/PedidoInternoControlador/disponibles-para-proveedor` | Para proveedores |

### Órdenes de Compra

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/OrdenCompraControlador` | Listar |
| GET | `/api/OrdenCompraControlador/con-contadores` | Con contadores |
| POST | `/api/OrdenCompraControlador` | Crear |
| PUT | `/api/OrdenCompraControlador/{id}/aprobar` | Aprobar |

### Adjudicaciones

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/AdjudicacionControlador` | Listar |
| POST | `/api/AdjudicacionControlador/adjudicar-pedidos` | Adjudicar |

### Reportes PDF (QuestPDF)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/ReporteControlador/pdf/ranking-proveedores` | Ranking |
| GET | `/api/ReporteControlador/pdf/gasto-departamental` | Gasto |
| GET | `/api/ReporteControlador/pdf/ordenes-activas` | Órdenes activas |
| GET | `/api/ReporteControlador/pdf/pedidos-pendientes` | Pedidos pendientes |
| GET | `/api/ReporteControlador/pdf/historial-articulo` | Historial |
| GET | `/api/ReporteControlador/pdf/ofertas-orden` | Ofertas |
| GET | `/api/ReporteControlador/pdf/eficiencia-proceso` | Eficiencia |
| GET | `/api/ReporteControlador/pdf/variacion-precios` | Variación |

## 🗄️ Base de Datos

### Tablas

| Tabla | Descripción |
|-------|-------------|
| `Usuarios` | Usuarios del sistema |
| `Roles` | Roles disponibles |
| `Usuarios_Roles` | Relación N:M |
| `Permisos` | Permisos granulares |
| `Rol_Permiso` | Relación N:M |
| `Articulo` | Catálogo de artículos |
| `Proveedor` | Proveedores |
| `Provee_Artic` | Qué provee cada proveedor |
| `Rubro` | Rubros |
| `Provee_Rubro` | Rubros por proveedor |
| `Relacion` | Relaciones entre proveedores |
| `Departamento` | Departamentos |
| `Sucursal` | Sucursales |
| `Direccion` | Direcciones |
| `Telefono` | Teléfonos |
| `Detalle_Telefono` | N:M Sucursal-Teléfono |
| `Tipo_Orden` | Tipos de orden |
| `Orden_Compra` | Órdenes de compra |
| `Pedido_Interno` | Pedidos internos |
| `Detalle_Pedido` | Artículos del pedido |
| `Oferta_Proveedor` | Ofertas |
| `Adjudicacion` | Adjudicaciones |
| `Detalle_adjudicacion` | Detalles de adjudicación |


### Relaciones principales

Usuario ──< Pedido_Interno
Usuario ──< Orden_Compra
Orden_Compra ──< Pedido_Interno
Pedido_Interno ──< Detalle_Pedido >── Articulo
Pedido_Interno ──< Oferta_Proveedor >── Proveedor
Pedido_Interno ──< Detalle_adjudicacion >── Adjudicacion


## 📄 Reportes (QuestPDF)

### Ubicación

`Aplicacion/Servicios/ReportePdfService.cs`

### Métodos

```csharp
public byte[] GenerarRankingProveedores()
public byte[] GenerarGastoDepartamental()
public byte[] GenerarOrdenesActivas()
public byte[] GenerarPedidosPendientes()
public byte[] GenerarHistorialArticulo()
public byte[] GenerarOfertasOrden()
public byte[] GenerarEficienciaProceso()
public byte[] GenerarVariacionPrecios()


Estructura del PDF

Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4.Landscape());
        page.Margin(20);
        page.Header().Element(Encabezado);
        page.Content().Element(TablaDatos);
        page.Footer().Element(PiePagina);
    });
}).GeneratePdf();


🔐 Autenticación JWT

Flujo
Login → POST /api/AuthControlador/login

Backend → Verifica BCrypt + genera JWT

Frontend → Guarda en localStorage

Interceptor → Añade Authorization: Bearer {token}

Claims del JWT
Claim	Descripción
NameIdentifier	ID del usuario
Email	Email
Name	Nombre
Role	Rol (Administrador, etc.)
IdRol	ID del rol
IdProveedor	ID del proveedor (si aplica)
IdDepartamento	ID del departamento (si aplica)


📬 Mensajería (RabbitMQ)

Consumidores
Consumidor	Evento
CompraRegistradaConsumer	Compra registrada
PedidoCreadoConsumer	Pedido creado
OfertaRegistradaConsumer	Oferta registrada
OrdenCreadaConsumer	Orden creada
AdjudicacionCreadaConsumer	Adjudicación creada
PedidoActualizadoConsumer	Pedido actualizado


Configuración

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PedidoCreadoConsumer>();
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


📝 Auditoría (MongoDB)

Colección	Descripción
compras_historial	Compras
pedidos_historial	Pedidos
ofertas_historial	Ofertas
ordenes_historial	Órdenes
cancelaciones_historial	Cancelaciones
adjudicaciones_historial	Adjudicaciones
pedidos_actualizaciones_historial	Actualizaciones
ofertas_actualizaciones_historial	Actualizaciones


📦 Empaquetado (Electron)

Estructura del .exe

SistemaVentas-Portable-1.0.0.exe (~140 MB)
├── SistemaVentas.exe (Electron)
└── resources/
    ├── app.asar (Angular compilado)
    └── backend/ (Backend .NET autocontenido)
   
   
 Proceso

# 1. Compilar Angular
cd proyecto-ventas-front
npm run build

# 2. Copiar a electron
Copy-Item "dist/proyecto-ventas-front/browser" "electron/browser" -Recurse

# 3. Actualizar temp_asar
Copy-Item "electron/browser" "electron/temp_asar/browser" -Recurse

# 4. Reempaquetar asar
npx asar pack temp_asar "dist/win-unpacked/resources/app.asar"

# 5. Generar .exe portable
npx electron-builder --win portable --prepackaged dist/win-unpacked


🚀 Despliegue

Desarrollo

# Backend
cd Venta
dotnet run

# Frontend
cd proyecto-ventas-front
npm start

# Electron (dev)
cd electron
npx electron .


Producción

# 1. Compilar backend
dotnet publish -c Release -r win-x64 --self-contained true -o bin/Release/net10.0/publish

# 2. Compilar frontend
npm run build

# 3. Empaquetar
npx electron-builder --win portable --prepackaged dist/win-unpacked

Distribución

1)Copiar SistemaVentas-Portable-1.0.0.exe a la PC del usuario.

2)Doble clic para ejecutar.

3)No requiere instalación.


📊 Métricas

Componente	Métrica
Backend (.NET 10)	~5 MB
Frontend (Angular)	~1.5 MB
Backend autocontenido	~120 MB
Electron	~100 MB
Total .exe	~140 MB
Tiempo de arranque	~3-5 seg
Tiempo de reporte PDF	~1-2 seg
📌 Última actualización: 07/10/2026