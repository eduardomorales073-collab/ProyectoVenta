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
import { MatBadgeModule } from '@angular/material/badge';
import { RouterLink } from '@angular/router';
import { ReporteService } from '../../../../services/reporte.service';
import { NotificacionService } from '../../../../services/notificacion';
import { OrdenCompraService, OrdenCompra } from '../../../../services/orden-compra.service';
import { OfertaComparativa } from '../../../../models/reporte.model';

@Component({
  selector: 'app-ofertas-orden',
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
    MatChipsModule,
    MatBadgeModule
  ],
  templateUrl: './ofertas-orden.html',
  styleUrl: './ofertas-orden.scss'
})
export class OfertasOrden implements OnInit {
  ordenes: OrdenCompra[] = [];
  ofertas: OfertaComparativa[] = [];
  displayedColumns: string[] = ['proveedor', 'precio', 'fechaOferta', 'estado'];
  cargando = false;
  form: FormGroup;
  buscado = false;
  ordenSeleccionada = '';
  precioGanador = 0;

  constructor(
    private fb: FormBuilder,
    private reporteService: ReporteService,
    private ordenCompraService: OrdenCompraService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      idOrden: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.cargarOrdenes();
  }

  cargarOrdenes(): void {
    this.ordenCompraService.listar().subscribe({
      next: (data) => {
        this.ordenes = data;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error al cargar órdenes: ${err.status}`);
      }
    });
  }

  buscar(): void {
    if (this.form.invalid) return;
    this.cargando = true;
    this.buscado = true;

    const idOrden = this.form.get('idOrden')?.value;
    const orden = this.ordenes.find(o => o.id === idOrden);
    this.ordenSeleccionada = orden ? `#${orden.id} - ${orden.descripcion}` : '';

    this.reporteService.ofertasPorOrden(idOrden).subscribe({
      next: (data) => {
        this.ofertas = data;
        const ganador = data.find(o => o.esGanador);
        this.precioGanador = ganador ? ganador.precio : 0;
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

  get ahorro(): number {
    if (this.ofertas.length < 2) return 0;
    const maxPrecio = Math.max(...this.ofertas.map(o => o.precio));
    return maxPrecio - this.precioGanador;
  }
}
