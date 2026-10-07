# Manual de Instalación - SistemaVentas

Guía para instalar y configurar el sistema.

## 📋 Índice

1. [Requisitos](#requisitos)
2. [Instalación de SQL Server](#instalación-de-sql-server)
3. [Instalación de MongoDB](#instalación-de-mongodb)
4. [Instalación de RabbitMQ](#instalación-de-rabbitmq)
5. [Configuración de Azure SQL (Opcional)](#configuración-de-azure-sql)
6. [Instalación de la Aplicación](#instalación-de-la-aplicación)
7. [Configuración Inicial](#configuración-inicial)
8. [Solución de Problemas](#solución-de-problemas)

## 🖥️ Requisitos

### Hardware mínimo
- **CPU:** 2 núcleos
- **RAM:** 4 GB
- **Disco:** 1 GB libre
- **SO:** Windows 10/11 (x64)

### Software
- **SQL Server** (local) o **Azure SQL** (nube)
- **MongoDB** (para auditoría)
- **RabbitMQ** (opcional, para eventos)

## 🗄️ Instalación de SQL Server

### Opción A: SQL Server Express (Local)

1. Descarga **SQL Server Express 2022**:
   https://www.microsoft.com/es-es/sql-server/sql-server-downloads
2. Ejecuta el instalador.
3. Selecciona **"Básica"**.
4. Acepta los términos.
5. Instala.
6. Instala **SQL Server Management Studio (SSMS)**:
   https://learn.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms

### Crear la base de datos

1. Abre **SSMS**.
2. Conéctate a `localhost`.
3. **Clic derecho en "Bases de datos"** → **"Nueva base de datos"**.
4. Nombre: `SystemVentas`
5. Clic en **"Aceptar"**.

### Ejecutar el script de migración

1. Abre el archivo `docs/script.sql` en SSMS.
2. Selecciona la BD `SystemVentas` en el dropdown.
3. Presiona **F5**.

## 🍃 Instalación de MongoDB

### Windows

1. Descarga **MongoDB Community**:
   https://www.mongodb.com/try/download/community
2. Ejecuta el instalador.
3. Selecciona **"Complete"**.
4. Instala como **servicio** (con "Install MongoDB as a Service").
5. Instala **MongoDB Compass** (GUI):
   https://www.mongodb.com/try/download/compass

### Verificar

```powershell
# Verificar el servicio
Get-Service MongoDB

# Verificar el puerto
Test-NetConnection -ComputerName localhost -Port 27017

 Instalación de RabbitMQ (Opcional)
Windows
Instala Erlang:
https://www.erlang.org/downloads

Instala RabbitMQ:
https://www.rabbitmq.com/download.html

Habilita el plugin de gestión:

powershell
rabbitmq-plugins enable rabbitmq_management

Accede a la consola:
http://localhost:15672

Usuario: guest

Contraseña: guest

powershell
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
Configuración de Azure SQL (Opcional)
1. Crear cuenta de Azure
https://azure.microsoft.com/free/

2. Crear Azure SQL Database
Portal de Azure → "Crear un recurso".

Busca "SQL Database".

Configura:

Grupo de recursos: rg-sistemaventas

Nombre BD: SystemVentas

Servidor: srv-sistemaventas

Ubicación: Mexico Central

Nivel de servicio: Uso general

Nivel de proceso: Sin servidor

Máx vCores: 2

Pausa automática: 20 min

Datos: 32 GB

Firewall: Permitir Azure + IP cliente.

Clic en "Crear".

3. Migrar los datos
Abre SSMS y conéctate a Azure SQL.

Ejecuta el script docs/script.sql.

Verifica las tablas.

Configurar la app

{
  "ConnectionStrings": {
    "AzureSql": "Server=tcp:srv-sistemaventas.database.windows.net,1433;Initial Catalog=SystemVentas;User ID=adminventas;Password=TU_PASSWORD;Encrypt=True;TrustServerCertificate=False;"
  },
  "DatabaseOptions": {
    "AzureConfigurado": true
  }
}

🚀 Instalación de la Aplicación

Opción A: Usar el .exe portable

1) Copia SistemaVentas.exe a tu PC.

2) Doble clic para ejecutar.

3) Listo.


Opción B: Compilar desde el código

bash
# 1. Clonar
git clone https://github.com/tu-usuario/ProyectoVentas.git
cd ProyectoVentas

# 2. Backend
cd Venta
dotnet restore
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained true -o bin\Release\net10.0\publish

# 3. Frontend
cd ..\proyecto-ventas-front
npm install
npm run build

# 4. Electron
cd ..\electron
npm install
Remove-Item -Recurse -Force "temp_asar\browser" -ErrorAction SilentlyContinue
Copy-Item "browser" "temp_asar\browser" -Recurse -Force
npx asar pack temp_asar "dist\win-unpacked\resources\app.asar"
npx electron-builder --win portable --prepackaged dist/win-unpacked


Configuración Inicial
1. Verificar servicios

powershell

# SQL Server
Get-Service MSSQLSERVER

# MongoDB
Get-Service MongoDB

# RabbitMQ
Get-Service RabbitMQ

2. Configurar appsettings.json

Abre Venta\appsettings.json y edita:
{
  "ConnectionStrings": {
    "LocalSqlServer": "Server=localhost;Database=SystemVentas;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "AzureSql": "",
    "MongoDB": "mongodb://localhost:27017"
  },
  "DatabaseOptions": {
    "AzureConfigurado": false,
    "DefaultBd": "local"
  },
  "Jwt": {
    "Key": "una-clave-secreta-muy-larga-de-al-menos-32-caracteres",
    "Issuer": "ProyectoVentas",
    "Audience": "ProyectoVentasUsuarios"
  }
}


3. Ejecutar la app

1) Abre SistemaVentas.exe.

2) Selecciona la BD.

3) Login

🔧 Solución de Problemas

El .exe no arranca
Síntoma: Doble clic no hace nada.

Solución:

1) Verifica que SQL Server esté corriendo.

2) Ejecuta como administrador.

3) Revisa el Event Viewer de Windows.


Error "Login failed for user 'adminventas'"

Causa: La contraseña es incorrecta.

Solución:

1) En el Portal de Azure, ve al servidor SQL.

2) Clic en "Restablecer contraseña".

3) Nueva contraseña: Sistemas2026 (sin caracteres especiales).
 

Error de conexión a Azure SQL

Causa: El firewall bloquea tu IP.

Solución:

1) Portal de Azure → Servidor SQL → "Redes".

2) Marca "Agregar dirección IP del cliente actual".

3) Guarda.


El PDF no se genera

Causa: El backend no está corriendo.

Solución:

1) Abre DevTools (F12) en la app.

2) Ve a la pestaña Console.

3) Busca el error.


La app se queda en blanco al recargar (F5)

Causa: El servidor HTTP local no está sirviendo el SPA fallback.

Solución:

1) Verifica que main.js tenga el servidor HTTP.

2) Recompila y reempaqueta.


📌 Última actualización: 07/10/2026