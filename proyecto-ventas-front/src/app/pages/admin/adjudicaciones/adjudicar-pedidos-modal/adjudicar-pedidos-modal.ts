import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
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
import { OrdenCompraService, OrdenCompraConContadores } from '../../../../services/orden-compra.service';
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
  @Input() ordenIdPreseleccionada: number | null = null; 
  @Output() cerrar = new EventEmitter<void>();
  @Output() adjudicado = new EventEmitter<void>();

  ordenesConPedidos: number[] = [];
  ordenSeleccionada: number | null = null;
  pedidosConOfertas: PedidoConOfertas[] = [];

  pedidos: PedidoInterno[] = [];
  proveedores: Proveedor[] = [];
  ofertas: OfertaProveedor[] = [];
  ordenes: OrdenCompraConContadores[] = [];

  cargando = false;
  guardando = false;

  constructor(
    private adjudicacionService: AdjudicacionService,
    private pedidoService: PedidoInternoService,
    private ofertaService: OfertaProveedorService,
    private proveedorService: ProveedorService,
    private ordenService: OrdenCompraService,
    private route: ActivatedRoute,
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
    proveedores: this.proveedorService.listar(),
    ordenes: this.ordenService.listarConContadores()
  }).subscribe({
    next: (result) => {
      this.pedidos = result.pedidos;
      this.ofertas = result.ofertas;
      this.proveedores = result.proveedores;
      this.ordenes = result.ordenes;

      // ✅ Obtener pedidos CON OFERTAS pero SIN ADJUDICAR
      const pedidosDisponibles = this.pedidos.filter(p =>
        p.totalOfertas > 0 && !p.adjudicado && p.id_OrdenCompra
      );

      const ordenesUnicas = [...new Set(
        pedidosDisponibles.map(p => p.id_OrdenCompra!)
      )];

      this.ordenesConPedidos = ordenesUnicas.filter(idOrden => {
        const orden = this.ordenes.find(o => o.id === idOrden);
        if (!orden || orden.estado === 'Adjudicada' || orden.estado === 'Cancelada') {
          return false;
        }
        const pedidosDeLaOrden = this.pedidos.filter(p => p.id_OrdenCompra === idOrden);
        return pedidosDeLaOrden.some(p => !p.adjudicado && p.totalOfertas > 0);
      });

      this.cargando = false;
      this.cdr.detectChanges();

      // ✅ Preseleccionar: primero el @Input, luego el query param
      const ordenIdQuery = this.ordenIdPreseleccionada;

      if (ordenIdQuery && this.ordenesConPedidos.includes(ordenIdQuery)) {
        setTimeout(() => {
          this.ordenSeleccionada = ordenIdQuery;
          this.onOrdenSeleccionada();
          this.cdr.detectChanges();
        }, 300);
      } else {
        // Fallback: leer query param
        this.route.queryParams.subscribe(params => {
          const ordenId = params['ordenId'];
          if (ordenId) {
            const id = parseInt(ordenId, 10);
            if (!isNaN(id) && this.ordenesConPedidos.includes(id)) {
              setTimeout(() => {
                this.ordenSeleccionada = id;
                this.onOrdenSeleccionada();
                this.cdr.detectChanges();
              }, 300);
            }
          }
        });
      }
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

    const pedidosDeLaOrden = this.pedidos.filter(
      p => p.id_OrdenCompra === this.ordenSeleccionada
    );

    this.pedidosConOfertas = pedidosDeLaOrden
      .map(pedido => {
        const ofertasDelPedido = this.ofertas.filter(
          o => o.id_Pedido_Interno === pedido.id
        );

        // Ordenar por precio (menor primero)
        ofertasDelPedido.sort((a, b) => a.precio - b.precio);

        const mejorOferta = ofertasDelPedido[0];

        return {
          pedido,
          ofertas: ofertasDelPedido,
          // ❌ NO marcar como seleccionado si ya está adjudicado
          seleccionado: !!mejorOferta && !pedido.adjudicado,
          id_ProveedorSeleccionado: mejorOferta ? mejorOferta.id_Proveedor : null
        };
      })
      // ✅ Excluir pedidos sin ofertas Y pedidos ya adjudicados
      .filter(p => p.ofertas.length > 0 && !p.pedido.adjudicado);

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