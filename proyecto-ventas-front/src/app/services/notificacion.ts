import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotificacionService {

  constructor(private snackBar: MatSnackBar) { }

  /**
   * Muestra una notificación de éxito (verde)
   */
  exito(mensaje: string, duracion = 3000): void {
    this.snackBar.open(mensaje, 'Cerrar', {
      duration: duracion,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['snackbar-exito']
    });
  }

  /**
   * Muestra una notificación de error (rojo)
   */
  error(mensaje: string, duracion = 5000): void {
    this.snackBar.open(mensaje, 'Cerrar', {
      duration: duracion,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['snackbar-error']
    });
  }

  /**
   * Muestra una notificación de advertencia (amarillo)
   */
  advertencia(mensaje: string, duracion = 4000): void {
    this.snackBar.open(mensaje, 'Cerrar', {
      duration: duracion,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['snackbar-advertencia']
    });
  }

  /**
   * Muestra una notificación de información (azul)
   */
  info(mensaje: string, duracion = 3000): void {
    this.snackBar.open(mensaje, 'Cerrar', {
      duration: duracion,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['snackbar-info']
    });
  }
}
