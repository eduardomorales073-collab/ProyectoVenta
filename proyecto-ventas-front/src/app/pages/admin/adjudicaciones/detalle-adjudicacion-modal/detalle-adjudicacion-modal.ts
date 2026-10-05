import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';

import { DetalleAdjudicacionService } from '../../../../services/detalle-adjudicacion.service';
import { PedidoInternoService } from '../../../../services/pedido-interno.service';
import { OfertaProveedorService } from '../../../../services/oferta-proveedor.service';
import { ProveedorService } from '../../../../services/proveedor.service';
import { NotificacionService } from '../../../../services/notificacion';
import { ConfirmService } from '../../../../services/confirm';

import { DetalleAdjudicacion } from '../../../../models/detalle-adjudicacion.model';
import { Adjudicacion } from '../../../../models/adjudicacion.model';
import { PedidoInterno } from '../../../../models/pedido-interno.model';
import { Proveedor } from '../../../../models/proveedor.model';
import { OfertaProveedor } from '../../../../models/oferta-proveedor.model';

@Component({
  selector: 'app-detalle-adjudicacion-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './detalle-adjudicacion-modal.html',
  styleUrl: './detalle-adjudicacion-modal.scss'
})
export class DetalleAdjudicacionModalComponent implements OnInit {
  @Input() adjudicacion!: Adjudicacion;
  @Output() cerrar = new EventEmitter<void>();

  // Datos
  detalles: DetalleAdjudicacion[] = [];
  pedidosDisponibles: PedidoInterno[] = [];
  proveedores: Proveedor[] = [];
  proveedoresDelPedido: Proveedor[] = [];
  pedidos: PedidoInterno[] = [];
  ofertasDelPedidoActual: OfertaProveedor[] = [];

  // Formulario para agregar
  form: FormGroup;
  mostrarFormulario = false;
  guardando = false;
  cargando = false;
  cargandoProveedores = false;

  // Precio auto-llenado
  precioSugerido = 0;

  // Columnas de la tabla
  displayedColumns: string[] = ['pedido', 'proveedor', 'precio', 'cantidad', 'acciones'];

