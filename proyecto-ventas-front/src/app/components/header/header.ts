import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  estaAutenticado = signal(false);
  usuario = signal<{ nombre?: string; rol?: string; idRol?: number } | null>(null);
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

  // Getters reactivos para el template
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

  cerrarSesion(): void {
    this.authService.logout();
    this.estaAutenticado.set(false);
    this.usuario.set(null);
    this.router.navigate(['/login']);
  }
}
