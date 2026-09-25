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
import { ProveedorService } from '../../../../services/proveedor.service';
import { Proveedor } from '../../../../models/proveedor.model';
import { VariacionPrecio } from '../../../../models/reporte.model';

@Component({
  selector: 'app-variacion-precios',
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
  templateUrl: './variacion-precios.html',
  styleUrl: './variacion-precios.scss'
})
export class VariacionPreciosComponent implements OnInit {
  articulos: Articulo[] = [];
  proveedores: Proveedor[] = [];
  variaciones: VariacionPrecio[] = [];
  displayedColumns: string[] = ['fecha', 'precio', 'variacion'];
  cargando = false;
  form: FormGroup;
  buscado = false;
  articuloSeleccionado = '';
  proveedorSeleccionado = '';

  constructor(
    private fb: FormBuilder,
    private reporteService: ReporteService,
    private articuloService: ArticuloService,
    private proveedorService: ProveedorService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      idArticulo: ['', [Validators.required]],
      idProveedor: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.cargarArticulos();
    this.cargarProveedores();
  }

  cargarArticulos(): void {
    this.articuloService.listar().subscribe({
      next: (data: Articulo[]) => {
        this.articulos = data;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error al cargar artículos: ${err.status}`);
      }
    });
  }

  cargarProveedores(): void {
    this.proveedorService.listar().subscribe({
      next: (data: Proveedor[]) => {
        this.proveedores = data;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error al cargar proveedores: ${err.status}`);
      }
    });
  }

  buscar(): void {
    if (this.form.invalid) return;
    this.cargando = true;
    this.buscado = true;

    const { idArticulo, idProveedor } = this.form.value;
    this.articuloSeleccionado = this.articulos.find(a => a.id === idArticulo)?.nombre || '';
    this.proveedorSeleccionado = this.proveedores.find(p => p.id === idProveedor)?.nombre || '';

    this.reporteService.variacionPrecios(idArticulo, idProveedor).subscribe({
      next: (data: VariacionPrecio[]) => {
        this.variaciones = data;
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

  getPrecioAnterior(index: number): number | null {
    if (index === 0) return null;
    return this.variaciones[index - 1].precio;
  }

  getVariacion(index: number): number {
    const anterior = this.getPrecioAnterior(index);
    if (anterior === null) return 0;
    return this.variaciones[index].precio - anterior;
  }

  getVariacionPorcentaje(index: number): number {
    const anterior = this.getPrecioAnterior(index);
    if (anterior === null || anterior === 0) return 0;
    return ((this.variaciones[index].precio - anterior) / anterior) * 100;
  }

  getTendencia(index: number): 'sube' | 'baja' | 'igual' {
    const variacion = this.getVariacion(index);
    if (variacion > 0) return 'sube';
    if (variacion < 0) return 'baja';
    return 'igual';
  }

  get precioMinimo(): number {
    if (this.variaciones.length === 0) return 0;
    return Math.min(...this.variaciones.map(v => v.precio));
  }

  get precioMaximo(): number {
    if (this.variaciones.length === 0) return 0;
    return Math.max(...this.variaciones.map(v => v.precio));
  }

  get precioPromedio(): number {
    if (this.variaciones.length === 0) return 0;
    return this.variaciones.reduce((sum, v) => sum + v.precio, 0) / this.variaciones.length;
  }
}
