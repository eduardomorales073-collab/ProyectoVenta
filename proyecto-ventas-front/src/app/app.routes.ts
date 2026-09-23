import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { ArticulosComponent } from './articulos/articulos.component';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'articulos', component: ArticulosComponent },
  { path: '', redirectTo: '/articulos', pathMatch: 'full' },
  { path: '**', redirectTo: '/articulos' }
];
