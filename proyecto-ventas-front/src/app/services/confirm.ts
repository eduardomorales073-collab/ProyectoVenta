import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent, ConfirmDialogData } from '../components/confirm-dialog/confirm-dialog';

@Injectable({ providedIn: 'root' })
export class ConfirmService {

  constructor(private dialog: MatDialog) { }

  /**
   * Muestra un diálogo de confirmación
   */
  confirmar(data: ConfirmDialogData): Observable<boolean> {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data,
      width: '400px',
      disableClose: true,
      autoFocus: true
    });

    return dialogRef.afterClosed();
  }

  /**
   * Atajo para confirmar eliminación
   */
  eliminar(nombre: string): Observable<boolean> {
    return this.confirmar({
      titulo: 'Confirmar eliminación',
      mensaje: `¿Estás seguro de que quieres eliminar "${nombre}"? Esta acción no se puede deshacer.`,
      textoConfirmar: 'Eliminar',
      textoCancelar: 'Cancelar',
      color: 'warn',
      icono: 'delete_forever'
    });
  }

  /**
   * Atajo para confirmar acciones genéricas
   */
  aceptar(titulo: string, mensaje: string): Observable<boolean> {
    return this.confirmar({
      titulo,
      mensaje,
      textoConfirmar: 'Aceptar',
      color: 'primary',
      icono: 'help'
    });
  }
}
