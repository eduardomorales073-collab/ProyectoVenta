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
import { RubroService } from '../../../services/rubro.service';
import { Rubro } from '../../../models/rubro.model';
import { RubroFormComponent } from './rubro-form/rubro-form';
import { ConfirmService } from '../../../services/confirm';
import { NotificacionService } from '../../../services/notificacion';

@Component({
  selector: 'app-rubros',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RubroFormComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './rubros.html',
  styleUrl: './rubros.scss'
})
export class RubrosComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['id', 'nombre', 'descripcion', 'acciones'];
  dataSource = new MatTableDataSource<Rubro>([]);

  cargando = false;
  error = '';
  mostrarFormulario = false;
  rubroSeleccionado: Rubro | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private rubroService: RubroService,
    private cdr: ChangeDetectorRef,
    private notificacion: NotificacionService,
    private confirm: ConfirmService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.filterPredicate = (rubro: Rubro, filtro: string) => {
      const dataStr = (rubro.id + ' ' + rubro.nombre + ' ' + rubro.descripcion).toLowerCase();
      return dataStr.includes(filtro);
    };

    this.cargar();
  }

  cargar(): void {
    this.cargando = true;
    this.error = '';
    this.rubroService.listar().subscribe({
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

  abrirFormulario(rubro: Rubro | null): void {
    this.rubroSeleccionado = rubro;
    this.mostrarFormulario = true;
    this.cdr.detectChanges();
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.rubroSeleccionado = null;
    this.cdr.detectChanges();
  }

  onGuardado(): void {
    this.cerrarFormulario();
    this.cargar();
  }

  confirmarEliminar(rubro: Rubro): void {
    this.confirm.eliminar(rubro.nombre).subscribe((confirmado: boolean) => {
      if (!confirmado) return;

      this.rubroService.eliminar(rubro.id).subscribe({
        next: () => {
          this.notificacion.exito(`Rubro "${rubro.nombre}" eliminado correctamente`);
          this.cargar();
        },
        error: (err: any) => {
          if (err.status === 500) {
            this.notificacion.error('No se puede eliminar: el rubro está asociado a proveedores');
          } else {
            this.notificacion.error(`Error al eliminar: ${err.status} ${err.statusText}`);
          }
        }
      });
    });
  }
}
