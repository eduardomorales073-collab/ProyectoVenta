import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// ✅ Rutas que NO deben llevar token
const RUTAS_PUBLICAS = [
  '/AuthControlador/login',
  '/AuthControlador/register',
  '/AuthControlador/refresh'
];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // ✅ Si la URL es una ruta pública, NO añadir token
  const esRutaPublica = RUTAS_PUBLICAS.some(ruta => req.url.includes(ruta));
  
  if (esRutaPublica) {
    console.log(`[AuthInterceptor] Ruta pública, sin token: ${req.url}`);
    return next(req);
  }

  const authService = inject(AuthService);
  const token = authService.getToken();

  // ✅ Solo añadir el header si hay token Y no está vacío
  if (token && token.trim() !== '') {
    const authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    return next(authReq);
  }

  return next(req);
};