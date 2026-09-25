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

  // ===== COMÚN (cualquier usuario autenticado) =====
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
