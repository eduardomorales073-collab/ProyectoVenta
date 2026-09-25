import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { RouterLink } from '@angular/router';
import { ReporteService } from '../../../../services/reporte.service';
import { NotificacionService } from '../../../../services/notificacion';
import { ArticuloService, Articulo } from '../../../../services/articulo.service';
import { HistorialCompra } from '../../../../models/reporte.model';

@Component({
  selector: 'app-historial-articulo',
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
    MatSelectModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './historial-articulo.html',
  styleUrl: './historial-articulo.scss'
})
export class HistorialArticulo implements OnInit {
  articulos: Articulo[] = [];
  historial: HistorialCompra[] = [];
  displayedColumns: string[] = ['fechaResolucion', 'proveedor', 'precio'];
  cargando = false;
  form: FormGroup;
  buscado = false;
  articuloSeleccionado: string = '';

  constructor(
    private fb: FormBuilder,
    private reporteService: ReporteService,
    private articuloService: ArticuloService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      idArticulo: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.cargarArticulos();
  }

  cargarArticulos(): void {
    this.articuloService.listar().subscribe({
      next: (data) => {
        this.articulos = data;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error al cargar artículos: ${err.status}`);
      }
    });
  }

  buscar(): void {
    if (this.form.invalid) return;
    this.cargando = true;
    this.buscado = true;

    const idArticulo = this.form.get('idArticulo')?.value;
    this.articuloSeleccionado = this.articulos.find(a => a.id === idArticulo)?.nombre || '';

    this.reporteService.historialPorArticulo(idArticulo).subscribe({
      next: (data) => {
        this.historial = data;
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

  get totalGastado(): number {
    return this.historial.reduce((sum, h) => sum + h.precio, 0);
  }
}
