import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { UsuarioService } from '../../../services/usuario.service';
import { Usuario } from '../../../models/usuario.model';
import { UsuarioFormComponent } from './usuario-form/usuario-form';
import { PermisosModalComponent } from './permisos-modal/permisos-modal';
import { NotificacionService } from '../../../services/notificacion';
import { ConfirmService } from '../../../services/confirm';   



@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UsuarioFormComponent,
    PermisosModalComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatChipsModule
  ],
  templateUrl: './usuarios.html',
  styleUrl: './usuarios.scss'
})
export class UsuariosComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'email', 'rol', 'activo', 'acciones'];
  dataSource = new MatTableDataSource<Usuario>([]);

  cargando = false;
  error = '';
  mostrarFormulario = false;
  usuarioSeleccionado: Usuario | null = null;
  mostrarPermisos = false;
  usuarioPermisos: Usuario | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;


  constructor(
    private usuarioService: UsuarioService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService  
  ) { }

  ngOnInit(): void {
    // NO cargamos aquí. Esperamos a que la vista esté lista.
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    // ✅ Personalizar el filtro para que busque en nombre, email y rol
    this.dataSource.filterPredicate = (usuario: Usuario, filtro: string) => {
      const nombreRol = this.nombreRol(usuario.id_Rol).toLowerCase();
      const dataStr = (
        usuario.nombre + ' ' +
        usuario.email + ' ' +
        nombreRol + ' ' +
        (usuario.activo ? 'activo' : 'inactivo')
      ).toLowerCase();

      return dataStr.includes(filtro);
    };

    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.usuarioService.listar().subscribe({
      next: (data) => {
        this.dataSource.data = data;
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

  aplicarFiltro(event: Event): void {
    const valor = (event.target as HTMLInputElement).value;
    this.dataSource.filter = valor.trim().toLowerCase();
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
    this.notificacion.exito('Usuario guardado correctamente');        // ← AÑADIR
  }
  

  confirmarEliminar(usuario: Usuario): void {
    this.confirm.eliminar(usuario.nombre).subscribe(confirmado => {
      if (!confirmado) return;

      this.usuarioService.eliminar(usuario.id).subscribe({
        next: () => {
          this.notificacion.exito(`Usuario "${usuario.nombre}" eliminado correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
        }
      });
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
