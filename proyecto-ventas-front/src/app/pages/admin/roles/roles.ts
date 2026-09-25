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
import { RolService } from '../../../services/rol.service';
import { Rol } from '../../../models/rol.model';
import { RolFormComponent } from './rol-form/rol-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RolFormComponent,
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
  templateUrl: './roles.html',
  styleUrl: './roles.scss'
})
export class RolesComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'descripcion', 'tipo', 'acciones'];
  dataSource = new MatTableDataSource<Rol>([]);

  cargando = false;
  error = '';
  mostrarFormulario = false;
  rolSeleccionado: Rol | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private rolService: RolService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (rol: Rol, filtro: string) => {
      const tipo = rol.externo ? 'externo' : 'interno';
      const dataStr = (rol.id + ' ' + rol.nombre + ' ' + rol.descripcion + ' ' + tipo).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.rolService.listar().subscribe({
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

  abrirFormulario(rol: Rol | null): void {
    this.rolSeleccionado = rol;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.rolSeleccionado = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(rol: Rol): void {
    this.confirm.eliminar(rol.nombre).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.rolService.eliminar(rol.id).subscribe({
        next: () => {
          this.notificacion.exito(`Rol "${rol.nombre}" eliminado correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          if (err.status === 500) {
            this.notificacion.error('No se puede eliminar: el rol está asociado a usuarios o permisos');
          } else {
            this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
          }
        }
      });
    });
  }
}
