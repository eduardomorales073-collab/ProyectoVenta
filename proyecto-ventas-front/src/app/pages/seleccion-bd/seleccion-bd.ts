import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { BdService, TipoBd } from '../../services/bd.service';

@Component({
  selector: 'app-seleccion-bd',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatCheckboxModule,
    MatChipsModule
  ],
  templateUrl: './seleccion-bd.html',
  styleUrl: './seleccion-bd.scss'
})
export class SeleccionBdComponent implements OnInit {
  tipoBdSeleccionado: TipoBd = 'local';
  recordarSeleccion = true;
  azureConfigurado = false;

  constructor(
    private bdService: BdService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Leer la BD guardada
    const bdGuardada = this.bdService.getTipoBd();
    if (bdGuardada) {
      this.tipoBdSeleccionado = bdGuardada;
    }
    this.azureConfigurado = this.bdService.azureEstaConfigurado();
  }

  seleccionar(tipo: TipoBd): void {
    if (tipo === 'azure' && !this.azureConfigurado) {
      return; // No permitir seleccionar Azure si no está configurado
    }
    this.tipoBdSeleccionado = tipo;
  }

  continuar(): void {
    this.bdService.setTipoBd(this.tipoBdSeleccionado, this.recordarSeleccion);
    this.router.navigate(['/login']);
  }
}