import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { AuditoriaService } from '../../../services/auditoria.service';
import { AuthService } from '../../../services/auth.service';
import { NotificacionService } from '../../../services/notificacion';
import { EventoAuditoria, FiltroAuditoria } from '../../../models/evento-auditoria.model';

@Component({
  selector: 'app-auditoria',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './auditoria.html',
  styleUrl: './auditoria.scss'
})
export class AuditoriaComponent implements OnInit {
  eventos: EventoAuditoria[] = [];
  displayedColumns: string[] = ['fecha', 'tipo', 'titulo', 'detalle', 'usuario'];
  cargando = false;
  rutaVolver = '/admin/dashboard';

  // ===== FILTROS =====
  tipoSeleccionado: string | null = null;
  fechaDesde: string | null = null;
  fechaHasta: string | null = null;

  tiposEvento = [
    { valor: null, etiqueta: 'Todos' },
    { valor: 'Compra', etiqueta: 'Compras' },
    { valor: 'Pedido', etiqueta: 'Pedidos' },
    { valor: 'Oferta', etiqueta: 'Ofertas' },
    { valor: 'Orden', etiqueta: 'Órdenes' },
    { valor: 'Cancelacion', etiqueta: 'Cancelaciones' },
    { valor: 'Adjudicacion', etiqueta: 'Adjudicaciones' },        
  { valor: 'PedidoActualizado', etiqueta: 'Pedidos Actualizados' }, 
  { valor: 'OfertaActualizada', etiqueta: 'Ofertas Actualizadas' } ,
  ];

  constructor(
    private auditoriaService: AuditoriaService,
    private authService: AuthService,
    private notification: NotificacionService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.rutaVolver = this.authService.getRutaHubReportes();
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    const filtro: FiltroAuditoria = {
      tipo: this.tipoSeleccionado,
      fechaDesde: this.fechaDesde,
      fechaHasta: this.fechaHasta
    };

    this.auditoriaService.getHistorial(filtro).subscribe({
      next: (data) => {
        this.eventos = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notification.error(`Error: ${err.status} ${err.statusText}`);
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  limpiarFiltros(): void {
    this.tipoSeleccionado = null;
    this.fechaDesde = null;
    this.fechaHasta = null;
    this.cargar();
  }

  // ===== COLORES POR TIPO =====
  getColorTipo(tipo: string): string {
    switch (tipo) {
      case 'Compra': return 'tipo-compra';
      case 'Pedido': return 'tipo-pedido';
      case 'Oferta': return 'tipo-oferta';
      case 'Orden': return 'tipo-orden';
      case 'Cancelacion': return 'tipo-cancelacion';
      case 'Adjudicacion': return 'tipo-adjudicacion';              
    case 'PedidoActualizado': return 'tipo-pedido-actualizado';   
    case 'OfertaActualizada': return 'tipo-oferta-actualizada';
      default: return '';
    }
  }

  getIconoTipo(tipo: string): string {
    switch (tipo) {
      case 'Compra': return 'shopping_cart';
      case 'Pedido': return 'assignment';
      case 'Oferta': return 'local_offer';
      case 'Orden': return 'receipt_long';
      case 'Cancelacion': return 'cancel';
      case 'Adjudicacion': return 'gavel';                         
    case 'PedidoActualizado': return 'edit_note';                
    case 'OfertaActualizada': return 'price_change';
      default: return 'event';
    }
  }

  get totalEventos(): number {
    return this.eventos.length;
  }
}