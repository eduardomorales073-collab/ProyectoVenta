import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { UsuarioService } from '../../../../services/usuario.service';
import { ProveedorService } from '../../../../services/proveedor.service';
import { DepartamentoService } from '../../../../services/departamento.service';
import { Usuario, CreateUsuarioDTO } from '../../../../models/usuario.model';
import { Proveedor } from '../../../../models/proveedor.model';
import { Departamento } from '../../../../models/departamento.model';
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

  proveedores: Proveedor[] = [];
  departamentos: Departamento[] = [];

  constructor(
    private fb: FormBuilder,
    private usuarioService: UsuarioService,
    private proveedorService: ProveedorService,
    private departamentoService: DepartamentoService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      contrasena: ['', [Validators.minLength(6)]],
      id_Rol: [2, [Validators.required]],
      id_Proveedor: [null],
      id_Departamento: [null],
      activo: [true]
    });
  }

  ngOnInit(): void {
    // Cargar catálogos
    this.proveedorService.listar().subscribe({
      next: (data) => { this.proveedores = data; this.cdr.detectChanges(); }
    });
    this.departamentoService.listar().subscribe({
      next: (data) => { this.departamentos = data; this.cdr.detectChanges(); }
    });

    if (this.usuario) {
      this.form.patchValue({
        nombre: this.usuario.nombre,
        email: this.usuario.email,
        id_Rol: this.usuario.id_Rol,
        id_Proveedor: this.usuario.id_Proveedor || null,
        id_Departamento: this.usuario.id_Departamento || null,
        activo: this.usuario.activo
      });
    } else {
      this.form.get('contrasena')?.setValidators([Validators.required, Validators.minLength(6)]);
      this.form.get('contrasena')?.updateValueAndValidity();
    }

    // Escuchar cambios en el rol para aplicar/quitar validadores
    this.form.get('id_Rol')?.valueChanges.subscribe((rolId: number) => {
      this.actualizarValidadoresPorRol(rolId);
    });
    // Aplicar validadores al cargar
    this.actualizarValidadoresPorRol(this.form.get('id_Rol')?.value);
  }

  /** Aplica validadores según el rol seleccionado */
  actualizarValidadoresPorRol(rolId: number): void {
    const proveedorCtrl = this.form.get('id_Proveedor');
    const departamentoCtrl = this.form.get('id_Departamento');

    // Resetear validadores
    proveedorCtrl?.clearValidators();
    departamentoCtrl?.clearValidators();

    if (rolId === 3) {
      // AdministradorProveedor → proveedor obligatorio
      proveedorCtrl?.setValidators([Validators.required]);
    } else if (rolId === 5) {
      // CreadorPedidos → departamento obligatorio
      departamentoCtrl?.setValidators([Validators.required]);
    } else if (rolId === 2) {
      // GestorCompras → departamento recomendado (no obligatorio)
      // Se puede dejar opcional
    }

    proveedorCtrl?.updateValueAndValidity();
    departamentoCtrl?.updateValueAndValidity();
    this.cdr.detectChanges();
  }

  /** ¿Mostrar el dropdown de proveedores? */
  mostrarSelectorProveedor(): boolean {
    return this.form.get('id_Rol')?.value === 3; // AdministradorProveedor
  }

  /** ¿Mostrar el dropdown de departamentos? */
  mostrarSelectorDepartamento(): boolean {
    const rol = this.form.get('id_Rol')?.value;
    return rol === 2 || rol === 5; // GestorCompras o CreadorPedidos
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
        id_Rol: datos.id_Rol,
        id_Proveedor: datos.id_Proveedor || null,
        id_Departamento: datos.id_Departamento || null
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
        id_Rol: datos.id_Rol,
        id_Proveedor: datos.id_Proveedor || null,
        id_Departamento: datos.id_Departamento || null
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