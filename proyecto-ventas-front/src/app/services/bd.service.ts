import { Injectable } from '@angular/core';

export type TipoBd = 'local' | 'azure';

@Injectable({ providedIn: 'root' })
export class BdService {
  private readonly STORAGE_KEY = 'tipo-bd';
  private tipoBdActual: TipoBd = 'local';

  constructor() {
    const guardada = localStorage.getItem(this.STORAGE_KEY) as TipoBd;
    if (guardada === 'local' || guardada === 'azure') {
      this.tipoBdActual = guardada;
    }
  }

  getTipoBd(): TipoBd {
    return this.tipoBdActual;
  }

  setTipoBd(tipo: TipoBd, recordar: boolean = true): void {
    this.tipoBdActual = tipo;
    if (recordar) {
      localStorage.setItem(this.STORAGE_KEY, tipo);
    } else {
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  limpiarSeleccion(): void {
    this.tipoBdActual = 'local';
    localStorage.removeItem(this.STORAGE_KEY);
  }

  // ⚠️ Por ahora Azure no está configurado
  azureEstaConfigurado(): boolean {
    return false;
  }
}