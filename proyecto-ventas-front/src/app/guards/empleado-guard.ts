import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const empleadoGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (authService.esGestorCompras() || authService.esAdmin()) return true;
  router.navigate(['/acceso-denegado']);
  return false;
};