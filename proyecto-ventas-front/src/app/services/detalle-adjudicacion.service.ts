import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  DetalleAdjudicacion,
  CreateDetalleAdjudicacionDTO,
  UpdateDetalleAdjudicacionDTO
} from '../models/detalle-adjudicacion.model';

@Injectable({ providedIn: 'root' })
export class DetalleAdjudicacionService {
  private readonly url = `${environment.apiUrl}/DetalleAdjudicacionControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<DetalleAdjudicacion[]> {
    return this.http.get<DetalleAdjudicacion[]>(this.url);
  }

  obtener(idAdjudicacion: number, idPedido: number, idProveedor: number): Observable<DetalleAdjudicacion> {
    return this.http.get<DetalleAdjudicacion>(
      `${this.url}/${idAdjudicacion}/${idPedido}/${idProveedor}`
    );
  }

  crear(dto: CreateDetalleAdjudicacionDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateDetalleAdjudicacionDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(idAdjudicacion: number, idPedido: number, idProveedor: number): Observable<void> {
    return this.http.delete<void>(
      `${this.url}/${idAdjudicacion}/${idPedido}/${idProveedor}`
    );
  }
}