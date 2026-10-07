
## 📄 ARCHIVO 2: `docs/MANUAL_USUARIO.md`

```markdown
# Manual de Usuario - SistemaVentas

Guía completa para el uso del sistema de gestión de compras.

## 📋 Índice

1. [Inicio de Sesión](#inicio-de-sesión)
2. [Selección de Base de Datos](#selección-de-base-de-datos)
3. [Dashboard](#dashboard)
4. [Roles y Funciones](#roles-y-funciones)
5. [Flujo Completo de Compra](#flujo-completo-de-compra)
6. [Reportes](#reportes)
7. [Preguntas Frecuentes](#preguntas-frecuentes)

## 🔐 Inicio de Sesión

### Pantalla de Login

1. Abre la aplicación **SistemaVentas**.
2. Ingresa tu **email** y **contraseña**.
3. Haz clic en **"Iniciar sesión"**.

### Credenciales de prueba

| Rol | Email | Contraseña |
|-----|-------|------------|
| Administrador | `carlos.lopez@gmail.com` | `Carlos123` |
| Gestor de Compras | `maria.garcia@gmail.com` | `Maria123` |
| Proveedor | `jose.hernandez@gmail.com` | `Jose123` |
| Auditor | `juan.perez@gmail.com` | `Juan123` |
| Creador de Pedidos | `test.creador@test.com` | `Test123` |

⚠️ **Cambia las contraseñas en producción.**

## 🗄️ Selección de Base de Datos

Al iniciar la app, puedes elegir entre 2 bases de datos:

| Opción | Descripción |
|--------|-------------|
| **SQL Server Local** | Base de datos en tu PC. Sin internet. Rápida. |
| **Azure SQL** | Base de datos en la nube. Requiere internet. |

### ¿Cuál elegir?

- **SQL Server Local** → Si trabajas en una sola PC.
- **Azure SQL** → Si quieres compartir datos con otras PCs.

## 📊 Dashboard

El Dashboard muestra:

- **Usuarios totales**
- **Roles activos**
- **Sucursales registradas**
- **Gráfico "Usuarios por Rol"**
- **Gráfico "Estado de Usuarios"**

## 👥 Roles y Funciones

### 👨‍💼 Administrador

**Menú completo:**
- Dashboard
- Artículos
- Usuarios
- Catálogos (Rubros, Categorías, Unidades)
- Compras (Pedidos, Órdenes, Adjudicaciones)
- Admin (Configuración, Reportes, Auditoría)

**Funciones:**
- ✅ Crear/editar/eliminar usuarios
- ✅ Gestionar catálogos
- ✅ Aprobar órdenes
- ✅ Adjudicar pedidos
- ✅ Ver todos los reportes

### 👩‍💼 Gestor de Compras

**Menú:**
- Dashboard
- Pedidos Internos
- Órdenes de Compra
- Adjudicaciones
- Reportes

**Funciones:**
- ✅ Crear pedidos internos
- ✅ Aprobar órdenes de compra
- ✅ Adjudicar pedidos
- ✅ Ver reportes

### 🚚 Administrador de Proveedor

**Menú:**
- Pedidos Disponibles
- Mis Ofertas
- Mi Perfil

**Funciones:**
- ✅ Ver pedidos disponibles (según su rubro)
- ✅ Ver competencia (precio mínimo del rubro)
- ✅ Registrar ofertas
- ✅ Ver estado de sus ofertas

### 🔍 Auditor

**Menú:**
- Dashboard
- Reportes
- Auditoría

**Funciones:**
- ✅ Solo lectura
- ✅ Ver reportes
- ✅ Ver logs de auditoría

### 📝 Creador de Pedidos

**Menú:**
- Pedidos Internos

**Funciones:**
- ✅ Crear pedidos internos
- ❌ NO puede aprobar órdenes
- ❌ NO puede adjudicar

## 🔄 Flujo Completo de Compra
┌─────────────────────────────────────────────────────────────┐
│ 1. CREADOR DE PEDIDOS crea un Pedido Interno │
│ → Estado: 🔵 Creado │
├─────────────────────────────────────────────────────────────┤
│ 2. GESTOR DE COMPRAS asigna el pedido a una Orden │
│ → Orden pasa a: 🟣 Aprobada │
│ → Al crear el 1er pedido, la Orden se publica: 🟡 Publicada│
├─────────────────────────────────────────────────────────────┤
│ 3. PROVEEDORES ven el pedido y envían ofertas │
│ → Pedido con ofertas │
├─────────────────────────────────────────────────────────────┤
│ 4. GESTOR DE COMPRAS adjudica al mejor precio │
│ → Adjudicación creada │
│ → Orden pasa a: 🟢 Adjudicada (automático) │
├─────────────────────────────────────────────────────────────┤
│ 5. AUDITOR genera reportes │
└─────────────────────────────────────────────────────────────┘


### Estados de un Pedido

| Estado | Descripción |
|--------|-------------|
| 🔵 **Creado** | Pedido recién creado |
| 🟠 **Sin Orden** | No tiene orden de compra asignada |
| 🟡 **Con Ofertas** | Proveedores ya ofertaron |
| 🟢 **Adjudicado** | Se seleccionó ganador |

### Estados de una Orden

| Estado | Descripción |
|--------|-------------|
| 🔵 **Borrador** | Recién creada |
| 🟣 **Aprobada** | Aprobada por Gestor |
| 🟡 **Publicada** | Visible para proveedores |
| 🟢 **Adjudicada** | Todos los pedidos adjudicados |
| 🔴 **Cancelada** | Cancelada por Admin |

## 📊 Reportes

### Cómo generar un reporte

1. Ve a **Admin → Reportes**.
2. Selecciona el reporte que quieres ver.
3. El PDF se genera en 1-2 segundos.
4. Botones disponibles:
   - 🔄 **Recargar** → Vuelve a generar el PDF
   - ⬇️ **Descargar** → Descarga el PDF

### Reportes disponibles

1. **Ranking de Proveedores** — Top 10 por monto adjudicado
2. **Gasto Departamental** — Gasto total por departamento
3. **Órdenes Activas** — Órdenes abiertas (fecha límite futura)
4. **Pedidos Pendientes** — Pedidos sin orden asignada
5. **Historial por Artículo** — Todas las compras de un artículo
6. **Ofertas por Orden** — Comparativa de precios por orden
7. **Eficiencia del Proceso** — Días promedio hasta adjudicación
8. **Variación de Precios** — Evolución de precios por proveedor

## ❓ Preguntas Frecuentes

### ¿Cómo cambio mi contraseña?

1. Ve a tu perfil (esquina superior derecha).
2. Clic en **"Cambiar contraseña"**.
3. Ingresa la contraseña actual y la nueva.
4. Guarda.

### ¿Cómo cambio entre Azure SQL y SQL Server Local?

1. **Cierra la aplicación.**
2. **Abre la app de nuevo.**
3. En la pantalla de selección de BD, elige la opción deseada.

### ¿Qué pasa si se va el internet?

- Si elegiste **Azure SQL** → La app no funcionará.
- Si elegiste **SQL Server Local** → La app sigue funcionando.

### ¿Por qué mi pedido no aparece para los proveedores?

Los proveedores **solo ven pedidos que coincidan con su rubro**. Si el pedido tiene artículos que el proveedor no maneja, no lo verá.

### ¿Puedo eliminar un pedido adjudicado?

**No.** Los pedidos adjudicados solo se pueden **editar**, no eliminar.

### ¿Cómo veo mis ofertas?

- Como Proveedor: **Menú → Mis Ofertas**
- Como Gestor: **Compras → Ofertas**

### ¿Los reportes se generan en el servidor o en mi PC?

Se generan **en el backend .NET** con **QuestPDF** (nativo, sin Java).

## 🆘 Soporte

Si tienes problemas:

1. Revisa este manual.
2. Consulta las [Preguntas Frecuentes](#preguntas-frecuentes).
3. Contacta al administrador del sistema.

---

📌 **Última actualización:** 07/10/2026