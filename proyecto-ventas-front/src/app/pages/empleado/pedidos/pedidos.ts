import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-empleado-pedidos',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pedidos.html',
  styleUrl: './pedidos.scss'
})
export class EmpleadoPedidosComponent implements OnInit {
  usuario: any = null;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.usuario = this.authService.getUsuario();
  }
}
