import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { OrdenCompraService, OrdenCompraConContadores, PedidoDeOrden } from '../../../../services/orden-compra.service';
import { NotificacionService } from '../../../../services/notificacion';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-pedidos-de-orden-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatTooltipModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './pedidos-de-orden-modal.html',
  styleUrl: './pedidos-de-orden-modal.scss'
})
export class PedidosDeOrdenModalComponent implements OnInit {
  @Input() orden!: OrdenCompraConContadores;
  @Output() cerrar = new EventEmitter<void>();
  @Output() pedidosActualizados = new EventEmitter<void>();
  @Output() crearPedido = new EventEmitter<number>();

  pedidos: PedidoDeOrden[] = [];
  cargando = false;

  displayedColumns: string[] = ['urgente', 'codigo', 'cantidad', 'estado', 'acciones'];

  constructor(
    private ordenService: OrdenCompraService,
    private notificacion: NotificacionService,
     private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargarPedidos();
  }

  cargarPedidos(): void {
    this.cargando = true;
    this.ordenService.obtenerPedidosDeOrden(this.orden.id).subscribe({
      next: (data) => {
        this.pedidos = data;
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

  onCrearPedido(): void {
    this.crearPedido.emit(this.orden.id);
  }

  onCerrar(): void {
    this.cerrar.emit();
  }

  // ===== PERMISOS =====
puedeAgregarPedidos(): boolean {
  // Solo Admin y Gestor pueden agregar pedidos
  return this.authService.esAdmin() || this.authService.esGestorCompras();
}
}