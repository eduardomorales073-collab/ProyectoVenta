import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { OrdenCompraService, OrdenCompraConContadores, CreateOrdenCompraDTO } from '../../../../services/orden-compra.service';
import { TipoOrdenService } from '../../../../services/tipo-orden.service';
import { NotificacionService } from '../../../../services/notificacion';
import { TipoOrden } from '../../../../models/tipo-orden.model';

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
    MatIconModule
  ],
  templateUrl: './orden-compra-form.html',
  styleUrl: './orden-compra-form.scss'
})
export class OrdenCompraFormComponent implements OnInit {
  // ⚠️ CAMBIO: Acepta tanto OrdenCompra como OrdenCompraConContadores
  @Input() orden: OrdenCompraConContadores | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;
  tiposOrden: TipoOrden[] = [];

  constructor(
    private fb: FormBuilder,
    private ordenService: OrdenCompraService,
    private tipoOrdenService: TipoOrdenService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      descripcion: ['', [Validators.required, Validators.maxLength(200)]],
      fecha_Creacion: [this.hoy(), [Validators.required]],
      fecha_Limite: ['', [Validators.required]],
      fecha_limite_ofertas: [null],
      tipo_Orden: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.tipoOrdenService.listar().subscribe({
      next: (data) => { this.tiposOrden = data; this.cdr.detectChanges(); }
    });

    if (this.orden) {
      this.form.patchValue({
        descripcion: this.orden.descripcion,
        fecha_Creacion: this.orden.fecha_Creacion?.substring(0, 10),
        fecha_Limite: this.orden.fecha_Limite?.substring(0, 10),
        fecha_limite_ofertas: this.orden.fecha_limite_ofertas?.substring(0, 10) || null,
        tipo_Orden: this.orden.tipo_Orden
      });
      this.form.get('fecha_Creacion')?.disable();
    }
  }

  hoy(): string {
    return new Date().toISOString().substring(0, 10);
  }

  get esEdicion(): boolean {
    return !!this.orden;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.getRawValue();

    if (this.esEdicion) {
      const dto = {
        id: this.orden!.id,
        descripcion: datos.descripcion,
        fecha_Creacion: datos.fecha_Creacion,
        fecha_Limite: datos.fecha_Limite,
        tipo_Orden: datos.tipo_Orden
      };

      this.ordenService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Orden actualizada correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(err.error?.mensaje || `Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateOrdenCompraDTO = {
        descripcion: datos.descripcion,
        fecha_Creacion: datos.fecha_Creacion,
        fecha_Limite: datos.fecha_Limite,
        fecha_limite_ofertas: datos.fecha_limite_ofertas || null,
        tipo_Orden: datos.tipo_Orden
      };

      this.ordenService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Orden creada correctamente');
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

  // ===== HELPER PARA EL TIPO =====
  nombreTipo(tipo: TipoOrden | undefined): string {
    if (!tipo) return '';
    const partes: string[] = [];
    if (tipo.grande) partes.push('Grande');
    if (tipo.urgente) partes.push('Urgente');
    return partes.length > 0
      ? `Tipo #${tipo.id} — ${partes.join(' · ')}`
      : `Tipo #${tipo.id} — Normal`;
  }
}