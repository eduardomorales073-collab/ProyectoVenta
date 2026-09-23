# ProyectoVentas

Sistema de gestión de ventas con **API REST en .NET** y **frontend en Angular**, con autenticación JWT, Entity Framework Core y SQL Server.

---

## 📋 Tabla de contenidos

- [Arquitectura](#-arquitectura)
- [Tecnologías](#-tecnologías)
- [Requisitos previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Ejecución](#-ejecución)
- [Endpoints principales](#-endpoints-principales)
- [Estructura del proyecto](#-estructura-del-proyecto)
- [Scripts útiles](#-scripts-útiles)

---

## 🏗️ Arquitectura

El proyecto sigue una arquitectura en capas:
Frontend (Angular) 
http://localhost:4200 

API REST (.NET) 
https://localhost:7132 

Venta(Controllers + Program.cs)
Aplicacion (Servicios + DTOs + Interfaces)
Infrastructura (EF Core +  Repositorios)
Entity Frame Work Core (DbContext + Migrations)

SQL Server 
Base de datos = SystemVentas


---

## 🛠️ Tecnologías

### Backend
- **.NET 10** con ASP.NET Core Web API
- **Entity Framework Core** (Code First + Migrations)
- **SQL Server** como base de datos
- **JWT** (JSON Web Tokens) para autenticación
- **AutoMapper** para mapeo de DTOs
- **Swagger / OpenAPI** para documentación
- **BCrypt.Net** para hash de contraseñas

### Frontend
- **Angular** (standalone components + zoneless)
- **TypeScript**
- **RxJS** para programación reactiva
- **Angular Router** con guards
- **HTTP Interceptors** para JWT
- **SCSS** para estilos

---

## ✅ Requisitos previos

Antes de empezar, asegúrate de tener instalado:

- [**.NET 10 SDK**](https://dotnet.microsoft.com/download)
- [**Node.js 20+**](https://nodejs.org/) (con npm)
- [**SQL Server**](https://www.microsoft.com/sql-server) (Express o Developer)
- [**Git**](https://git-scm.com/)
- [**Visual Studio 2022+**](https://visualstudio.microsoft.com/) o [**VS Code**](https://code.visualstudio.com/) (opcional)

---

## 📦 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/TU-USUARIO/ProyectoVentas.git
cd ProyectoVentas

Restaurar dependencias del backend

dotnet restore

Instalar dependencias del frontend

cd proyecto-ventas-front
npm install
cd ..

Aplicar migraciones de base de datos

cd Infraestructura
dotnet ef database update --startup-project ../Venta
cd ..

O desde la Consola del Administrador de paquetes de Visual Studio:

Update-Database -Project Infraestructura -StartupProject Venta

Configuración

Backend: Venta/appsettings.json

{
  "ConnectionStrings": {
    "connectionString": "Server=localhost;Database=SystemVentas;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "una-clave-secreta-muy-larga-de-al-menos-32-caracteres",
    "Issuer": "ProyectoVentas",
    "Audience": "ProyectoVentasUsuarios"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

Frontend: proyecto-ventas-front/src/environments/environment.development.ts

export const environment = {
  production: false,
  apiUrl: '/api'
};

El apiUrl: '/api' funciona con el proxy configurado en proxy.conf.json.

 Ejecución
 Necesitas dos terminales abiertas simultáneamente

 Terminal 1 — Backend

 cd Venta
dotnet run --launch-profile https

Deberías ver:

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7132
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development

 Swagger: https://localhost:7132/swagger

API HTTP: http://localhost:5000

API HTTPS: https://localhost:7132

Terminal 2 — Frontend

cd proyecto-ventas-front
npm start

Deberías ver:

** Angular Live Development Server is listening on localhost:4200 **
√ Compiled successfully.
App: http://localhost:4200

Login: http://localhost:4200/login

Artículos: http://localhost:4200/articulos

Endpoints principales

Todos los endpoints están documentados en Swagger: https://localhost:7132/swagger

Método	Endpoint	                   Descripción	
POST	/api/AuthControlador/login	    Iniciar sesión	
GET	    /api/ArticuloControlador	    Listar artículos
GET	    /api/ArticuloControlador/{id}	Obtener artículo por ID	
POST	/api/ArticuloControlador    	Crear artículo	
PUT	    /api/ArticuloControlador	    Actualizar artículo	
DELETE	/api/ArticuloControlador/{id}	Eliminar artículo

Ejemplo de login

curl -X POST https://localhost:7132/api/AuthControlador/login \
  -H "Content-Type: application/json" \
  -d '{"email":"usuario@ejemplo.com","password":"mi-contraseña"}' \
  -k

  Respuesta:

  {
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "nombre": "Juan Pérez",
  "email": "usuario@ejemplo.com"
}

Estructura del proyecto

ProyectoVentas/
├── Aplicacion/                    # Capa de aplicación
│   ├── DTO/                       # Data Transfer Objects
│   ├── Interfaz/                  # Interfaces de servicios
│   ├── Mapping/                   # Perfiles de AutoMapper
│   ├── modelos/                   # Entidades del dominio
│   ├── Repositorio/               # Interfaces de repositorios
│   └── Servicios/                 # Implementación de servicios
│
├── Infraestructura/               # Capa de infraestructura
│   ├── Datos/                     # DbContext + Configuraciones
│   ├── Migrations/                # Migraciones de EF Core
│   └── Repositorio/               # Implementación de repositorios
│
├── Venta/                         # Proyecto Web API
│   ├── Controllers/               # Controladores REST
│   ├── Properties/
│   │   └── launchSettings.json    # Perfiles de arranque
│   ├── Program.cs                 # Punto de entrada + DI
│   ├── appsettings.json           # Configuración base
│   └── appsettings.Development.json
│
├── proyecto-ventas-front/         # Frontend Angular
│   ├── src/
│   │   ├── app/
│   │   │   ├── articulos/         # Componente de artículos
│   │   │   ├── guards/            # Guards de autenticación
│   │   │   ├── interceptors/      # Interceptores HTTP
│   │   │   ├── models/            # Interfaces TypeScript
│   │   │   ├── pages/login/       # Página de login
│   │   │   ├── services/          # Servicios HTTP
│   │   │   ├── app.component.ts
│   │   │   ├── app.config.ts      # Configuración de la app
│   │   │   └── app.routes.ts      # Rutas
│   │   ├── environments/          # Configuración por entorno
│   │   └── main.ts
│   ├── angular.json
│   ├── package.json
│   └── proxy.conf.json            # Proxy hacia el backend
│
├── .gitignore                     # Archivos excluidos de Git
└── README.md                      # Este archivo


Scripts útiles

Comando	                                                                                     Descripción
dotnet restore	                                                                            Restaurar dependencias
dotnet build    	                                                                        Compilar la solución
dotnet run --launch-profile https	                                                        Arrancar con HTTPS
dotnet ef migrations add NombreMigracion --project Infraestructura --startup-project Venta	Crear migración
dotnet ef database update --project Infraestructura --startup-project                       Venta	Aplicar migraciones
dotnet dev-certs https --trust	                                                            Confiar en el certificado de desarrollo

Frontend

Comando	                    Descripción
npm install	            Instalar dependencias
npm start	            Servidor de desarrollo (4200)
npm run build	        Build de producción
npm test	            Ejecutar tests

Git

Comando	                Descripción
git status	            Ver cambios
git add .	            Añadir todo al staging
git commit -m "mensaje"	Crear commit
git push	            Subir al remoto
git pull --rebase	    Traer cambios del remoto

Seguridad

Las contraseñas se almacenan con hash BCrypt.

La autenticación usa JWT con expiración.

El backend valida Issuer, Audience, Lifetime y SigningKey.

CORS configurado solo para http://localhost:4200 en desarrollo.

 Para producción:

Usa una clave JWT larga y aleatoria (mínimo 32 caracteres).

Mueve secretos a User Secrets o variables de entorno.

Habilita app.UseHttpsRedirection().

Restringe el CORS a dominios específicos.

Añade [Authorize] a los controladores que lo requieran.

Problemas comunes
        
Problema	                            Solución
ERR_SSL_PROTOCOL_ERROR	                Asegúrate de usar https://localhost:7132 para HTTPS y http://localhost:5000 para HTTP
MSB3021 / MSB3027	                    Mata el proceso Venta.exe con taskkill /IM Venta.exe /F
ERR_CONNECTION_REFUSED en Angular	    Verifica que el backend está corriendo
CORS policy	                            Verifica proxy.conf.json y que apiUrl: '/api'
401 Unauthorized	                    Asegúrate de haber hecho login primero
Pantalla en blanco en Angular	        Verifica que <router-outlet> está en app.component.html

Licencia

Este proyecto es de uso educativo

Autor

Eduardo — GitHub

Última actualización: Septiembre 2026