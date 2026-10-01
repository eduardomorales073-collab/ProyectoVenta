import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { PedidoInternoService } from '../../../services/pedido-interno.service';
import { DepartamentoService } from '../../../services/departamento.service';
import { PedidoInterno } from '../../../models/pedido-interno.model';
import { Departamento } from '../../../models/departamento.model';
import { OfertaFormComponent } from './oferta-form/oferta-form';
import { NotificacionService } from '../../../services/notificacion';
import { OfertaProveedorService } from '../../../services/oferta-proveedor.service';
import { OfertaProveedor } from '../../../models/oferta-proveedor.model';

@Component({
  selector: 'app-pedidos-disponibles',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    OfertaFormComponent
  ],
  templateUrl: './pedidos-disponibles.html',
  styleUrl: './pedidos-disponibles.scss'
})
export class PedidosDisponiblesComponent implements OnInit {
  pedidos: PedidoInterno[] = [];
  departamentos: Departamento[] = [];
  displayedColumns: string[] = ['codigo', 'cantidad', 'departamento', 'fecha_Solicitada', 'acciones'];
  cargando = false;
  mostrarFormulario = false;
  pedidoSeleccionado: PedidoInterno | null = null;

  constructor(
    private pedidoService: PedidoInternoService,
    private departamentoService: DepartamentoService,
    private notificacion: NotificacionService,
    private ofertaService: OfertaProveedorService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
  this.cargarDepartamentos();
  this.cargarMisOfertas();   // ← NUEVO
  this.cargar();
}

  cargarDepartamentos(): void {
    this.departamentoService.listar().subscribe({
      next: (data) => { this.departamentos = data; this.cdr.detectChanges(); }
    });
  }

  
  cargar(): void {
  this.cargando = true;
  this.pedidoService.listar().subscribe({
    next: (data) => {
      // ✅ Mostrar TODOS los pedidos (los ya ofertados aparecen con "Ya ofertaste")
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

  nombreDepartamento(id: number): string {
    return this.departamentos.find(d => d.id === id)?.nombre || `Depto #${id}`;
  }

  abrirOferta(pedido: PedidoInterno): void {
    this.pedidoSeleccionado = pedido;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.pedidoSeleccionado = null;
    this.cdr.detectChanges();
  }

  onOfertado(): void {
  this.cerrarFormulario();
  this.cargarMisOfertas();   
  this.cargar();
}
  misOfertas: OfertaProveedor[] = [];
  cargarMisOfertas(): void {
  this.ofertaService.misOfertas().subscribe({
    next: (data) => {
      this.misOfertas = data;
      this.cdr.detectChanges();
    }
  });
}

yaOferto(idPedido: number): boolean {
  return this.misOfertas.some(o => o.id_Pedido_Interno === idPedido);
}
}