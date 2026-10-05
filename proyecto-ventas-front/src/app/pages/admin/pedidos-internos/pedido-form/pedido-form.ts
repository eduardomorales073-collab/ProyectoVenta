import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { PedidoInternoService } from '../../../../services/pedido-interno.service';
import { SucursalService } from '../../../../services/sucursal.service';
import { AuthService } from '../../../../services/auth.service';
import { ArticuloService } from '../../../../services/articulo.service';
import { PedidoInterno, CreatePedidoInternoDTO, CreateArticuloPedidoDTO } from '../../../../models/pedido-interno.model';
import { Departamento } from '../../../../models/departamento.model';
import { Sucursal } from '../../../../models/sucursal.model';
import { OrdenCompraConContadores } from '../../../../services/orden-compra.service';
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
    MatIconModule,
    MatCheckboxModule
  ],
  templateUrl: './pedido-form.html',
  styleUrl: './pedido-form.scss'
})
export class PedidoFormComponent implements OnInit {
  @Input() pedido: PedidoInterno | null = null;
  @Input() departamentos: Departamento[] = [];
  @Input() ordenes: OrdenCompraConContadores[] = [];
  @Input() ordenPreseleccionada: number | null = null;

  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  sucursalSeleccionada = '';
  sucursales: Sucursal[] = [];
  articulosDisponibles: any[] = [];

