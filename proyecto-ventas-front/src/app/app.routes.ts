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

      // ===== REPORTES =====
      { path: 'reportes', component: ReportesComponent },
      { path: 'reportes/ordenes-activas', component: OrdenesActivas },
      { path: 'reportes/pedidos-pendientes', component: PedidosPendientes },
      { path: 'reportes/eficiencia-proceso', component: EficienciaProceso },
      { path: 'reportes/ranking-proveedores', component: RankingProveedores },

      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },

  // ===== EMPLEADO =====
  {
    path: 'empleado',
    canActivate: [empleadoGuard],
    children: [
      { path: 'pedidos', component: EmpleadoPedidosComponent },
      { path: 'articulos', component: ArticulosComponent },
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
