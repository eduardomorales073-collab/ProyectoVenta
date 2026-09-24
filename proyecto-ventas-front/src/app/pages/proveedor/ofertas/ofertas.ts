import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-proveedor-ofertas',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ofertas.html',
  styleUrl: './ofertas.scss'
})
export class ProveedorOfertasComponent implements OnInit {
  usuario: any = null;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.usuario = this.authService.getUsuario();
  }
}
