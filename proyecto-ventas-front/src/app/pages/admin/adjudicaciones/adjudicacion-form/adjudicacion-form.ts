import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AdjudicacionService } from '../../../../services/adjudicacion.service';
import { OrdenCompraService, OrdenCompra } from '../../../../services/orden-compra.service';
import { Adjudicacion, CreateAdjudicacionDTO } from '../../../../models/adjudicacion.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-adjudicacion-form',
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
  templateUrl: './adjudicacion-form.html',
  styleUrl: './adjudicacion-form.scss'
})
export class AdjudicacionFormComponent implements OnInit {
  @Input() adjudicacion: Adjudicacion | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;
  ordenes: OrdenCompra[] = [];
  estados: string[] = ['Activa', 'Cerrada', 'Cancelada'];

  constructor(
    private fb: FormBuilder,
    private adjudicacionService: AdjudicacionService,
    private ordenCompraService: OrdenCompraService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      fecha_Resolucion: ['', [Validators.required]],
      orden_Compra: ['', [Validators.required]],
      estado: ['Activa', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.cargarOrdenes();

    if (this.adjudicacion) {
      this.form.patchValue({
        fecha_Resolucion: this.adjudicacion.fecha_Resolucion?.substring(0, 10),
        orden_Compra: this.adjudicacion.orden_Compra,
        estado: this.adjudicacion.estado
      });
    }
  }

  cargarOrdenes(): void {
    this.ordenCompraService.listar().subscribe({
      next: (data: OrdenCompra[]) => this.ordenes = data,
      error: (err: any) => this.notificacion.error(`Error al cargar órdenes: ${err.status}`)
    });
  }

  get esEdicion(): boolean {
    return !!this.adjudicacion;
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.adjudicacion!.id,
        fecha_Resolucion: datos.fecha_Resolucion,
        orden_Compra: datos.orden_Compra,
        estado: datos.estado
      };
      this.adjudicacionService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Adjudicación actualizada correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(err.error?.mensaje || `Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateAdjudicacionDTO = {
        fecha_Resolucion: datos.fecha_Resolucion,
        orden_Compra: datos.orden_Compra,
        estado: datos.estado
      };
      this.adjudicacionService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Adjudicación creada correctamente');
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
}
