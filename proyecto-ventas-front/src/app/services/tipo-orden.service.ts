import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { TipoOrden, CreateTipoOrdenDTO, UpdateTipoOrdenDTO } from '../models/tipo-orden.model';

@Injectable({ providedIn: 'root' })
export class TipoOrdenService {
  private readonly url = `${environment.apiUrl}/TipoOrdenControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<TipoOrden[]> {
    return this.http.get<TipoOrden[]>(this.url);
  }

  obtener(id: number): Observable<TipoOrden> {
    return this.http.get<TipoOrden>(`${this.url}/${id}`);
  }

  crear(dto: CreateTipoOrdenDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateTipoOrdenDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
