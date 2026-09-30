import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { EventoAuditoria, FiltroAuditoria } from '../models/evento-auditoria.model';

@Injectable({ providedIn: 'root' })
export class AuditoriaService {
  private readonly url = `${environment.apiUrl}/AuditoriaControlador`;

  constructor(private http: HttpClient) { }

  /**
   * Obtener el historial de auditoría con filtros opcionales.
   */
  getHistorial(filtro?: FiltroAuditoria): Observable<EventoAuditoria[]> {
    let params = new HttpParams();
    if (filtro) {
      if (filtro.tipo) params = params.set('tipo', filtro.tipo);
      if (filtro.fechaDesde) params = params.set('fechaDesde', filtro.fechaDesde);
      if (filtro.fechaHasta) params = params.set('fechaHasta', filtro.fechaHasta);
    }
    return this.http.get<EventoAuditoria[]>(`${this.url}/historial`, { params });
  }
}