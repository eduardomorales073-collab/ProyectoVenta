import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';

import { OrdenCompraService, OrdenCompraConContadores } from '../../../../services/orden-compra.service';
import { TipoOrdenService } from '../../../../services/tipo-orden.service';
import { ArticuloService } from '../../../../services/articulo.service';
import { PedidoInternoService } from '../../../../services/pedido-interno.service';
import { AuthService } from '../../../../services/auth.service';
import { TipoOrden } from '../../../../models/tipo-orden.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-orden-compra-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatTooltipModule
  ],
  templateUrl: './orden-compra-form.html',
  styleUrl: './orden-compra-form.scss'
})
export class OrdenCompraFormComponent implements OnInit {
  @Input() orden: OrdenCompraConContadores | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  tiposOrden: TipoOrden[] = [];
  guardando = false;
  esEdicion = false;

  articulosDisponibles: any[] = [];
  articulosFiltrados: any[] = [];

  constructor(
    private fb: FormBuilder,
    private ordenService: OrdenCompraService,
    private tipoOrdenService: TipoOrdenService,
    private articuloService: ArticuloService,
    private pedidoService: PedidoInternoService,
    private authService: AuthService,
    private notification: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      descripcion: ['', [Validators.required]],
      fecha_Creacion: [{ value: '', disabled: true }],
      fecha_Limite: ['', [Validators.required]],
      fecha_limite_ofertas: [''],
      tipo_Orden: ['', [Validators.required]],
      articulos: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.esEdicion = !!this.orden;

    this.tipoOrdenService.listar().subscribe({
      next: (tipos) => {
        this.tiposOrden = tipos;
        this.cdr.detectChanges();
      }
    });

    this.articuloService.listar().subscribe({
      next: (arts: any) => {
        this.articulosDisponibles = Array.isArray(arts) ? arts : (arts.$values || []);
        this.articulosFiltrados = [...this.articulosDisponibles];
        this.cdr.detectChanges();

        if (this.orden) {
          this.cargarArticulosDeLaOrden(this.orden.id);
        }
      }
    });

    if (this.orden) {
      this.form.patchValue({
        descripcion: this.orden.descripcion,
        fecha_Creacion: this.orden.fecha_Creacion?.substring(0, 10),
        fecha_Limite: this.orden.fecha_Limite?.substring(0, 10),
        fecha_limite_ofertas: this.orden.fecha_limite_ofertas?.substring(0, 10),
        tipo_Orden: this.orden.tipo_Orden
      });
    } else {
      this.form.patchValue({ fecha_Creacion: new Date().toISOString().substring(0, 10) });
    }
  }

  // ✅ CORREGIDO: Busca en TODOS los pedidos de la orden y suma sus artículos
  private cargarArticulosDeLaOrden(idOrden: number) {
    this.pedidoService.listar().subscribe(pedidos => {
      const pedidosDeLaOrden = pedidos.filter(p => Number(p.id_OrdenCompra) === Number(idOrden));
      this.articulos.clear();

      pedidosDeLaOrden.forEach(pedido => {
        if (pedido.articulos && pedido.articulos.length > 0) {
          pedido.articulos.forEach(art => {
            const existe = this.articulos.controls.find(c => c.get('id_Articulo')?.value === art.id_Articulo);
            if (existe) {
              // Si ya existe, sumamos la cantidad
              existe.patchValue({ cantidad: existe.get('cantidad')?.value + art.cantidad });
            } else {
              // Si no existe, lo agregamos
              const artInfo = this.articulosDisponibles.find(a => a.id === art.id_Articulo);
              this.articulos.push(this.fb.group({
                id_Articulo: [art.id_Articulo, Validators.required],
                nombre: [artInfo?.nombre || 'Artículo ' + art.id_Articulo],
                codigo: [artInfo?.codigo || ''],
                cantidad: [art.cantidad, [Validators.required, Validators.min(1)]]
              }));
            }
          });
        }
      });
      this.cdr.detectChanges();
    });
  }

  nombreTipo(tipo: any): string {
    if (!tipo) return '';
    const partes = [];
    if (tipo.grande) partes.push('Grande');
    if (tipo.urgente) partes.push('Urgente');
    return partes.length > 0 ? partes.join(' ') : 'Normal';
  }

  filtrarArticulos(event: Event) {
    const valor = (event.target as HTMLInputElement).value.toLowerCase();
    this.articulosFiltrados = this.articulosDisponibles.filter(
      a => a.nombre.toLowerCase().includes(valor) || a.codigo.toLowerCase().includes(valor)
    );
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
    this.cdr.detectChanges();
  }

  eliminarDelCarrito(index: number) {
    this.articulos.removeAt(index);
    this.cdr.detectChanges();
  }

