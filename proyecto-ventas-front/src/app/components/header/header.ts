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
  usuario = signal<{ nombre?: string; email?: string } | null>(null);
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

    const raw = localStorage.getItem('usuario');
    this.usuario.set(raw ? JSON.parse(raw) : null);
  }

  cerrarSesion(): void {
    this.authService.logout();
    this.estaAutenticado.set(false);
    this.usuario.set(null);
    this.router.navigate(['/login']);
  }
}