  constructor(
    private fb: FormBuilder,
    private pedidoService: PedidoInternoService,
    private sucursalService: SucursalService,
    private articuloService: ArticuloService,
    private authService: AuthService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      codigo: [{ value: '', disabled: true }],
      cantidad: [1, [Validators.required, Validators.min(1)]],
      id_Departamento: ['', [Validators.required]],
      id_OrdenCompra: [null],
      fecha_Solicitada: ['', [Validators.required]],
      urgente: [false],
      observaciones: [''],
      articulos: this.fb.array([])
    });
  }

  ngOnInit(): void {
    // ===== Cargar sucursales =====
    this.sucursalService.listar().subscribe({
      next: (data) => { this.sucursales = data; }
    });

    // ===== Cargar artículos disponibles =====
    this.articuloService.listar().subscribe({
      next: (data) => { this.articulosDisponibles = data; }
    });

    if (this.pedido) {
      // ==================== EDICIÓN ====================
      this.form.patchValue({
        codigo: this.pedido.codigo || '',
        cantidad: this.pedido.cantidad || 1,
        id_Departamento: this.pedido.id_Departamento,
        id_OrdenCompra: this.pedido.id_OrdenCompra,
        fecha_Solicitada: this.pedido.fecha_Solicitada?.substring(0, 10),
        urgente: this.pedido.urgente || false,
        observaciones: this.pedido.observaciones || ''
      });

      this.actualizarSucursal(this.pedido.id_Departamento);

      // ✅ Cargar los artículos existentes en el FormArray
      if (this.pedido.articulos && this.pedido.articulos.length > 0) {
        this.pedido.articulos.forEach(art => {
          this.articulos.push(this.fb.group({
            id_Articulo: [art.id_Articulo, [Validators.required]],
            cantidad: [art.cantidad, [Validators.required, Validators.min(1)]]
          }));
        });
      }
    } else {
      // ==================== CREACIÓN ====================
      this.generarCodigo();

      // ✅ Preseleccionar la orden (viene desde el modal de Órdenes de Compra)
      if (this.ordenPreseleccionada) {
        this.form.patchValue({ id_OrdenCompra: this.ordenPreseleccionada });

        // ✅ Heredar fecha límite de ofertas de la orden
        const ordenSeleccionada = this.ordenes.find(o => o.id === this.ordenPreseleccionada);
        if (ordenSeleccionada && ordenSeleccionada.fecha_limite_ofertas) {
          this.form.patchValue({
            fecha_Solicitada: ordenSeleccionada.fecha_limite_ofertas.substring(0, 10)
          });
        }
      }

      // ✅ Preseleccionar departamento según rol
      this.preseleccionarDepartamento();
    }

    // ===== Escuchar cambios en el departamento =====
    this.form.get('id_Departamento')?.valueChanges.subscribe(idDepto => {
      this.actualizarSucursal(idDepto);
    });

    // ===== Escuchar cambios en la orden (validación de fecha) =====
    this.form.get('id_OrdenCompra')?.valueChanges.subscribe(idOrden => {
      if (idOrden) {
        const orden = this.ordenes.find(o => o.id === idOrden);
        if (orden && orden.fecha_Creacion) {
          this.form.get('fecha_Solicitada')?.setValidators([
            Validators.required,
            this.fechaMayorQueOrden(orden.fecha_Creacion)
          ]);
          this.form.get('fecha_Solicitada')?.updateValueAndValidity();
        }
      } else {
        this.form.get('fecha_Solicitada')?.setValidators([Validators.required]);
        this.form.get('fecha_Solicitada')?.updateValueAndValidity();
      }
    });
  }

  // ✅ Validador personalizado
  fechaMayorQueOrden(fechaCreacionOrden: string): any {
    return (control: any) => {
      if (!control.value) return null;
      const fechaSolicitada = new Date(control.value);
      const fechaOrden = new Date(fechaCreacionOrden);
      if (fechaSolicitada < fechaOrden) {
        return { fechaInvalida: true };
      }
      return null;
    };
  }

  // ===== GETTER PARA EL FORM ARRAY =====
  get articulos(): FormArray {
    return this.form.get('articulos') as FormArray;
  }

  // ===== MÉTODOS DE ARTÍCULOS =====
  agregarArticulo(): void {
    this.articulos.push(this.fb.group({
      id_Articulo: ['', [Validators.required]],
      cantidad: [1, [Validators.required, Validators.min(1)]]
    }));
  }

  eliminarArticulo(index: number): void {
    this.articulos.removeAt(index);
  }

  /** Preselecciona el departamento del usuario logueado según su rol */
  private preseleccionarDepartamento(): void {
    const idDeptoUsuario = this.authService.getIdDepartamento();
    if (!idDeptoUsuario) return;
    if (this.authService.esAdmin()) return;

    this.form.patchValue({ id_Departamento: idDeptoUsuario });
    this.actualizarSucursal(idDeptoUsuario);

    if (this.authService.esCreadorPedidos()) {
      this.form.get('id_Departamento')?.disable();
    }
  }

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

    const datos = this.form.getRawValue();

    // ===== VALIDACIÓN 1: Fecha solicitada >= Fecha creación de la orden =====
    if (datos.id_OrdenCompra && datos.fecha_Solicitada) {
      const ordenSeleccionada = this.ordenes.find(o => o.id === datos.id_OrdenCompra);
      if (ordenSeleccionada && ordenSeleccionada.fecha_Creacion) {
        const fechaOrden = new Date(ordenSeleccionada.fecha_Creacion);
        const fechaSolicitada = new Date(datos.fecha_Solicitada);
        if (fechaSolicitada < fechaOrden) {
          this.notificacion.error(
            `La fecha límite de ofertas no puede ser anterior a la creación de la orden (${fechaOrden.toLocaleDateString()}).`
          );
          return;
        }
      }
    }

    this.guardando = true;

    const articulosFiltrados: CreateArticuloPedidoDTO[] = (datos.articulos || [])
      .filter((a: any) => a.id_Articulo && a.cantidad > 0)
      .map((a: any) => ({
        id_Articulo: a.id_Articulo,
        cantidad: a.cantidad
      }));

    if (this.esEdicion) {
      const dto = {
        id: this.pedido!.id,
        codigo: datos.codigo,
        cantidad: datos.cantidad,
        id_Departamento: datos.id_Departamento,
        id_OrdenCompra: datos.id_OrdenCompra || null,
        fecha_Solicitada: datos.fecha_Solicitada,
        fecha_Ingreso: this.pedido!.fecha_Ingreso,
        urgente: datos.urgente,
        observaciones: datos.observaciones || null,
        articulos: articulosFiltrados
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
        urgente: datos.urgente,
        observaciones: datos.observaciones || undefined,
        articulos: articulosFiltrados.length > 0 ? articulosFiltrados : undefined
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