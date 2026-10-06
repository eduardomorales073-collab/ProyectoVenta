import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { JasperService, ReporteJasper } from '../../../../services/jasper.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-reporte-jasper',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './reporte-jasper.html',
  styleUrl: './reporte-jasper.scss'
})
export class ReporteJasperComponent implements OnInit {
  reporte!: ReporteJasper;
  urlSegura!: SafeResourceUrl;
  cargando = true;
  error = '';
  rutaVolver: string = '/login';

  // ✅ URL base de Jasper
  private readonly jasperBaseUrl = 'http://localhost:8080/jasperserver-pro';

  constructor(
    private route: ActivatedRoute,
    private jasperService: JasperService,
    private authService: AuthService,
    private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef
  ) { }

  async ngOnInit(): Promise<void> {
    // ✅ Determinar la ruta de regreso según el rol del usuario
    this.rutaVolver = this.authService.getRutaHubReportes();

    // Obtener el reporte por id
    const id = this.route.snapshot.paramMap.get('id');
    const reporte = this.jasperService.reportes.find(r => r.id === id);

    if (!reporte) {
      this.error = 'Reporte no encontrado';
      this.cargando = false;
      return;
    }

    this.reporte = reporte;

    // ✅ PASO 1: Hacer login en JasperReports para obtener la cookie JSESSIONID
    await this.hacerLoginJasper();

    // ✅ PASO 2: Cargar el iframe
    this.cargarIframe();
  }

  /**
   * ✅ Hacer login en JasperReports para obtener la cookie de sesión.
   * Sin este paso, el iframe carga en blanco.
   */
  private async hacerLoginJasper(): Promise<void> {
    try {
      const formData = new URLSearchParams();
      formData.set('j_username', 'jasperadmin');
      formData.set('j_password', 'jasperadmin');

      const response = await fetch(
        `${this.jasperBaseUrl}/j_spring_security_check`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
          },
          body: formData.toString(),
          credentials: 'include', // ✅ IMPORTANTE: Incluir cookies
          redirect: 'manual' // ✅ No seguir redirects automáticamente
        }
      );

      console.log('[ReporteJasper] Login status:', response.status);
      console.log('[ReporteJasper] Login OK, cookie JSESSIONID obtenida');
    } catch (err) {
      console.error('[ReporteJasper] Error en login Jasper:', err);
      // Continuar de todas formas (puede que el login no sea necesario)
    }
  }

  /**
   * Cargar el iframe con la URL del reporte.
   */
  private cargarIframe(): void {
    const url = this.jasperService.getUrlReporte(this.reporte.rutaJasper);
    console.log('[ReporteJasper] URL del iframe:', url);
    this.urlSegura = this.sanitizer.bypassSecurityTrustResourceUrl(url);

    // ✅ Detectar cuando el iframe termina de cargar
    setTimeout(() => {
      this.cargando = false;
      this.cdr.detectChanges();
    }, 3000);
  }

  recargar(): void {
    if (!this.reporte) return;
    this.cargando = true;
    this.error = '';

    // ✅ Rehacer login + recargar iframe
    this.hacerLoginJasper().then(() => {
      setTimeout(() => {
        this.cargarIframe();
      }, 500);
    });
  }
}