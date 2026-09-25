import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DepartamentoService } from '../../../../services/departamento.service';
import { Departamento, CreateDepartamentoDTO } from '../../../../models/departamento.model';
import { Sucursal } from '../../../../models/sucursal.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-departamento-form',
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
  templateUrl: './departamento-form.html',
  styleUrl: './departamento-form.scss'
})
export class DepartamentoFormComponent implements OnInit {
  @Input() departamento: Departamento | null = null;
  @Input() sucursales: Sucursal[] = [];
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private departamentoService: DepartamentoService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      descripcion: ['', [Validators.required, Validators.maxLength(200)]],
      id_Sucursal: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    if (this.departamento) {
      this.form.patchValue({
        nombre: this.departamento.nombre,
        descripcion: this.departamento.descripcion,
        id_Sucursal: this.departamento.id_Sucursal
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.departamento;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.departamento!.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        id_Sucursal: datos.id_Sucursal
      };
      this.departamentoService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Departamento actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateDepartamentoDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        id_Sucursal: datos.id_Sucursal
      };
      this.departamentoService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Departamento creado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    }
  }

  onCancelar(): void {
    this.cancelado.emit();
  }
}
