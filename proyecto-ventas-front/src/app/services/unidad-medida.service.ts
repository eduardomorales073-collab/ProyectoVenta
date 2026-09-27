import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  UnidadMedida,
  CreateUnidadMedidaDTO,
  UpdateUnidadMedidaDTO
} from '../models/unidad-medida.model';

@Injectable({ providedIn: 'root' })
export class UnidadMedidaService {
  private readonly url = `${environment.apiUrl}/UnidadMedidaControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<UnidadMedida[]> {
    return this.http.get<UnidadMedida[]>(this.url);
  }

  obtener(id: number): Observable<UnidadMedida> {
    return this.http.get<UnidadMedida>(`${this.url}/${id}`);
  }

  crear(dto: CreateUnidadMedidaDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateUnidadMedidaDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}