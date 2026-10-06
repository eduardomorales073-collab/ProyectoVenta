import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../services/auth.service';

interface ReportePdf {
  id: string;
  titulo: string;
  descripcion: string;
  icono: string;
  endpoint: string;
  color: string;
}

@Component({
  selector: 'app-reporte-pdf',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './reporte-pdf.html',
  styleUrl: './reporte-pdf.scss'
})
export class ReportePdfComponent implements OnInit, OnDestroy {
  reporte!: ReportePdf;
  urlPdf!: SafeResourceUrl;
  cargando = true;
  error = '';
  rutaVolver: string = '/login';

  // ✅ Blob URL local (evita que Electron descargue el PDF)
  private blobUrl: string | null = null;

  private readonly apiUrl = 'http://localhost:5000/api/ReporteControlador/pdf';

  private readonly reportes: ReportePdf[] = [
    { id: 'ranking-proveedores', titulo: 'Ranking de Proveedores', descripcion: 'Top proveedores con mayores montos adjudicados.', icono: 'leaderboard', endpoint: 'ranking-proveedores', color: 'gradient-green' },
    { id: 'gasto-departamental', titulo: 'Gasto Departamental', descripcion: 'Monto total gastado por departamento y sucursal.', icono: 'account_balance', endpoint: 'gasto-departamental', color: 'gradient-red' },
    { id: 'ordenes-activas', titulo: 'Órdenes Activas', descripcion: 'Órdenes de compra abiertas a recibir ofertas.', icono: 'play_circle', endpoint: 'ordenes-activas', color: 'gradient-cyan' },
    { id: 'pedidos-pendientes', titulo: 'Pedidos Pendientes', descripcion: 'Pedidos internos sin asignar a una orden.', icono: 'pending_actions', endpoint: 'pedidos-pendientes', color: 'gradient-purple' },
    { id: 'historial-articulo', titulo: 'Historial por Artículo', descripcion: 'Compras de un artículo con proveedor y precio.', icono: 'history', endpoint: 'historial-articulo', color: 'gradient-blue' },
    { id: 'ofertas-orden', titulo: 'Ofertas por Orden', descripcion: 'Comparativa de ofertas recibidas por orden.', icono: 'compare_arrows', endpoint: 'ofertas-orden', color: 'gradient-orange' },
    { id: 'eficiencia-proceso', titulo: 'Eficiencia del Proceso', descripcion: 'Tiempo promedio desde orden hasta adjudicación.', icono: 'speed', endpoint: 'eficiencia-proceso', color: 'gradient-teal' },
    { id: 'variacion-precios', titulo: 'Variación de Precios', descripcion: 'Evolución de precios por artículo y proveedor.', icono: 'trending_up', endpoint: 'variacion-precios', color: 'gradient-pink' }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private authService: AuthService,
    private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.rutaVolver = this.authService.getRutaHubReportes();

    const id = this.route.snapshot.paramMap.get('id');
    const reporte = this.reportes.find(r => r.id === id);

    if (!reporte) {
      this.rutaVolver = this.authService.getRutaHubReportes();
      this.cargando = false;
      return;
    }

    this.reporte = reporte;
    this.cargarPdf();
  }

  ngOnDestroy(): void {
    // ✅ Limpiar blob URL al destruir el componente
    if (this.blobUrl) {
      URL.revokeObjectURL(this.blobUrl);
      this.blobUrl = null;
    }
  }

  /**
   * ✅ Descarga el PDF como Blob y lo muestra en un iframe con URL local.
   * Esto EVITA que Electron descargue el PDF automáticamente.
   */
  private cargarPdf(): void {
    this.cargando = true;
    this.error = '';

    // Limpiar blob URL anterior
    if (this.blobUrl) {
      URL.revokeObjectURL(this.blobUrl);
      this.blobUrl = null;
    }

    const url = `${this.apiUrl}/${this.reporte.endpoint}?t=${Date.now()}`;

    console.log('[ReportePdf] Descargando PDF:', url);

    // ✅ CRÍTICO: responseType 'blob'
    this.http.get(url, { responseType: 'blob' }).subscribe({
      next: (blob: Blob) => {
        console.log('[ReportePdf] PDF recibido:', blob.size, 'bytes');

        // ✅ Crear URL local con el blob
        this.blobUrl = URL.createObjectURL(blob);

        // ✅ Sanitizar para el iframe
        this.urlPdf = this.sanitizer.bypassSecurityTrustResourceUrl(this.blobUrl);

        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('[ReportePdf] Error al cargar PDF:', err);
        this.error = 'No se pudo generar el PDF';
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  recargar(): void {
    this.cargarPdf();
  }

  descargar(): void {
    if (!this.reporte) return;
    // Descargar el PDF original (desde el backend)
    const url = `${this.apiUrl}/${this.reporte.endpoint}`;
    window.open(url, '_blank');
  }

  volver(): void {
    this.router.navigate([this.rutaVolver]);
  }
}