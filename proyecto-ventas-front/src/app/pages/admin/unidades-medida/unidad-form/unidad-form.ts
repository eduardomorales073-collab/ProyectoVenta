import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { UnidadMedidaService } from '../../../../services/unidad-medida.service';
import { UnidadMedida, CreateUnidadMedidaDTO } from '../../../../models/unidad-medida.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-unidad-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './unidad-form.html',
  styleUrl: './unidad-form.scss'
})
export class UnidadFormComponent implements OnInit {
  @Input() unidad: UnidadMedida | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private unidadService: UnidadMedidaService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(20)]],
      abreviatura: ['', [Validators.maxLength(10)]],
      descripcion: ['', [Validators.maxLength(200)]]
    });
  }

  ngOnInit(): void {
    if (this.unidad) {
      this.form.patchValue({
        nombre: this.unidad.nombre,
        abreviatura: this.unidad.abreviatura || '',
        descripcion: this.unidad.descripcion || ''
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.unidad;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.unidad!.id,
        nombre: datos.nombre,
        abreviatura: datos.abreviatura || '',
        descripcion: datos.descripcion || ''
      };

      this.unidadService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Unidad de medida actualizada correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error al actualizar: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateUnidadMedidaDTO = {
        nombre: datos.nombre,
        abreviatura: datos.abreviatura || '',
        descripcion: datos.descripcion || ''
      };

      this.unidadService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Unidad de medida creada correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error al crear: ${err.status} ${err.statusText}`);
        }
      });
    }
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}