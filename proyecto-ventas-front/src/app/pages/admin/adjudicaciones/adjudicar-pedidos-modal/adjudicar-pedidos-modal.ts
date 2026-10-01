import { Component, EventEmitter, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { forkJoin } from 'rxjs';

import { AdjudicacionService } from '../../../../services/adjudicacion.service';
import { PedidoInternoService } from '../../../../services/pedido-interno.service';
import { OfertaProveedorService } from '../../../../services/oferta-proveedor.service';
import { ProveedorService } from '../../../../services/proveedor.service';
import { NotificacionService } from '../../../../services/notificacion';

import { PedidoInterno } from '../../../../models/pedido-interno.model';
import { Proveedor } from '../../../../models/proveedor.model';
import { OfertaProveedor } from '../../../../models/oferta-proveedor.model';

interface PedidoConOfertas {
  pedido: PedidoInterno;
  ofertas: OfertaProveedor[];
  seleccionado: boolean;
  id_ProveedorSeleccionado: number | null;
}

@Component({
  selector: 'app-adjudicar-pedidos-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './adjudicar-pedidos-modal.html',
  styleUrl: './adjudicar-pedidos-modal.scss'
})
export class AdjudicarPedidosModalComponent implements OnInit {
  @Output() cerrar = new EventEmitter<void>();
  @Output() adjudicado = new EventEmitter<void>();

  // Datos
  ordenesConPedidos: number[] = [];
  ordenSeleccionada: number | null = null;
  pedidosConOfertas: PedidoConOfertas[] = [];

  pedidos: PedidoInterno[] = [];
  proveedores: Proveedor[] = [];
  ofertas: OfertaProveedor[] = [];

  cargando = false;
  guardando = false;

  constructor(
    private adjudicacionService: AdjudicacionService,
    private pedidoService: PedidoInternoService,
    private ofertaService: OfertaProveedorService,
    private proveedorService: ProveedorService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargarTodo();
  }

  cargarTodo(): void {
    this.cargando = true;

    forkJoin({
      pedidos: this.pedidoService.listar(),
      ofertas: this.ofertaService.getAll(),
      proveedores: this.proveedorService.listar()
    }).subscribe({
      next: (result) => {
        this.pedidos = result.pedidos;
        this.ofertas = result.ofertas;
        this.proveedores = result.proveedores;

        // Obtener las órdenes que tienen pedidos pendientes (sin adjudicar)
        // ⚠️ Aquí filtramos pedidos que tienen ofertas y no están adjudicados
        // Para simplificar, mostramos todos los pedidos que tengan ofertas
        const pedidosConOfertas = this.pedidos.filter(p =>
          this.ofertas.some(o => o.id_Pedido_Interno === p.id)
        );

        // Extraer las órdenes únicas
        const ordenes = [...new Set(
          pedidosConOfertas
            .filter(p => p.id_OrdenCompra)
            .map(p => p.id_OrdenCompra!)
        )];

        this.ordenesConPedidos = ordenes;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error al cargar datos: ${err.status}`);
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  onOrdenSeleccionada(): void {
    if (!this.ordenSeleccionada) {
      this.pedidosConOfertas = [];
      return;
    }

    // Filtrar pedidos de esta orden que tengan ofertas
    const pedidosDeLaOrden = this.pedidos.filter(
      p => p.id_OrdenCompra === this.ordenSeleccionada
    );

    this.pedidosConOfertas = pedidosDeLaOrden
      .map(pedido => {
        const ofertasDelPedido = this.ofertas.filter(
          o => o.id_Pedido_Interno === pedido.id
        );

        // Ordenar ofertas por precio (menor primero)
        ofertasDelPedido.sort((a, b) => a.precio - b.precio);

        // Auto-seleccionar la mejor oferta (menor precio)
        const mejorOferta = ofertasDelPedido[0];

        return {
          pedido,
          ofertas: ofertasDelPedido,
          seleccionado: !!mejorOferta,          // Solo si tiene ofertas
          id_ProveedorSeleccionado: mejorOferta ? mejorOferta.id_Proveedor : null
        };
      })
      .filter(p => p.ofertas.length > 0);  // Solo pedidos con ofertas

    this.cdr.detectChanges();
  }

  get nombreProveedor(): (id: number) => string {
    return (id: number) => {
      const p = this.proveedores.find(pr => pr.id === id);
      return p ? p.nombre : `Proveedor #${id}`;
    };
  }

  get totalAdjudicar(): number {
    return this.pedidosConOfertas
      .filter(p => p.seleccionado)
      .reduce((sum, p) => {
        const oferta = p.ofertas.find(o => o.id_Proveedor === p.id_ProveedorSeleccionado);
        return sum + (oferta ? oferta.precio * (p.pedido.cantidad || 1) : 0);
      }, 0);
  }

  get totalPedidosSeleccionados(): number {
    return this.pedidosConOfertas.filter(p => p.seleccionado).length;
  }

  onSubmit(): void {
    if (!this.ordenSeleccionada) {
      this.notificacion.error('Selecciona una orden de compra');
      return;
    }

    const pedidosAAdjudicar = this.pedidosConOfertas
      .filter(p => p.seleccionado && p.id_ProveedorSeleccionado)
      .map(p => ({
        id_Pedido: p.pedido.id,
        id_Proveedor: p.id_ProveedorSeleccionado!
      }));

    if (pedidosAAdjudicar.length === 0) {
      this.notificacion.error('Selecciona al menos un pedido para adjudicar');
      return;
    }

    this.guardando = true;
    const dto = {
      orden_Compra: this.ordenSeleccionada,
      pedidos: pedidosAAdjudicar
    };

    this.adjudicacionService.adjudicarPedidos(dto).subscribe({
      next: () => {
        this.guardando = false;
        this.notificacion.exito(`${pedidosAAdjudicar.length} pedidos adjudicados correctamente`);
        this.adjudicado.emit();
      },
      error: (err: any) => {
        this.guardando = false;
        this.notificacion.error(
          err.error?.mensaje || `Error: ${err.status} ${err.statusText}`
        );
      }
    });
  }

  onCerrar(): void {
    this.cerrar.emit();
  }
}