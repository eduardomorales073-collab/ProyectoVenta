import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { PerfilService } from '../../services/perfil';
import { NotificacionService } from '../../services/notificacion';
import { Perfil, UpdatePerfilDTO, ChangePasswordDTO } from '../../models/perfil.model';

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatChipsModule
  ],
  templateUrl: './perfil.html',
  styleUrl: './perfil.scss'
})
export class PerfilComponent implements OnInit {
  perfil: Perfil | null = null;
  cargando = false;
  error = '';

  // Formulario de perfil
  formPerfil: FormGroup;
  guardandoPerfil = false;

  // Formulario de contraseña
  formPassword: FormGroup;
  guardandoPassword = false;
  ocultarActual = true;
  ocultarNueva = true;
  ocultarConfirmar = true;

  constructor(
    private fb: FormBuilder,
    private perfilService: PerfilService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    this.formPerfil = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]]
    });

    this.formPassword = this.fb.group({
      contrasenaActual: ['', [Validators.required]],
      contrasenaNueva: ['', [Validators.required, Validators.minLength(6)]],
      confirmarContrasena: ['', [Validators.required]]
    }, { validators: this.passwordsMatch });
  }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.perfilService.obtener().subscribe({
      next: (data) => {
        this.perfil = data;
        this.formPerfil.patchValue({
          nombre: data.nombre,
          email: data.email
        });
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Validador personalizado para confirmar contraseña
  passwordsMatch(group: FormGroup): { [key: string]: boolean } | null {
    const nueva = group.get('contrasenaNueva')?.value;
    const confirmar = group.get('confirmarContrasena')?.value;
    return nueva === confirmar ? null : { noCoinciden: true };
  }

  get iniciales(): string {
    const nombre = this.perfil?.nombre || '';
    return nombre
      .split(' ')
      .map(p => p[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  get nombreRol(): string {
    switch (this.perfil?.id_Rol) {
      case 1: return 'Administrador';
      case 2: return 'Empleado';
      case 3: return 'Proveedor';
      default: return 'Desconocido';
    }
  }

  guardarPerfil(): void {
    if (this.formPerfil.invalid) return;
    this.guardandoPerfil = true;
    const dto: UpdatePerfilDTO = this.formPerfil.value;

    this.perfilService.actualizar(dto).subscribe({
      next: () => {
        this.guardandoPerfil = false;
        this.notificacion.exito('Perfil actualizado correctamente');
        this.cargar();
      },
      error: (err: any) => {
        this.guardandoPerfil = false;
        this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        this.cdr.detectChanges();
      }
    });
  }

  cambiarPassword(): void {
    if (this.formPassword.invalid) return;
    this.guardandoPassword = true;
    const dto: ChangePasswordDTO = this.formPassword.value;

    this.perfilService.cambiarContrasena(dto).subscribe({
      next: () => {
        this.guardandoPassword = false;
        this.notificacion.exito('Contraseña cambiada correctamente');
        this.formPassword.reset();
      },
      error: (err: any) => {
        this.guardandoPassword = false;
        this.notificacion.error(`Error: ${err.error?.mensaje || err.status + ' ' + err.statusText}`);
        this.cdr.detectChanges();
      }
    });
  }
}