  constructor(
    private fb: FormBuilder,
    private detalleService: DetalleAdjudicacionService,
    private pedidoService: PedidoInternoService,
    private ofertaService: OfertaProveedorService,
    private proveedorService: ProveedorService,
    private notificacion: NotificacionService,
    private confirm: ConfirmService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      id_Pedido: ['', [Validators.required]],
      id_Proveedor: ['', [Validators.required]],
      precio: ['', [Validators.required, Validators.min(0.01)]],
      cantidad: [1, [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.cargarTodo();
    this.escucharCambiosPedido();
  }

  cargarTodo(): void {
    this.cargando = true;

    forkJoin({
      detalles: this.detalleService.listar(),
      pedidos: this.pedidoService.listar(),
      proveedores: this.proveedorService.listar()
    }).subscribe({
      next: (result) => {
        // Detalles de ESTA adjudicación (para mostrar en la tabla)
        this.detalles = result.detalles.filter(
          (d: DetalleAdjudicacion) => d.id_Adjudicacion === this.adjudicacion.id
        );

        // Excluir TODOS los pedidos adjudicados (en cualquier adjudicación)
        const idsPedidosYaAdjudicados = result.detalles.map(
          (d: DetalleAdjudicacion) => d.id_Pedido
        );

        this.pedidosDisponibles = result.pedidos.filter(
          (p: PedidoInterno) => p.id_OrdenCompra === this.adjudicacion.orden_Compra
            && !idsPedidosYaAdjudicados.includes(p.id)
        );

        this.pedidos = result.pedidos;
        this.proveedores = result.proveedores;
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

  escucharCambiosPedido(): void {
    this.form.get('id_Pedido')?.valueChanges.subscribe((idPedido: number) => {
      if (!idPedido) {
        this.proveedoresDelPedido = [];
        this.precioSugerido = 0;
        return;
      }

      this.cargandoProveedores = true;
      this.form.patchValue({ id_Proveedor: null, precio: null });

      this.ofertaService.getAll().subscribe({
        next: (ofertas: OfertaProveedor[]) => {
          const ofertasDelPedido = ofertas.filter(
            (o: OfertaProveedor) => o.id_Pedido_Interno === idPedido
          );

          const idsProveedores = [...new Set(ofertasDelPedido.map((o: OfertaProveedor) => o.id_Proveedor))];
          this.proveedoresDelPedido = this.proveedores.filter(
            (p: Proveedor) => idsProveedores.includes(p.id)
          );

          this.ofertasDelPedidoActual = ofertasDelPedido;
          this.cargandoProveedores = false;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al cargar ofertas: ${err.status}`);
          this.cargandoProveedores = false;
          this.cdr.detectChanges();
        }
      });
    });

    this.form.get('id_Proveedor')?.valueChanges.subscribe((idProveedor: number) => {
      if (!idProveedor || !this.ofertasDelPedidoActual) {
        this.precioSugerido = 0;
        return;
      }

      const oferta = this.ofertasDelPedidoActual.find(
        (o: OfertaProveedor) => o.id_Proveedor === idProveedor
      );
      if (oferta) {
        this.precioSugerido = oferta.precio;
        this.form.patchValue({ precio: oferta.precio });
      }
    });
  }

  // ===== HELPERS =====
  codigoPedido(idPedido: number): string {
    return this.pedidos.find((p: PedidoInterno) => p.id === idPedido)?.codigo || `Pedido #${idPedido}`;
  }

  nombreProveedor(idProveedor: number): string {
    return this.proveedores.find((p: Proveedor) => p.id === idProveedor)?.nombre || `Proveedor #${idProveedor}`;
  }

  getPrecioOferta(idProveedor: number): number {
    const oferta = this.ofertasDelPedidoActual?.find(
      (o: OfertaProveedor) => o.id_Proveedor === idProveedor
    );
    return oferta?.precio || 0;
  }

  // ✅ NUEVO: Color del chip según el tipo de relación
  claseTipoRelacion(tipo: string): string {
    switch (tipo?.toLowerCase()) {
      case 'socio comercial': return 'rel-socio';
      case 'distribuidor':    return 'rel-distribuidor';
      case 'colaborador':     return 'rel-colaborador';
      default:                return 'rel-default';
    }
  }

  // ===== FORMULARIO =====
  abrirFormulario(): void {
    this.mostrarFormulario = true;
    this.form.reset({ cantidad: 1 });
    this.proveedoresDelPedido = [];
    this.precioSugerido = 0;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.form.reset({ cantidad: 1 });
    this.proveedoresDelPedido = [];
    this.precioSugerido = 0;
    this.cdr.detectChanges();
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    const dto = {
      id_Adjudicacion: this.adjudicacion.id,
      id_Pedido: datos.id_Pedido,
      id_Proveedor: datos.id_Proveedor,
      precio: datos.precio,
      cantidad: datos.cantidad
    };

    this.detalleService.crear(dto).subscribe({
      next: () => {
        this.guardando = false;
        this.notificacion.exito('Detalle agregado correctamente');
        this.cerrarFormulario();
        this.cargarTodo();
      },
      error: (err: any) => {
        this.guardando = false;
        this.notificacion.error(
          err.error?.mensaje || `Error: ${err.status} ${err.statusText}`
        );
      }
    });
  }

  confirmarEliminar(detalle: DetalleAdjudicacion): void {
    this.confirm.eliminar(`Detalle del pedido ${this.codigoPedido(detalle.id_Pedido)}`)
      .subscribe((confirmado: boolean) => {
        if (!confirmado) return;

        this.detalleService.eliminar(
          detalle.id_Adjudicacion,
          detalle.id_Pedido,
          detalle.id_Proveedor
        ).subscribe({
          next: () => {
            this.notificacion.exito('Detalle eliminado correctamente');
            this.cargarTodo();
          },
          error: (err: any) => {
            this.notificacion.error(
              err.error?.mensaje || `Error: ${err.status} ${err.statusText}`
            );
          }
        });
      });
  }

  onCerrar(): void {
    this.cerrar.emit();
  }
}