import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  form: FormGroup;
  error = '';
  cargando = false;
  ocultarPassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      console.log('[Login] Formulario inválido:', this.form.errors);
      return;
    }

    this.cargando = true;
    this.error = '';

    // ✅ Limpiar espacios en blanco del email
    const datos = {
      email: (this.form.value.email || '').trim().toLowerCase(),
      password: this.form.value.password
    };

    console.log('[Login] Datos a enviar:', datos);

    this.authService.login(datos).subscribe({
      next: (respuesta: any) => {
        console.log('[Login] ✅ Login exitoso:', respuesta);
        this.cargando = false;
        const ruta = this.authService.getRutaInicio();
        this.router.navigate([ruta]);
      },
      error: (err: any) => {
        console.error('[Login] ❌ Error:', err);
        console.error('[Login] Status:', err.status);
        console.error('[Login] Error body:', err.error);
        console.error('[Login] Error message:', err.message);
        this.cargando = false;
        this.error = 'Credenciales incorrectas. Verifica tu email y contraseña.';
      }
    });
  }

  togglePassword(): void {
    this.ocultarPassword = !this.ocultarPassword;
  }
}