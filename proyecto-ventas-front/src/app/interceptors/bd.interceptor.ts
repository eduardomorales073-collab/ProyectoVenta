import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BdService } from '../services/bd.service';

export const bdInterceptor: HttpInterceptorFn = (req, next) => {
  const bdService = inject(BdService);
  const tipoBd = bdService.getTipoBd();

  // ✅ Añadir header X-Tipo-BD a TODAS las peticiones al backend
  if (req.url.includes('localhost:5000') || req.url.includes('/api/')) {
    const authReq = req.clone({
      headers: req.headers.set('X-Tipo-BD', tipoBd)
    });
    return next(authReq);
  }

  return next(req);
};