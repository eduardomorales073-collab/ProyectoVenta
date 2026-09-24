import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuarioService } from '../../../services/usuario.service';
import { Usuario } from '../../../models/usuario.model';
import { UsuarioFormComponent } from './usuario-form/usuario-form';
import { PermisosModalComponent } from './permisos-modal/permisos-modal';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [CommonModule, UsuarioFormComponent, PermisosModalComponent],
  templateUrl: './usuarios.html',
  styleUrl: './usuarios.scss'
})
export class UsuariosComponent implements OnInit {
  usuarios: Usuario[] = [];
  cargando = false;
  error = '';
  mostrarFormulario = false;
  usuarioSeleccionado: Usuario | null = null;

  // ⬇️ NUEVAS PROPIEDADES PARA PERMISOS
  mostrarPermisos = false;
  usuarioPermisos: Usuario | null = null;

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
    this.usuarioService.listar().subscribe({
      next: (data) => {
        this.usuarios = data;
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

  abrirFormulario(usuario: Usuario | null): void {
    this.usuarioSeleccionado = usuario;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.usuarioSeleccionado = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(usuario: Usuario): void {
    if (!confirm(`¿Eliminar a "${usuario.nombre}"?`)) return;

    this.usuarioService.eliminar(usuario.id).subscribe({
      next: () => this.cargar(),
      error: (err: any) => alert(`Error: ${err.status} ${err.statusText}`)
    });
  }

  nombreRol(idRol: number): string {
    switch (idRol) {
      case 1: return 'Administrador';
      case 2: return 'Empleado';
      case 3: return 'Proveedor';
      default: return 'Desconocido';
    }
  }

  // ⬇️ MÉTODOS NUEVOS PARA PERMISOS
  verPermisos(usuario: Usuario): void {
    this.usuarioPermisos = usuario;
    this.mostrarPermisos = true;
    this.cdr.detectChanges();
  }

  cerrarPermisos(): void {
    this.mostrarPermisos = false;
    this.usuarioPermisos = null;
    this.cdr.detectChanges();
  }
  onPermisosActualizados(): void {
    this.cerrarPermisos();
    this.cargar();
  }
}
