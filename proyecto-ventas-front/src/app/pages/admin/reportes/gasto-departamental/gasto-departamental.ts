import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { RouterLink } from '@angular/router';
import { ReporteService } from '../../../../services/reporte.service';
import { NotificacionService } from '../../../../services/notificacion';
import { SucursalService } from '../../../../services/sucursal.service';
import { Sucursal } from '../../../../models/sucursal.model';
import { GastoDepartamental } from '../../../../models/reporte.model';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-gasto-departamental',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    MatChipsModule
  ],
  templateUrl: './gasto-departamental.html',
  styleUrl: './gasto-departamental.scss'
})
export class GastoDepartamentalComponent implements OnInit {
  sucursales: Sucursal[] = [];
  gastos: GastoDepartamental[] = [];
  displayedColumns: string[] = ['idDepartamento', 'nombreDepartamento', 'totalGastado'];
  cargando = false;
  form: FormGroup;
  buscado = false;
  sucursalSeleccionada = '';
  anioSeleccionado = 0;
  anios: number[] = [];

  constructor(
    private fb: FormBuilder,
    private reporteService: ReporteService,
    private sucursalService: SucursalService,
    private notificacion: NotificacionService,
    private cdr: ChangeDetectorRef
  ) {
    // Generar años: año actual y los 5 anteriores
    const anioActual = new Date().getFullYear();
    for (let i = 0; i < 6; i++) {
      this.anios.push(anioActual - i);
    }

    this.form = this.fb.group({
      idSucursal: ['', [Validators.required]],
      anio: [anioActual, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.cargarSucursales();
  }

  cargarSucursales(): void {
    this.sucursalService.listar().subscribe({
      next: (data: Sucursal[]) => {
        this.sucursales = data;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error al cargar sucursales: ${err.status}`);
      }
    });
  }

  buscar(): void {
    if (this.form.invalid) return;
    this.cargando = true;
    this.buscado = true;

    const { idSucursal, anio } = this.form.value;
    const sucursal = this.sucursales.find(s => s.id === idSucursal);
    this.sucursalSeleccionada = sucursal ? sucursal.nombre : '';
    this.anioSeleccionado = anio;

    this.reporteService.gastoDepartamental(idSucursal, anio).subscribe({
      next: (data: GastoDepartamental[]) => {
        this.gastos = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.notificacion.error(`Error: ${err.status} ${err.statusText}`);
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  get totalGeneral(): number {
    return this.gastos.reduce((sum, g) => sum + g.totalGastado, 0);
  }

  get maxGasto(): number {
    if (this.gastos.length === 0) return 0;
    return Math.max(...this.gastos.map(g => g.totalGastado));
  }

  getPorcentaje(total: number): number {
    if (this.totalGeneral === 0) return 0;
    return (total / this.totalGeneral) * 100;
  }
}
