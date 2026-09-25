import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { UsuarioService } from '../../../../services/usuario.service';
import { Usuario, CreateUsuarioDTO } from '../../../../models/usuario.model';
import { NotificacionService } from '../../../../services/notificacion';

@Component({
  selector: 'app-usuario-form',
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
    MatDialogModule
  ],
  templateUrl: './usuario-form.html',
  styleUrl: './usuario-form.scss'
})
export class UsuarioFormComponent implements OnInit {
  @Input() usuario: Usuario | null = null;
  @Output() guardado = new EventEmitter<void>();
  @Output() cancelado = new EventEmitter<void>();

  form: FormGroup;
  guardando = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private usuarioService: UsuarioService,
    private notificacion: NotificacionService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      contrasena: ['', [Validators.minLength(6)]],
      id_Rol: [2, [Validators.required]],
      activo: [true]
    });
  }

  ngOnInit(): void {
    if (this.usuario) {
      this.form.patchValue({
        nombre: this.usuario.nombre,
        email: this.usuario.email,
        id_Rol: this.usuario.id_Rol,
        activo: this.usuario.activo
      });
    } else {
      this.form.get('contrasena')?.setValidators([Validators.required, Validators.minLength(6)]);
      this.form.get('contrasena')?.updateValueAndValidity();
    }
  }

  get esEdicion(): boolean {
    return !!this.usuario;
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.guardando = true;
    this.error = '';

    const datos = this.form.value;

    if (this.esEdicion) {
      const dto: any = {
        id: this.usuario!.id,
        nombre: datos.nombre,
        email: datos.email,
        activo: datos.activo,
        id_Rol: datos.id_Rol
      };
      if (datos.contrasena) {
        dto.contrasena = datos.contrasena;
      }

      this.usuarioService.actualizar(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Usuario actualizado correctamente');
          this.guardado.emit();
        },
        error: (err: any) => {
          this.guardando = false;
          this.notificacion.error(`Error al actualizar: ${err.status} ${err.statusText}`);
        }
      });
    } else {
      const dto: CreateUsuarioDTO = {
        nombre: datos.nombre,
        email: datos.email,
        contrasena: datos.contrasena,
        activo: datos.activo,
        id_Rol: datos.id_Rol
      };

      this.usuarioService.crear(dto).subscribe({
        next: () => {
          this.guardando = false;
          this.notificacion.exito('Usuario creado correctamente');
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
