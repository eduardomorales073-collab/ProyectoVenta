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
import { AdjudicacionesComponent } from './pages/admin/adjudicaciones/adjudicaciones';
import { PedidosInternosComponent } from './pages/admin/pedidos-internos/pedidos-internos';
import { AuditorReportesComponent } from './pages/auditor/reportes/reportes';
import { CategoriasProveedorComponent } from './pages/admin/categorias-proveedor/categorias-proveedor';
import { UnidadesMedidaComponent } from './pages/admin/unidades-medida/unidades-medida';
import { AccesoDenegadoComponent } from './pages/acceso-denegado/acceso-denegado';
import { PedidosDisponiblesComponent } from './pages/proveedor/pedidos-disponibles/pedidos-disponibles';
import { creadorGuard } from './guards/creador-guard';
import { AuditoriaComponent } from './pages/admin/auditoria/auditoria';
import { OrdenesCompraComponent } from './pages/admin/ordenes-compra/ordenes-compra';
import { SeleccionBdComponent } from './pages/seleccion-bd/seleccion-bd';

// ✅ NUEVO: Componente de reportes PDF (QuestPDF)
import { ReportePdfComponent } from './pages/admin/reportes/reporte-pdf/reporte-pdf';

export const routes: Routes = [
  // ✅ PRIMERA RUTA: Selección de BD
  { path: 'seleccion-bd', component: SeleccionBdComponent },

  // ✅ LOGIN
  { path: 'login', component: Login },

  // ✅ RAÍZ: Redirige a selección BD
  { path: '', redirectTo: '/seleccion-bd', pathMatch: 'full' },

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
      { path: 'ordenes-compra', component: OrdenesCompraComponent },
      { path: 'reportes', component: ReportesComponent },
      // ✅ NUEVO: Ruta de reportes PDF (QuestPDF)
      { path: 'reportes/pdf/:id', component: ReportePdfComponent },
      { path: 'adjudicaciones', component: AdjudicacionesComponent },
      { path: 'pedidos-internos', component: PedidosInternosComponent },
      { path: 'auditoria', component: AuditoriaComponent },
      { path: 'acceso-denegado', component: AccesoDenegadoComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },

  // ===== CREADOR DE PEDIDOS =====
  {
    path: 'creador',
    canActivate: [creadorGuard],
    children: [
      { path: 'ordenes-compra', component: OrdenesCompraComponent },
      { path: 'pedidos', component: PedidosInternosComponent },
      { path: '', redirectTo: 'ordenes-compra', pathMatch: 'full' }
    ]
  },

  // ===== AUDITOR =====
  {
    path: 'auditor',
    canActivate: [auditorGuard],
    children: [
      { path: 'reportes', component: AuditorReportesComponent },
      // ✅ NUEVO: Ruta de reportes PDF
      { path: 'reportes/pdf/:id', component: ReportePdfComponent },
      { path: 'auditoria', component: AuditoriaComponent },
      { path: '', redirectTo: 'reportes', pathMatch: 'full' }
    ]
  },

  // ===== EMPLEADO (Gestor de Compras) =====
  {
    path: 'empleado',
    canActivate: [empleadoGuard],
    children: [
      { path: 'pedidos', component: PedidosInternosComponent },
      { path: 'ordenes-compra', component: OrdenesCompraComponent },
      { path: 'adjudicaciones', component: AdjudicacionesComponent },
      { path: 'articulos', component: ArticulosComponent },
      { path: 'reportes', component: ReportesComponent },
      // ✅ NUEVO: Ruta de reportes PDF
      { path: 'reportes/pdf/:id', component: ReportePdfComponent },
      { path: 'auditoria', component: AuditoriaComponent },
      { path: '', redirectTo: 'pedidos', pathMatch: 'full' }
    ]
  },

  // ===== PROVEEDOR =====
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
  { path: 'articulos', component: ArticulosComponent, canActivate: [authGuard] },
  { path: 'perfil', component: PerfilComponent, canActivate: [authGuard] },

  // ===== 404 =====
  { path: '**', redirectTo: '/login' }
];