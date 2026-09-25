import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { RolService } from '../../../../services/rol.service';
import { Rol, CreateRolDTO } from '../../../../models/rol.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-rol-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSlideToggleModule
  ],
  templateUrl: './rol-form.html',
  styleUrl: './rol-form.scss'
})
export class RolFormComponent implements OnInit {
  @Input() rol: Rol | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;

  constructor(
    private fb: FormBuilder,
    private rolService: RolService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(50)]],
      descripcion: ['', [Validators.required, Validators.maxLength(200)]],
      externo: [false]
    });
  }

  ngOnInit(): void {
    if (this.rol) {
      this.form.patchValue({
        nombre: this.rol.nombre,
        descripcion: this.rol.descripcion,
        externo: this.rol.externo
      });
    }
  }

  get esEdicion(): boolean {
    return !!this.rol;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    const datos = this.form.value;

    if (this.esEdicion) {
      const dto = {
        id: this.rol!.id,
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        externo: datos.externo
      };
      this.rolService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Rol actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateRolDTO = {
        nombre: datos.nombre,
        descripcion: datos.descripcion,
        externo: datos.externo
      };
      this.rolService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Rol creado correctamente');
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
