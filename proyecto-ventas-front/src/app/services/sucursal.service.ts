import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Sucursal, CreateSucursalDTO, UpdateSucursalDTO } from '../models/sucursal.model';

@Injectable({ providedIn: 'root' })
export class SucursalService {
  private readonly url = `${environment.apiUrl}/SucursalControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Sucursal[]> {
    return this.http.get<Sucursal[]>(this.url);
  }

  obtener(id: number): Observable<Sucursal> {
    return this.http.get<Sucursal>(`${this.url}/${id}`);
  }

  crear(dto: CreateSucursalDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateSucursalDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
