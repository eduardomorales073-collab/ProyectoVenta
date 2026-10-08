import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { APP_BASE_HREF } from '@angular/common';  // ← NUEVO

import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth-interceptor';
import { bdInterceptor } from './interceptors/bd.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),

    // ✅ PathLocationStrategy (default)
    provideRouter(routes),

    // ✅ Forzar base href a '/' (para que los recursos se pidan desde la raíz)
    { provide: APP_BASE_HREF, useValue: '/' },

    provideHttpClient(
      withInterceptors([
        authInterceptor,
        bdInterceptor
      ])
    ),
    provideAnimationsAsync()
  ]
};
