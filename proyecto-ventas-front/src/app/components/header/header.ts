import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatBadgeModule } from '@angular/material/badge';
import { AuthService } from '../../services/auth.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    MatBadgeModule
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  estaAutenticado = signal(false);
  usuario = signal<{ nombre?: string; email?: string; rol?: string; idRol?: number } | null>(null);   // ← CAMBIO
  enLogin = signal(false);

  ngOnInit(): void {
    this.actualizarEstado();

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.enLogin.set(event.urlAfterRedirects === '/login');
        this.actualizarEstado();
      });
  }

  private actualizarEstado(): void {
    this.estaAutenticado.set(this.authService.isLoggedIn());
    this.usuario.set(this.authService.getUsuario());
  }

  get esAdmin(): boolean {
    return this.usuario()?.idRol === 1;
  }

  get esEmpleado(): boolean {
    return this.usuario()?.idRol === 2;
  }

  get esProveedor(): boolean {
    return this.usuario()?.idRol === 3;
  }

  get nombreRol(): string {
    switch (this.usuario()?.idRol) {
      case 1: return 'Administrador';
      case 2: return 'Empleado';
      case 3: return 'Proveedor';
      default: return '';
    }
  }

  get iniciales(): string {
    const nombre = this.usuario()?.nombre || '';
    return nombre
      .split(' ')
      .map(p => p[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  cerrarSesion(): void {
    this.authService.logout();
    this.estaAutenticado.set(false);
    this.usuario.set(null);
    this.router.navigate(['/login']);
  }
}