  get articulos(): FormArray {
    return this.form.get('articulos') as FormArray;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.guardando = true;
    const datos = this.form.getRawValue();
    const articulosDto = datos.articulos.map((a: any) => ({ id_Articulo: a.id_Articulo, cantidad: a.cantidad }));

    const usuarioActual = this.authService.getUsuario() as any;
    const idUsuario = usuarioActual?.id || usuarioActual?.idUsuario || 1;

    const dtoOrden: any = {
      descripcion: datos.descripcion,
      fecha_Creacion: datos.fecha_Creacion,
      fecha_Limite: datos.fecha_Limite,
      fecha_limite_ofertas: datos.fecha_limite_ofertas || null,
      tipo_Orden: datos.tipo_Orden,
      id_UsuarioCreador: idUsuario
    };

    if (this.esEdicion && this.orden) {
      // ================= ACTUALIZAR =================
      dtoOrden.id = this.orden.id;

      this.ordenService.actualizar(dtoOrden).subscribe({
        next: () => {
          this.pedidoService.listar().subscribe(pedidos => {
            const pedidosDeLaOrden = pedidos.filter(p => Number(p.id_OrdenCompra) === Number(this.orden!.id));

            if (pedidosDeLaOrden.length > 0) {
              const pedidoActualizar = pedidosDeLaOrden[0];
              const dtoPedido = {
                id: pedidoActualizar.id,
                codigo: pedidoActualizar.codigo,
                cantidad: articulosDto.reduce((acc: number, a: any) => acc + a.cantidad, 0) || 1,
                id_Departamento: pedidoActualizar.id_Departamento,
                id_OrdenCompra: this.orden!.id,
                fecha_Solicitada: datos.fecha_limite_ofertas || datos.fecha_Limite,
                urgente: pedidoActualizar.urgente,
                observaciones: pedidoActualizar.observaciones,
                articulos: articulosDto
              };

              this.pedidoService.actualizar(dtoPedido as any).subscribe({
                next: () => this.finalizarGuardado('Orden y Pedido actualizados correctamente'),
                error: () => this.finalizarGuardado('Orden actualizada, error al actualizar pedido', true)
              });
            } else if (articulosDto.length > 0) {
              this.crearPedidoAutomatico(this.orden!.id, datos, articulosDto);
            } else {
              this.finalizarGuardado('Orden actualizada');
            }
          });
        },
        error: (err: any) => {
          this.finalizarGuardado(err.error?.mensaje || 'Error al actualizar la orden', true);
        }
      });

    } else {
      // ================= CREAR =================
      this.ordenService.crear(dtoOrden).subscribe({
        next: (ordenCreada: any) => {
          let idOrden = ordenCreada?.id;

          if (!idOrden) {
            // ✅ CORREGIDO: Si el backend no devuelve el ID, buscamos el ID más alto (la orden más reciente)
            this.ordenService.listar().subscribe(ordenes => {
              if (ordenes && ordenes.length > 0) {
                const maxId = Math.max(...ordenes.map(o => o.id));
                this.crearPedidoAutomatico(maxId, datos, articulosDto);
              } else {
                this.finalizarGuardado('Orden creada, pero no se pudo asociar el pedido');
              }
            });
          } else {
            this.crearPedidoAutomatico(idOrden, datos, articulosDto);
          }
        },
        error: (err: any) => {
          this.finalizarGuardado(err.error?.mensaje || 'Error al crear la orden', true);
        }
      });
    }
  }

  private crearPedidoAutomatico(idOrden: number, datos: any, articulosDto: any[]) {
    if (articulosDto.length === 0) {
      this.finalizarGuardado('Orden creada sin artículos');
      return;
    }

    const dtoPedido = {
      codigo: `PED-AUTO-${idOrden}-${Date.now().toString().slice(-4)}`,
      cantidad: articulosDto.reduce((acc: number, a: any) => acc + a.cantidad, 0),
      id_Departamento: this.authService.getIdDepartamento() || 1,
      id_OrdenCompra: idOrden,
      fecha_Solicitada: datos.fecha_limite_ofertas || datos.fecha_Limite,
      urgente: false,
      observaciones: 'Creado automáticamente desde Orden de Compra',
      articulos: articulosDto
    };

    this.pedidoService.crear(dtoPedido as any).subscribe({
      next: () => this.finalizarGuardado('Orden y Pedido creados correctamente'),
      error: (err) => this.finalizarGuardado('Orden creada, pero error al crear el pedido', true)
    });
  }

  private finalizarGuardado(mensaje: string, isError: boolean = false) {
    this.guardando = false;
    if (isError) this.notification.error(mensaje);
    else this.notification.exito(mensaje);
    this.guardado.emit();
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}
