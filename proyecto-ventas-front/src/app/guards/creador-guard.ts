import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const creadorGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.esCreadorPedidos() || authService.esAdmin()) return true;

  router.navigate(['/acceso-denegado']);
  return false;
};