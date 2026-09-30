import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { JasperService } from '../../../../services/jasper.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-reporte-jasper',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule],
  templateUrl: './reporte-jasper.html',
  styleUrl: './reporte-jasper.scss'
})
export class ReporteJasperComponent implements OnInit {
  reporte: any;
  urlSegura!: SafeResourceUrl;
  cargando = true;
  rutaVolver = '/admin/reportes';

  constructor(
    private route: ActivatedRoute,
    private jasperService: JasperService,
    private authService: AuthService,
    private sanitizer: DomSanitizer,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    const reporte = this.jasperService.reportes.find(r => r.id === id);

    if (reporte) {
      this.reporte = reporte;
      const url = this.jasperService.getUrlReporte(reporte.rutaJasper);
      this.urlSegura = this.sanitizer.bypassSecurityTrustResourceUrl(url);

      setTimeout(() => {
        this.cargando = false;
        this.cdr.detectChanges();
      }, 3000);
    } else {
      this.rutaVolver = this.authService.getRutaHubReportes();
    }
  }

  recargar(): void {
    if (!this.reporte) return;
    this.cargando = true;
    const url = this.jasperService.getUrlReporte(this.reporte.rutaJasper);
    this.urlSegura = this.sanitizer.bypassSecurityTrustResourceUrl(url);
    setTimeout(() => {
      this.cargando = false;
      this.cdr.detectChanges();
    }, 2000);
  }
}