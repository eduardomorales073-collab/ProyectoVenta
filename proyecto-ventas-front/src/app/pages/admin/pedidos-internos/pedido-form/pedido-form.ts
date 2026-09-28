import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PedidoInternoService } from '../../../../services/pedido-interno.service';
import { SucursalService } from '../../../../services/sucursal.service';
import { PedidoInterno, CreatePedidoInternoDTO } from '../../../../models/pedido-interno.model';
import { Departamento } from '../../../../models/departamento.model';
import { Sucursal } from '../../../../models/sucursal.model';
import { OrdenCompra } from '../../../../services/orden-compra.service';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-pedido-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './pedido-form.html',
  styleUrl: './pedido-form.scss'
})
export class PedidoFormComponent implements OnInit {
  @Input() pedido: PedidoInterno | null = null;
  @Input() departamentos: Departamento[] = [];
  @Input() ordenes: OrdenCompra[] = [];
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  // ← NUEVO: Para la sucursal auto-detectada
  sucursalSeleccionada = '';
  sucursales: Sucursal[] = [];

  constructor(
    private fb: FormBuilder,
    private pedidoService: PedidoInternoService,
    private sucursalService: SucursalService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      codigo: [{ value: '', disabled: true }],
      cantidad: [1, [Validators.required, Validators.min(1)]],
      id_Departamento: ['', [Validators.required]],
      id_OrdenCompra: [null],
      fecha_Solicitada: ['', [Validators.required]],
      fecha_Ingreso: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    // Cargar sucursales
    this.sucursalService.listar().subscribe({
      next: (data) => { this.sucursales = data; }
    });

    if (this.pedido) {
      // EDICIÓN
      this.form.patchValue({
        codigo: this.pedido.codigo || '',
        cantidad: this.pedido.cantidad || 1,
        id_Departamento: this.pedido.id_Departamento,
        id_OrdenCompra: this.pedido.id_OrdenCompra,
        fecha_Solicitada: this.pedido.fecha_Solicitada?.substring(0, 10),
        fecha_Ingreso: this.pedido.fecha_Ingreso?.substring(0, 10)
      });
      // Calcular sucursal inicial
      this.actualizarSucursal(this.pedido.id_Departamento);
    } else {
      // CREACIÓN: auto-generar el código
      this.generarCodigo();
    }

    // Escuchar cambios en el departamento
    this.form.get('id_Departamento')?.valueChanges.subscribe(idDepto => {
      this.actualizarSucursal(idDepto);
    });
  }

  /** Calcula la sucursal basándose en el departamento seleccionado */
  actualizarSucursal(idDepartamento: number): void {
    if (!idDepartamento) {
      this.sucursalSeleccionada = '';
      return;
    }

    const depto = this.departamentos.find(d => d.id === idDepartamento);
    if (depto && depto.id_Sucursal) {
      const sucursal = this.sucursales.find(s => s.id === depto.id_Sucursal);
      this.sucursalSeleccionada = sucursal?.nombre || '';
    } else {
      this.sucursalSeleccionada = '';
    }
  }

  get esEdicion(): boolean {
    return !!this.pedido;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.getRawValue();

    if (this.esEdicion) {
      const dto = {
        id: this.pedido!.id,
        codigo: datos.codigo,
        cantidad: datos.cantidad,
        id_Departamento: datos.id_Departamento,
        id_OrdenCompra: datos.id_OrdenCompra || null,
        fecha_Solicitada: datos.fecha_Solicitada,
        fecha_Ingreso: datos.fecha_Ingreso
      };

      this.pedidoService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Pedido actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(err.error?.mensaje || `Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreatePedidoInternoDTO = {
        codigo: datos.codigo,
        cantidad: datos.cantidad,
        id_Departamento: datos.id_Departamento,
        id_OrdenCompra: datos.id_OrdenCompra || null,
        fecha_Solicitada: datos.fecha_Solicitada,
        fecha_Ingreso: datos.fecha_Ingreso
      };

      this.pedidoService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Pedido creado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(err.error?.mensaje || `Error: ${err.status} ${err.statusText}`);
        }
      });
    }
  }

  onCancelar(): void {
    this.cancelado.emit();
  }

  generarCodigo(): void {
    this.pedidoService.listar().subscribe({
      next: (pedidos) => {
        const maxId = pedidos.length > 0 ? Math.max(...pedidos.map(p => p.id)) : 0;
        const nuevoId = maxId + 1;
        const anio = new Date().getFullYear();
        const codigo = `PED-${anio}-${String(nuevoId).padStart(3, '0')}`;
        this.form.patchValue({ codigo });
      },
      error: () => {
        const codigo = 'PED-' + Date.now().toString().slice(-4);
        this.form.patchValue({ codigo });
      }
    });
  }
}