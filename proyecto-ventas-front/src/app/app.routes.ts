import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { ArticulosComponent } from './articulos/articulos.component';
import { AdminDashboardComponent } from './pages/admin/dashboard/dashboard';
import { EmpleadoPedidosComponent } from './pages/empleado/pedidos/pedidos';
import { ProveedorOfertasComponent } from './pages/proveedor/ofertas/ofertas';
import { authGuard } from './guards/auth-guard';
import { adminGuard } from './guards/admin-guard';
import { empleadoGuard } from './guards/empleado-guard';
import { proveedorGuard } from './guards/proveedor-guard';
import { auditorGuard } from './guards/auditor-guard';
import { UsuariosComponent } from './pages/admin/usuarios/usuarios';
import { SucursalesComponent } from './pages/admin/sucursales/sucursales';
import { DepartamentosComponent } from './pages/admin/departamentos/departamentos';
import { RubrosComponent } from './pages/admin/rubros/rubros';
import { ProveedoresComponent } from './pages/admin/proveedores/proveedores';
import { RolesComponent } from './pages/admin/roles/roles';
import { TiposOrdenComponent } from './pages/admin/tipos-orden/tipos-orden';
import { PerfilComponent } from './pages/perfil/perfil';
import { ReportesComponent } from './pages/admin/reportes/reportes';
import { OrdenesActivas } from './pages/admin/reportes/ordenes-activas/ordenes-activas';
import { PedidosPendientes } from './pages/admin/reportes/pedidos-pendientes/pedidos-pendientes';
import { EficienciaProceso } from './pages/admin/reportes/eficiencia-proceso/eficiencia-proceso';
import { RankingProveedores } from './pages/admin/reportes/ranking-proveedores/ranking-proveedores';
import { HistorialArticulo } from './pages/admin/reportes/historial-articulo/historial-articulo';
import { OfertasOrden } from './pages/admin/reportes/ofertas-orden/ofertas-orden';
import { GastoDepartamentalComponent } from './pages/admin/reportes/gasto-departamental/gasto-departamental';
import { VariacionPreciosComponent } from './pages/admin/reportes/variacion-precios/variacion-precios';
import { AdjudicacionesComponent } from './pages/admin/adjudicaciones/adjudicaciones';
import { PedidosInternosComponent } from './pages/admin/pedidos-internos/pedidos-internos';
import { AuditorReportesComponent } from './pages/auditor/reportes/reportes';
import { CategoriasProveedorComponent } from './pages/admin/categorias-proveedor/categorias-proveedor';
import { UnidadesMedidaComponent } from './pages/admin/unidades-medida/unidades-medida';
import { AccesoDenegadoComponent } from './pages/acceso-denegado/acceso-denegado';
import { PedidosDisponiblesComponent } from './pages/proveedor/pedidos-disponibles/pedidos-disponibles';
import { creadorGuard } from './guards/creador-guard';

export const routes: Routes = [
  { path: 'login', component: Login },

  // ===== ADMINISTRADOR =====
  {
    path: 'admin',
    canActivate: [adminGuard],
    children: [
      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'articulos', component: ArticulosComponent },
      { path: 'usuarios', component: UsuariosComponent },
      { path: 'sucursales', component: SucursalesComponent },
      { path: 'departamentos', component: DepartamentosComponent },
      { path: 'rubros', component: RubrosComponent },
      { path: 'proveedores', component: ProveedoresComponent },
      { path: 'roles', component: RolesComponent },
      { path: 'tipos-orden', component: TiposOrdenComponent },
      { path: 'unidades-medida', component: UnidadesMedidaComponent },
      { path: 'categorias-proveedor', component: CategoriasProveedorComponent },

      // ===== REPORTES =====
      { path: 'reportes', component: ReportesComponent },
      { path: 'reportes/ordenes-activas', component: OrdenesActivas },
      { path: 'reportes/pedidos-pendientes', component: PedidosPendientes },
      { path: 'reportes/eficiencia-proceso', component: EficienciaProceso },
      { path: 'reportes/ranking-proveedores', component: RankingProveedores },
      { path: 'reportes/historial-articulo', component: HistorialArticulo },
      { path: 'reportes/ofertas-orden', component: OfertasOrden },
      { path: 'reportes/gasto-departamental', component: GastoDepartamentalComponent },
      { path: 'reportes/variacion-precios', component: VariacionPreciosComponent },
      { path: 'adjudicaciones', component: AdjudicacionesComponent },
      { path: 'pedidos-internos', component: PedidosInternosComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

      { path: 'acceso-denegado', component: AccesoDenegadoComponent },
    ]
  },
  // ===== CREADOR DE PEDIDOS =====
{
  path: 'creador',
  canActivate: [creadorGuard],
  children: [
    { path: 'pedidos', component: PedidosInternosComponent },
    { path: '', redirectTo: 'pedidos', pathMatch: 'full' }
  ]
},

   // ===== AUDITOR =====
  {
    path: 'auditor',
    canActivate: [auditorGuard],
    children: [
      { path: 'reportes', component: AuditorReportesComponent },
      { path: 'reportes/ordenes-activas', component: OrdenesActivas },
      { path: 'reportes/pedidos-pendientes', component: PedidosPendientes },
      { path: 'reportes/eficiencia-proceso', component: EficienciaProceso },
      { path: 'reportes/ranking-proveedores', component: RankingProveedores },
      { path: 'reportes/historial-articulo', component: HistorialArticulo },
      { path: 'reportes/ofertas-orden', component: OfertasOrden },
      { path: 'reportes/gasto-departamental', component: GastoDepartamentalComponent },
      { path: 'reportes/variacion-precios', component: VariacionPreciosComponent },
      { path: '', redirectTo: 'reportes', pathMatch: 'full' }
    ]
  },

  // ===== EMPLEADO (Gestor de Compras) =====
{
  path: 'empleado',
  canActivate: [empleadoGuard],
  children: [
    { path: 'pedidos', component: PedidosInternosComponent },        // ← Reutiliza componente del Admin
    { path: 'adjudicaciones', component: AdjudicacionesComponent },  // ← Reutiliza componente del Admin
    { path: 'articulos', component: ArticulosComponent },
    { path: 'ordenes-activas', component: OrdenesActivas },
    { path: 'pedidos-pendientes', component: PedidosPendientes },
    { path: 'reportes/eficiencia-proceso', component: EficienciaProceso },
    { path: 'reportes/ranking-proveedores', component: RankingProveedores },
    { path: 'reportes/historial-articulo', component: HistorialArticulo },
    { path: 'reportes/ofertas-orden', component: OfertasOrden },
    { path: 'reportes/gasto-departamental', component: GastoDepartamentalComponent },
    { path: 'reportes/variacion-precios', component: VariacionPreciosComponent },
    { path: '', redirectTo: 'pedidos', pathMatch: 'full' }
  ]
},

  // ===== PROVEEDOR (Admin de Proveedor) =====
  {
    path: 'proveedor',
    canActivate: [proveedorGuard],
    children: [
      { path: 'ofertas', component: ProveedorOfertasComponent },
      { path: 'articulos', component: ArticulosComponent },
      { path: 'pedidos-disponibles', component: PedidosDisponiblesComponent },
      { path: '', redirectTo: 'ofertas', pathMatch: 'full' }
    ]
  },

  // ===== COMÚN =====
  {
    path: 'articulos',
    component: ArticulosComponent,
    canActivate: [authGuard]
  },
  {
    path: 'perfil',
    component: PerfilComponent,
    canActivate: [authGuard]
  },

  // ===== RAÍZ Y 404 =====
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];