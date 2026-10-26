import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { JasperService, ReporteJasper } from '../../../../services/jasper.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-reporte-jasper',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSnackBarModule
  ],
  templateUrl: './reporte-jasper.html',
  styleUrl: './reporte-jasper.scss'
})
export class ReporteJasperComponent implements OnInit {
  reporte!: ReporteJasper;
  urlSegura!: SafeResourceUrl;
  cargando = true;
  descargando = false;
  error = '';
  rutaVolver: string = '/login';

  // ✅ Propiedades para el header formal
  fechaActual = new Date();
  anioActual = new Date().getFullYear();
  nombreUsuario = 'Usuario';

  private readonly jasperBaseUrl = 'http://localhost:8080/jasperserver-pro';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jasperService: JasperService,
    private authService: AuthService,
    private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef,
    private snackBar: MatSnackBar
  ) { }

  async ngOnInit(): Promise<void> {
    // ✅ Determinar la ruta de regreso según el rol del usuario
    if (this.authService.esAdmin()) {
      this.rutaVolver = '/admin/reportes';
    } else if (this.authService.esAuditor()) {
      this.rutaVolver = '/auditor/reportes';
    } else if (this.authService.esGestorCompras()) {
      this.rutaVolver = '/empleado/reportes';
    } else {
      this.rutaVolver = '/login';
    }

    // ✅ Obtener el nombre del usuario
    this.nombreUsuario = this.authService.getNombreUsuario() || 'Usuario';

    // Obtener el reporte por id
    const id = this.route.snapshot.paramMap.get('id');
    const reporte = this.jasperService.reportes.find((r: ReporteJasper) => r.id === id);

    if (!reporte) {
      this.error = 'Reporte no encontrado';
      this.cargando = false;
      return;
    }

    this.reporte = reporte;

    await this.hacerLoginJasper();
    this.cargarIframe();
  }

  /**
   * ✅ Hacer login en JasperReports para obtener la cookie de sesión.
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
          credentials: 'include',
          redirect: 'manual'
        }
      );

      console.log('[ReporteJasper] Login status:', response.status);
    } catch (err) {
      console.error('[ReporteJasper] Error en login Jasper:', err);
    }
  }

  /**
   * ✅ Cargar el iframe con la URL del reporte.
   */
  private cargarIframe(): void {
    const url = this.jasperService.getUrlReporte(this.reporte.rutaJasper);
    console.log('[ReporteJasper] URL del iframe:', url);
    this.urlSegura = this.sanitizer.bypassSecurityTrustResourceUrl(url);

    setTimeout(() => {
      this.cargando = false;
      this.cdr.detectChanges();
    }, 3000);
  }

  /**
   * ✅ Volver al hub de Reportes
   */
  volver(): void {
    this.router.navigate([this.rutaVolver]);
  }

  /**
   * ✅ Recargar el reporte
   */
  recargar(): void {
    if (!this.reporte) return;
    this.cargando = true;
    this.error = '';

    this.hacerLoginJasper().then(() => {
      setTimeout(() => {
        this.cargarIframe();
      }, 500);
    });
  }

  /**
   * ✅ Descargar el reporte como PDF
   */
  descargar(): void {
    if (!this.reporte) return;

    const urlPdf = this.jasperService.getUrlPdf(this.reporte.rutaJasper);
    console.log('[ReporteJasper] Abriendo PDF:', urlPdf);

    window.open(urlPdf, '_blank');

    this.snackBar.open('📄 PDF abierto en nueva pestaña', 'Cerrar', { duration: 3000 });
  }
}
