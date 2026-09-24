import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { ArticulosComponent } from './articulos/articulos.component';
import { authGuard } from './guards/auth-guard';
import { adminGuard } from './guards/admin-guard';
import { empleadoGuard } from './guards/empleado-guard';
import { proveedorGuard } from './guards/proveedor-guard';

export const routes: Routes = [
  { path: 'login', component: Login },

  // Rutas de Administrador
  {
    path: 'admin',
    canActivate: [adminGuard],
    children: [
      { path: 'dashboard', component: ArticulosComponent },
      { path: 'articulos', component: ArticulosComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },

  // Rutas de Empleado
  {
    path: 'empleado',
    canActivate: [empleadoGuard],
    children: [
      { path: 'pedidos', component: ArticulosComponent },
      { path: 'articulos', component: ArticulosComponent },
      { path: '', redirectTo: 'pedidos', pathMatch: 'full' }
    ]
  },

  // Rutas de Proveedor
  {
    path: 'proveedor',
    canActivate: [proveedorGuard],
    children: [
      { path: 'ofertas', component: ArticulosComponent },
      { path: '', redirectTo: 'ofertas', pathMatch: 'full' }
    ]
  },

  // Rutas comunes
  {
    path: 'articulos',
    component: ArticulosComponent,
    canActivate: [authGuard]
  },

  // Raíz y 404
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
