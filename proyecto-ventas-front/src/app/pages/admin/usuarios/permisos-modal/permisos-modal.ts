import { Component, EventEmitter, Input, Output, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsuarioService } from '../../../../services/usuario.service';
import { PermisoUsuario, UpdatePermisosDTO } from '../../../../models/permiso.model';

@Component({
  selector: 'app-permisos-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './permisos-modal.html',
  styleUrl: './permisos-modal.scss'
})
export class PermisosModalComponent implements OnInit {
  @Input() idUsuario!: number;
  @Input() nombreUsuario!: string;
  @Output() cerrar = new EventEmitter<void>();
  @Output() actualizado = new EventEmitter<void>();

  permisos: PermisoUsuario | null = null;
  cargando = false;
  guardando = false;
  error = '';
  exito = '';

  constructor(
    private usuarioService: UsuarioService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.exito = '';
    this.usuarioService.obtenerPermisos(this.idUsuario).subscribe({
      next: (data) => {
        this.permisos = data;
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

  togglePermiso(campo: 'crear' | 'leer' | 'actualizar' | 'borrar', event?: Event): void {
    if (event) event.stopPropagation();
    if (!this.permisos) return;
    this.permisos[campo] = !this.permisos[campo];
    this.cdr.detectChanges();
  }

  guardar(): void {
    if (!this.permisos) return;

    this.guardando = true;
    this.error = '';
    this.exito = '';

    const dto: UpdatePermisosDTO = {
      id: this.permisos.idPermiso,      // ← AÑADIR
      crear: this.permisos.crear,
      leer: this.permisos.leer,
      actualizar: this.permisos.actualizar,
      borrar: this.permisos.borrar,
      fecha: new Date().toISOString()   // ← AÑADIR
    };

    this.usuarioService.actualizarPermisos(this.idUsuario, dto).subscribe({
      next: () => {
        this.guardando = false;
        this.exito = '✅ Permisos actualizados';
        this.cdr.detectChanges();
        setTimeout(() => {
          this.actualizado.emit();
          this.onCerrar();
        }, 1500);
      },
      error: (err: any) => {
        this.guardando = false;
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cdr.detectChanges();
      }
    });
  }

  onCerrar(): void {
    this.cerrar.emit();
  }
}
