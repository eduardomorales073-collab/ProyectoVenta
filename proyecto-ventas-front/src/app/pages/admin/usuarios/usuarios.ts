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
import { forkJoin } from 'rxjs';

import { UsuarioService } from '../../../services/usuario.service';
import { DepartamentoService } from '../../../services/departamento.service';
import { SucursalService } from '../../../services/sucursal.service';
import { ProveedorService } from '../../../services/proveedor.service';
import { RolService } from '../../../services/rol.service'; // ✅ IMPORTAMOS EL SERVICIO DE ROLES
import { Usuario } from '../../../models/usuario.model';
import { Departamento } from '../../../models/departamento.model';
import { Sucursal } from '../../../models/sucursal.model';
import { Proveedor } from '../../../models/proveedor.model';
import { UsuarioFormComponent } from './usuario-form/usuario-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UsuarioFormComponent,
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
  displayedColumns: string[] = ['id', 'nombre', 'email', 'rol', 'asignacion', 'activo', 'acciones'];
  dataSource = new MatTableDataSource<Usuario>([]);

  usuarios: Usuario[] = [];
  departamentos: Departamento[] = [];
  sucursales: Sucursal[] = [];
  proveedores: Proveedor[] = [];
  roles: any[] = []; // ✅ NUEVA LISTA DE ROLES

  cargando = false;
  error = '';
  mostrarFormulario = false;
  usuarioSeleccionado: Usuario | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private usuarioService: UsuarioService,
    private departamentoService: DepartamentoService,
    private sucursalService: SucursalService,
    private proveedorService: ProveedorService,
    private rolService: RolService, // ✅ INYECTAMOS EL SERVICIO
    private cdr: ChangeDetectorRef,
    private confirm: ConfirmService,
    private notification: NotificacionService
  ) { }

  ngOnInit(): void { }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (usuario: Usuario, filtro: string) => {
      const rolStr = this.getNombreRol(usuario);
      const dataStr = (
        usuario.id + ' ' +
        (usuario.nombre || '') + ' ' +
        (usuario.email || '') + ' ' +
        rolStr + ' ' +
        this.getAsignacion(usuario)
      ).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargarTodo();
  }

  cargarTodo(): void {
    this.cargando = true;

    // ✅ Agregamos los roles al forkJoin
    forkJoin({
      usuarios: this.usuarioService.listar(),
      departamentos: this.departamentoService.listar(),
      sucursales: this.sucursalService.listar(),
      proveedores: this.proveedorService.listar(),
      roles: this.rolService.listar()
    }).subscribe({
      next: (res: any) => {
        this.usuarios = Array.isArray(res.usuarios) ? res.usuarios : (res.usuarios.$values || []);
        this.departamentos = Array.isArray(res.departamentos) ? res.departamentos : (res.departamentos.$values || []);
        this.sucursales = Array.isArray(res.sucursales) ? res.sucursales : (res.sucursales.$values || []);
        this.proveedores = Array.isArray(res.proveedores) ? res.proveedores : (res.proveedores.$values || []);
        this.roles = Array.isArray(res.roles) ? res.roles : (res.roles.$values || []);

        this.dataSource.data = this.usuarios;
        this.cargando = false;

        if (this.paginator) {
          this.paginator.firstPage();
        }

        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.error = `Error: ${err.status} ${err.statusText}`;
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ✅ NUEVO MÉTODO: Busca el nombre del rol usando el id_Rol del JSON
  getNombreRol(usuario: any): string {
    if (!usuario.id_Rol) return 'Sin Rol';

    const rolEncontrado = this.roles.find(r => r.id === usuario.id_Rol);
    return rolEncontrado ? (rolEncontrado.nombre || rolEncontrado.Nombre) : `Rol #${usuario.id_Rol}`;
  }

  getAsignacion(usuario: Usuario): string {
    if (usuario.id_Proveedor) {
      const prov = this.proveedores.find(p => p.id === usuario.id_Proveedor);
      return prov ? `Empresa: ${prov.nombre}` : `Proveedor #${usuario.id_Proveedor}`;
    }

    if (usuario.id_Departamento) {
      const depto = this.departamentos.find(d => d.id === usuario.id_Departamento);
      if (depto) {
        const sucursal = this.sucursales.find(s => s.id === depto.id_Sucursal);
        const nombreSucursal = sucursal ? sucursal.nombre : 'Sin Sucursal';
        return `${nombreSucursal} → ${depto.nombre}`;
      }
      return `Depto #${usuario.id_Departamento}`;
    }

    return 'Sin asignación';
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
    this.cargarTodo();
  }

  confirmarEliminar(usuario: Usuario): void {
    this.confirm.eliminar(`Usuario ${usuario.nombre}`).subscribe((confirmado: boolean) => {
      if (!confirmado) return;
      this.usuarioService.eliminar(usuario.id).subscribe({
        next: () => {
          this.notification.exito('Usuario eliminado correctamente');
          this.cargarTodo();
        },
        error: (err: any) => {
          this.notification.error(`Error al eliminar: ${err.error?.mensaje || err.status + ' ' + err.statusText}`);
        }
      });
    });
  }

  claseRol(rol: string): string {
    switch (rol?.toLowerCase()) {
      case 'administrador del sistema': return 'chip-admin';
      case 'gestor de compras': return 'chip-gestor';
      case 'administrador de proveedor': return 'chip-proveedor';
      case 'auditor': return 'chip-auditor';
      case 'creador de pedidos': return 'chip-creador';
      default: return 'chip-default';
    }
  }
}
