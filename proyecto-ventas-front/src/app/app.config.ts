import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth-interceptor';
import { bdInterceptor } from './interceptors/bd.interceptor';

export const appConfig: ApplicationConfig = {   // ← minúscula (a)
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, bdInterceptor])),
    provideAnimationsAsync()
  ]
};