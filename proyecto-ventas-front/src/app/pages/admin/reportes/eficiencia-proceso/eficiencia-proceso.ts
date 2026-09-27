import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { ReporteService } from '../../../../services/reporte.service';
import { NotificacionService } from '../../../../services/notificacion';
import { AuthService } from '../../../../services/auth.service';
import { EficienciaCompra } from '../../../../models/reporte.model';

@Component({
  selector: 'app-eficiencia-proceso',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './eficiencia-proceso.html',
  styleUrl: './eficiencia-proceso.scss'
})
export class EficienciaProceso implements OnInit {
  eficiencia: EficienciaCompra | null = null;
  cargando = false;
  rutaVolver: string = '/admin/reportes';

  constructor(
    private reporteService: ReporteService,
    private notificacion: NotificacionService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.rutaVolver = this.authService.getRutaHubReportes();
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.reporteService.eficienciaProceso().subscribe({
      next: (data) => {
        this.eficiencia = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  get colorPromedio(): string {
    if (!this.eficiencia) return '#64748b';
    const dias = this.eficiencia.promedioDias;
    if (dias <= 7) return '#10b981';
    if (dias <= 15) return '#f59e0b';
    return '#ef4444';
  }

  get descripcionPromedio(): string {
    if (!this.eficiencia) return '';
    const dias = this.eficiencia.promedioDias;
    if (dias <= 7) return '¡Excelente! El proceso es muy rápido.';
    if (dias <= 15) return 'El proceso tiene una duración normal.';
    return 'El proceso es lento. Considera optimizar los tiempos.';
  }
}