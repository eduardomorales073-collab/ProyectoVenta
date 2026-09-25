import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { RouterLink } from '@angular/router';
import { ReporteService } from '../../../../services/reporte.service';
import { NotificacionService } from '../../../../services/notificacion';
import { RankingProveedor } from '../../../../models/reporte.model';

@Component({
  selector: 'app-ranking-proveedores',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './ranking-proveedores.html',
  styleUrl: './ranking-proveedores.scss'
})
export class RankingProveedores implements OnInit {
  ranking: RankingProveedor[] = [];
  displayedColumns: string[] = ['posicion', 'nombre', 'totalAdjudicado'];
  cargando = false;
  formFiltro: FormGroup;

  constructor(
    private fb: FormBuilder,
    private reporteService: ReporteService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    const hoy = new Date();
    const inicioAnio = new Date(hoy.getFullYear(), 0, 1);

    this.formFiltro = this.fb.group({
      desde: [inicioAnio.toISOString().substring(0, 10)],
      hasta: [hoy.toISOString().substring(0, 10)]
    });
  }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    const { desde, hasta } = this.formFiltro.value;

    this.reporteService.rankingProveedores(desde, hasta).subscribe({
      next: (data) => {
        this.ranking = data;
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

  getMedalla(posicion: number): string {
    switch (posicion) {
      case 1: return '🥇';
      case 2: return '🥈';
      case 3: return '🥉';
      default: return `#${posicion}`;
    }
  }
}
