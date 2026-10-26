import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';

import { PedidoInternoService } from '../../../../services/pedido-interno.service';
import { SucursalService } from '../../../../services/sucursal.service';
import { AuthService } from '../../../../services/auth.service';
import { ArticuloService } from '../../../../services/articulo.service';
import { PedidoInterno, CreatePedidoInternoDTO } from '../../../../models/pedido-interno.model';
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
    MatCheckboxModule,
    MatCardModule,
    MatTooltipModule
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
  articulosFiltrados: any[] = [];

  constructor(
    private fb: FormBuilder,
    private pedidoService: PedidoInternoService,
    private sucursalService: SucursalService,
    private articuloService: ArticuloService,
    private authService: AuthService,
    private notification: NotificacionService,
    private cdr: ChangeDetectorRef // ✅ Inyectado para forzar redibujado
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
    this.sucursalService.listar().subscribe({
      next: (data) => this.sucursales = data
    });

    // ✅ Cargamos el catálogo de productos
    this.articuloService.listar().subscribe({
      next: (data: any) => {
        this.articulosDisponibles = Array.isArray(data) ? data : (data.$values || []);
        this.articulosFiltrados = [...this.articulosDisponibles];

        // ✅ Si estamos editando un pedido, cargamos sus artículos.
        if (this.pedido) {
          this.cargarArticulosEnEdicion();
        }
        // ✅ Si venimos de una orden y es un pedido NUEVO, cargamos los artículos de esa orden.
        else if (this.ordenPreseleccionada) {
          this.cargarArticulosDeLaOrden(this.ordenPreseleccionada);
        }
      }
    });

    if (this.pedido) {
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
    } else {
      this.generarCodigo();
      if (this.ordenPreseleccionada) {
        this.form.patchValue({ id_OrdenCompra: this.ordenPreseleccionada });
        const ordenSel = this.ordenes.find(o => o.id === this.ordenPreseleccionada);
        if (ordenSel && ordenSel.fecha_limite_ofertas) {
          this.form.patchValue({ fecha_Solicitada: ordenSel.fecha_limite_ofertas.substring(0, 10) });
        }
      }
      this.preseleccionarDepartamento();
    }

    this.form.get('id_Departamento')?.valueChanges.subscribe(idDepto => {
      this.actualizarSucursal(idDepto);
    });
  }

  private cargarArticulosEnEdicion() {
    if (this.pedido?.articulos && this.pedido.articulos.length > 0) {
      this.articulos.clear();
      this.pedido.articulos.forEach(art => {
        const artInfo = this.articulosDisponibles.find(a => a.id === art.id_Articulo);
        this.articulos.push(this.fb.group({
          id_Articulo: [art.id_Articulo, [Validators.required]],
          nombre: [artInfo?.nombre || 'Artículo ' + art.id_Articulo],
          codigo: [artInfo?.codigo || ''],
          cantidad: [art.cantidad, [Validators.required, Validators.min(1)]]
        }));
      });
      this.cdr.detectChanges();
    }
  }

  private cargarArticulosDeLaOrden(idOrden: number) {
    this.pedidoService.listar().subscribe({
      next: (pedidos) => {
        const pedidosDeLaOrden = pedidos.filter(p => Number(p.id_OrdenCompra) === Number(idOrden));
        const pedidoConArticulos = pedidosDeLaOrden.find(p => p.articulos && p.articulos.length > 0);

        if (pedidoConArticulos) {
          this.articulos.clear(); // ✅ Limpiamos el carrito antes de llenarlo

          pedidoConArticulos.articulos.forEach(art => {
            const artInfo = this.articulosDisponibles.find(a => a.id === art.id_Articulo);
            this.articulos.push(this.fb.group({
              id_Articulo: [art.id_Articulo, [Validators.required]],
              nombre: [artInfo?.nombre || 'Artículo ' + art.id_Articulo],
              codigo: [artInfo?.codigo || ''],
              cantidad: [art.cantidad, [Validators.required, Validators.min(1)]]
            }));
          });

          this.cdr.detectChanges();
        }
      },
      error: (err) => console.error('Error al cargar artículos de la orden:', err)
    });
  }

  filtrarArticulos(event: Event) {
    const valor = (event.target as HTMLInputElement).value.toLowerCase();
    this.articulosFiltrados = this.articulosDisponibles.filter(a => a.nombre.toLowerCase().includes(valor) || a.codigo.toLowerCase().includes(valor));
  }

  agregarAlCarrito(articulo: any) {
    const existe = this.articulos.controls.find(c => c.get('id_Articulo')?.value === articulo.id);
    if (existe) {
      existe.patchValue({ cantidad: existe.get('cantidad')?.value + 1 });
    } else {
      this.articulos.push(this.fb.group({
        id_Articulo: [articulo.id, Validators.required],
        nombre: [articulo.nombre],
        codigo: [articulo.codigo],
        cantidad: [1, [Validators.required, Validators.min(1)]]
      }));
    }
    this.cdr.detectChanges(); // ✅ Forzamos redibujado al agregar
  }

  eliminarDelCarrito(index: number) {
    this.articulos.removeAt(index);
    this.cdr.detectChanges(); // ✅ Forzamos redibujado al eliminar
  }

  get articulos(): FormArray {
    return this.form.get('articulos') as FormArray;
  }

  fechaMayorQueOrden(fechaCreacionOrden: string): any {
    return (control: any) => {
      if (!control.value) return null;
      if (new Date(control.value) < new Date(fechaCreacionOrden)) return { fechaInvalid: true };
      return null;
    };
  }

  preseleccionarDepartamento(): void {
    const idDeptoUsuario = this.authService.getIdDepartamento();
    if (!idDeptoUsuario) return;

    this.form.patchValue({ id_Departamento: idDeptoUsuario });
    this.actualizarSucursal(idDeptoUsuario);

    if (this.authService.esCreadorPedidos()) {
      this.form.get('id_Departamento')?.disable();
    }
  }

  actualizarSucursal(idDepartamento: number): void {
    if (!idDepartamento) { this.sucursalSeleccionada = ''; return; }
    const depto = this.departamentos.find(d => d.id === idDepartamento);
    if (depto && depto.id_Sucursal) {
      const sucursal = this.sucursales.find(s => s.id === depto.id_Sucursal);
      this.sucursalSeleccionada = sucursal ? sucursal.nombre : '';
    }
  }

  generarCodigo(): void {
    this.pedidoService.listar().subscribe({
      next: (pedidos) => {
        const maxId = pedidos.length > 0 ? Math.max(...pedidos.map(p => p.id)) : 0;
        this.form.patchValue({ codigo: `PED-${new Date().getFullYear()}-${String(maxId + 1).padStart(3, '0')}` });
      },
      error: () => this.form.patchValue({ codigo: 'PED-' + Date.now().toString().slice(-4) })
    });
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.guardando = true;
    const datos = this.form.getRawValue();
    const articulosFiltrados = datos.articulos.map((a: any) => ({ id_Articulo: a.id_Articulo, cantidad: a.cantidad }));

    const totalCantidad = datos.articulos.reduce((acc: number, a: any) => acc + a.cantidad, 0) || 1;

    if (this.pedido) {
      const dto = { ...datos, id: this.pedido.id, cantidad: totalCantidad, articulos: articulosFiltrados };
      this.pedidoService.actualizar(dto as any).subscribe({
        next: () => { this.guardando = false; this.notification.exito('Pedido actualizado'); this.guardado.emit(); },
        error: (err: any) => { this.guardando = false; this.notification.error(err.error?.mensaje || 'Error'); }
      });
    } else {
      const dto: CreatePedidoInternoDTO = { ...datos, cantidad: totalCantidad, articulos: articulosFiltrados.length > 0 ? articulosFiltrados : undefined };
      this.pedidoService.crear(dto).subscribe({
        next: () => { this.guardando = false; this.notification.exito('Pedido creado'); this.guardado.emit(); },
        error: (err: any) => { this.guardando = false; this.notification.error(err.error?.mensaje || 'Error'); }
      });
    }
  }

  onCancel(): void { this.cancelado.emit(); }
}
