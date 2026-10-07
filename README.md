# SistemaVentas - Aplicación de Gestión de Compras

Aplicación empresarial de escritorio para la gestión integral de compras, pedidos internos, ofertas de proveedores, adjudicaciones y reportes.

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Características](#características)
- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Requisitos](#requisitos)
- [Instalación Rápida](#instalación-rápida)
- [Uso](#uso)
- [Roles y Permisos](#roles-y-permisos)
- [Reportes](#reportes)
- [Bases de Datos](#bases-de-datos)
- [Documentación](#documentación)
- [Licencia](#licencia)

## 🎯 Descripción

SistemaVentas es una aplicación de escritorio que digitaliza el flujo completo de compras de una empresa:

1. **Creación de Pedidos Internos** → Departamentos solicitan artículos
2. **Órdenes de Compra** → Se agrupan pedidos y se publican
3. **Ofertas de Proveedores** → Proveedores compiten con precios
4. **Adjudicaciones** → Se selecciona al mejor postor
5. **Reportes** → Análisis de gastos, eficiencia y tendencias

## ✨ Características

- ✅ **App de escritorio** (Electron + Angular + .NET)
- ✅ **Doble base de datos** (SQL Server Local + Azure SQL)
- ✅ **Multi-rol** (5 roles con permisos granulares)
- ✅ **8 reportes PDF** generados con QuestPDF
- ✅ **Auditoría completa** con MongoDB
- ✅ **Mensajería asíncrona** con RabbitMQ + MassTransit
- ✅ **Autenticación JWT**
- ✅ **~140 MB portable** (sin instalación)
- ✅ **Sin dependencia de Java** (eliminado JasperReports)

## 🏗️ Arquitectura
┌──────────────────────────────────────────────────────────┐
│ App de Escritorio (Windows) │
│ SistemaVentas.exe (~140 MB) │
├──────────────────────────────────────────────────────────┤
│ │
│ ┌────────────────┐ ┌─────────────────────────┐ │
│ │ Electron │◄──────►│ Backend .NET 10 │ │
│ │ (Chromium) │ HTTP │ (Kestrel en :5000) │ │
│ │ │ │ │ │
│ │ - Angular 22 │ │ - Web API │ │
│ │ - Material │ │ - QuestPDF │ │
│ │ - PDF Viewer │ │ - JWT Auth │ │
│ └────────────────┘ └───────────┬─────────────┘ │
│ │ │
│ ┌─────────────────────────────────────▼─────────────┐ │
│ │ Servidor HTTP local (127.0.0.1:4321) │ │
│ │ Sirve Angular (SPA fallback) │ │
│ └───────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────┘
│
┌───────────────────┼───────────────────┐
│ │ │
▼ ▼ ▼
┌─────────┐ ┌──────────┐ ┌──────────┐
│SQL Server│ │MongoDB │ │RabbitMQ │
│/Azure SQL│ │(Auditoría)│ │(Eventos) │
└─────────┘ └──────────┘ └──────────┘

## 🛠️ Tecnologías

### Backend
- **.NET 10** (Web API)
- **Entity Framework Core** (ORM)
- **SQL Server** / **Azure SQL** (datos transaccionales)
- **MongoDB** (auditoría y logs)
- **RabbitMQ + MassTransit** (mensajería)
- **QuestPDF** (generación de reportes)
- **JWT** (autenticación)
- **BCrypt.Net** (hash de contraseñas)

### Frontend
- **Angular 22** (framework)
- **Angular Material** (UI)
- **TypeScript 6** (tipado)
- **RxJS** (reactividad)

### Desktop
- **Electron 44** (empaquetado)
- **electron-builder** (instalador portable)

## 📦 Requisitos

### Para desarrollo
- Node.js 24+
- .NET SDK 10+
- SQL Server 2022+ (local)
- MongoDB 7+
- RabbitMQ 4+
- Visual Studio Code (recomendado)

### Para el usuario final
- Windows 10/11 (x64)
- SQL Server Local (con BD `SystemVentas`) **O** Azure SQL configurado
- ~200 MB de espacio libre

## 🚀 Instalación Rápida

### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/ProyectoVentas.git
cd ProyectoVentas

2. Configurar el backend

cd Venta
# Editar appsettings.json con tus connection strings
dotnet restore
dotnet build -c Release

3. Compilar el frontend

cd ../proyecto-ventas-front
npm install
npm run build

4. Empaquetar la app de escritorio

cd ../electron
npm install
npx asar pack temp_asar "dist\win-unpacked\resources\app.asar"
npx electron-builder --win portable --prepackaged dist/win-unpacked

5. Ejecutar

cd dist
.\SistemaVentas-Portable-1.0.0.exe

💻 Uso
Selecciona la base de datos:

SQL Server Local (offline)

Azure SQL (nube)

Login con tu cuenta:

Email y contraseña

Navega según tu rol:

Admin → Dashboard, Usuarios, Catálogos, Reportes

Gestor → Pedidos, Órdenes, Adjudicaciones

Proveedor → Ofertas, Pedidos disponibles

Auditor → Reportes, Auditoría


👥 Roles y Permisos
Rol	                                           Permisos
Administrador	                  Control total. Gestiona usuarios, catálogos, órdenes, reportes.
Gestor de Compras	              Crea pedidos, aprueba órdenes, adjudica.
Administrador de Proveedor	    Registra ofertas para pedidos disponibles.
Auditor	                        Solo lectura. Acceso a reportes y auditoría.
Creador de Pedidos	            Solo crea pedidos internos.

📊 Reportes
8 reportes disponibles en PDF (QuestPDF):

Ranking de Proveedores — Top proveedores por monto adjudicado

Gasto Departamental — Gasto por departamento y sucursal

Órdenes Activas — Órdenes abiertas a ofertas

Pedidos Pendientes — Pedidos sin orden asignada

Historial por Artículo — Compras de un artículo

Ofertas por Orden — Comparativa de ofertas

Eficiencia del Proceso — Tiempo promedio de adjudicación

Variación de Precios — Evolución de precios

🗄️ Bases de Datos
{
  "ConnectionStrings": {
    "LocalSqlServer": "Server=localhost;Database=SystemVentas;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}

Azure SQL
{
  "ConnectionStrings": {
    "AzureSql": "Server=tcp:srv-sistemaventas.database.windows.net,1433;Initial Catalog=SystemVentas;User ID=adminventas;Password=TU_PASSWORD;Encrypt=True;"
  }
}

MongoDB (Auditoría)
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  }
}

👨‍💻 Autor
Email: eduardomorales073@gmail.com