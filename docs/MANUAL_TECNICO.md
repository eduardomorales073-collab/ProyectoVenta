# Manual Técnico - SistemaVentas

Documentación técnica para desarrolladores.

## 📋 Índice

1. [Arquitectura](#arquitectura)
2. [Estructura del Proyecto](#estructura-del-proyecto)
3. [API Endpoints](#api-endpoints)
4. [Base de Datos](#base-de-datos)
5. [Reportes (JasperReports)](#reportes-jasperreports)
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
├─────────────────────────────────────────┤
│ Infraestructura (Repositorios) │ ← Acceso a datos
│ - Repositorios │
│ - DbContext │
├─────────────────────────────────────────┤
│ JasperReports Server (Docker) │ ← Motor de Reportes (NUEVO)
└─────────────────────────────────────────┘


### Flujo de una Petición

[Angular] → [HTTP :5000] → [Venta API] → [Aplicacion] → [Infraestructura] → [SQL Server / Azure]
↓
[HTTP :8080] → [JasperReports Server (Docker)] ← (Para generación de reportes PDF/HTML)


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
│ │ ├── OfertaProveedorService.cs
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
│ │ └── ...
│ ├── appsettings.json # Configuración
│ ├── Program.cs # Startup
│ └── Venta.csproj
│
├── proyecto-ventas-front/ # Angular
│ ├── src/
│ │ ├── app/
│ │ │ ├── pages/ # Componentes
│ │ │ ├── services/ # Servicios HTTP (incluye jasper.service.ts)
│ │ │ ├── interceptors/ # Interceptores
│ │ │ └── app.routes.ts
│ │ └── environments/
│ ├── package.json
│ └── angular.json
│
├── electron/ # Electron
│ ├── main.js
│ ├── preload.js
│ └── dist/ # .exe portable
│
└── docs/ # Documentación
├── MANUAL_USUARIO.md
├── MANUAL_INSTALACION.md
├── MANUAL_TECNICO.md
└── script.sql

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
| GET | `/api/PedidoInternoControlador/disponibles-para-proveedor` | Para proveedores (filtra por rubro y ofertas existentes) |

### Ofertas de Proveedores
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/OfertaProveedorControlador/mis-ofertas` | Obtiene las ofertas del proveedor logueado (incluye socios y estado adjudicado) |
| POST | `/api/OfertaProveedorControlador` | Registrar oferta |
| PUT | `/api/OfertaProveedorControlador` | Actualizar/Mejorar oferta |
| DELETE | `/api/OfertaProveedorControlador/{id}` | Eliminar oferta |

### Órdenes de Compra
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/OrdenCompraControlador` | Listar |
| POST | `/api/OrdenCompraControlador` | Crear |
| PUT | `/api/OrdenCompraControlador/{id}/aprobar` | Aprobar (pasa a Publicada) |

### Adjudicaciones
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/AdjudicacionControlador` | Listar |
| POST | `/api/AdjudicacionControlador/adjudicar-pedidos` | Adjudicar |

### Relaciones de Proveedores
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/RelacionControlador` | Listar relaciones |
| POST | `/api/RelacionControlador` | Crear relación (Socio Comercial, Matriz, etc.) |
| DELETE | `/api/RelacionControlador/{id}` | Eliminar relación |

## 🗄️ Base de Datos

### Tablas Principales
| Tabla | Descripción |
|-------|-------------|
| `Usuarios` | Usuarios del sistema |
| `Roles` / `Usuarios_Roles` | Roles y relación N:M |
| `Permisos` / `Rol_Permiso` | Permisos granulares |
| `Articulo` | Catálogo de artículos |
| `Proveedor` | Proveedores |
| `Provee_Artic` | Qué provee cada proveedor |
| `Rubro` / `Provee_Rubro` | Rubros (Tecnología, Alimentos, etc.) |
| `Relacion` | Relaciones entre proveedores (Socios, Matriz, etc.) |
| `Departamento` / `Sucursal` | Ubicaciones |
| `Orden_Compra` | Órdenes de compra |
| `Pedido_Interno` | Pedidos internos |
| `Detalle_Pedido` | Artículos del pedido |
| `Oferta_Proveedor` | Ofertas de los proveedores |
| `Adjudicacion` / `Detalle_adjudicacion` | Adjudicaciones (Ganadores) |

## 📄 Reportes (JasperReports)

### Arquitectura de Reportes
Hemos migrado de QuestPDF a **JasperReports Server Community Edition**. El servidor corre en un contenedor **Docker** para evitar problemas de licencias y asegurar compatibilidad.

- **URL del Servidor:** `http://localhost:8080`
- **Credenciales:** `jasperadmin` / `jasperadmin`
- **Ruta de Reportes:** `http://localhost:8080/rest_v2/reports/datasources/{NombreReporte}.html` (o `.pdf`)

### Lista de Reportes
1. `RankingProveedores`
2. `GastoDepartamental`
3. `OrdenesActivas`
4. `PedidosPendientes`
5. `HistorialArticulos`
6. `OfertasOrden`
7. `EficienciaProceso`
8. `VariacionPrecio`

### Integración Frontend (`jasper.service.ts`)
El frontend genera la URL dinámicamente y la carga en un `<iframe>` o redirige a la descarga del PDF:
```typescript
private readonly serverBaseUrl = 'http://localhost:8080';
getUrlReporte(rutaJasper: string): string {
  return `${this.serverBaseUrl}/rest_v2/reports/${rutaLimpia}.html`;
}

🔐 Autenticación JWT

Flujo
1). Login: POST /api/AuthControlador/login

2). Backend: Verifica BCrypt + genera JWT.

3). Frontend: Guarda en localStorage.

4). Interceptor: Añade Authorization: Bearer {token} en cada petición.

Claims del JWT

NameIdentifier: ID del usuario

Role: Rol (Administrador, Proveedor, etc.)

IdProveedor: ID del proveedor (si aplica)

IdDepartamento: ID del departamento (si aplica)

📬 Mensajería (RabbitMQ)

Consumidores
PedidoCreadoConsumer

OfertaRegistradaConsumer

OrdenCreadaConsumer

AdjudicacionCreadaConsumer

PedidoActualizadoConsumer

📝 Auditoría (MongoDB)

Se utiliza MongoDB para guardar un historial de eventos.

Colecciones: compras_historial, pedidos_historial, ofertas_historial, adjudicaciones_historial, etc.

🚀 Despliegue

1. Levantar JasperReports (Docker)
Antes de iniciar el sistema, asegúrate de tener Jasper corriendo:

powershell
# Si el contenedor está detenido, inícialo:
docker start jasperserver
docker start jasper-db

# Si es la primera vez, ejecuta:
docker network create jasper-net
docker run -d --name jasper-db --network jasper-net -e POSTGRES_DB=jasperserver -e POSTGRES_USER=jasperadmin -e POSTGRES_PASSWORD=jasperadmin postgres:14
docker run -d --name jasperserver --network jasper-net -p 8080:8080 -e DB_TYPE=postgresql -e DB_HOST=jasper-db -e DB_PORT=5432 -e DB_USER=jasperadmin -e DB_PASSWORD=jasperadmin -e DB_NAME=jasperserver judahpaul/jasperserver

2. Levantar Backend y Frontend (Desarrollo)
powershell
# Backend
cd Venta
dotnet run

# Frontend
cd proyecto-ventas-front
npm start

3. Distribución (Producción)
1). Compilar el backend: dotnet publish -c Release -r win-x64 --self-contained true

2). Compilar el frontend: npm run build

3). Empaquetar con Electron: npx electron-builder --win portable

4). Copiar el .exe a la PC del usuario (no requiere instalación).