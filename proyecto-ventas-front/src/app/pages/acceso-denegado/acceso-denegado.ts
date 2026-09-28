import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-acceso-denegado',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatButtonModule],
  templateUrl: './acceso-denegado.html',
  styleUrl: './acceso-denegado.scss'
})
export class AccesoDenegadoComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  irAInicio(): void {
    const ruta = this.authService.getRutaInicio();
    this.router.navigate([ruta]);
  }

  cerrarSesion(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}