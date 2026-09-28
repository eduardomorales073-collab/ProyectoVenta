import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { OfertaProveedorService } from '../../../services/oferta-proveedor.service';
import { PedidoInternoService } from '../../../services/pedido-interno.service';
import { OfertaProveedor } from '../../../models/oferta-proveedor.model';
import { PedidoInterno } from '../../../models/pedido-interno.model';
import { NotificacionService } from '../../../services/notificacion';
import { ConfirmService } from '../../../services/confirm';

@Component({
  selector: 'app-ofertas',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTooltipModule
  ],
  templateUrl: './ofertas.html',
  styleUrl: './ofertas.scss'
})
export class ProveedorOfertasComponent implements OnInit {
  ofertas: OfertaProveedor[] = [];
  pedidos: PedidoInterno[] = [];
  displayedColumns: string[] = ['id', 'pedido', 'precio', 'fecha', 'acciones'];
  cargando = false;

  constructor(
    private ofertaService: OfertaProveedorService,
    private pedidoService: PedidoInternoService,
    private notificacion: NotificacionService,
    private confirm: ConfirmService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargarPedidos();
    this.cargarOfertas();
  }

  cargarPedidos(): void {
    this.pedidoService.listar().subscribe({
      next: (data) => { this.pedidos = data; this.cdr.detectChanges(); }
    });
  }

  cargarOfertas(): void {
    this.cargando = true;
    this.ofertaService.misOfertas().subscribe({
      next: (data) => {
        this.ofertas = data;
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

  codigoPedido(idPedido: number): string {
    const pedido = this.pedidos.find(p => p.id === idPedido);
    return pedido?.codigo || `Pedido #${idPedido}`;
  }

  confirmarEliminar(oferta: OfertaProveedor): void {
    this.confirm.eliminar(`Oferta #${oferta.id}`).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.ofertaService.eliminar(oferta.id).subscribe({
        next: () => {
          this.notificacion.exito(`Oferta #${oferta.id} eliminada correctamente`);
          this.cargarOfertas();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
    });
  }
}