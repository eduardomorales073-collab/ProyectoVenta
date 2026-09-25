import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Proveedor, CreateProveedorDTO, UpdateProveedorDTO } from '../models/proveedor.model';

@Injectable({ providedIn: 'root' })
export class ProveedorService {
  private readonly url = `${environment.apiUrl}/ProveedorControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Proveedor[]> {
    return this.http.get<Proveedor[]>(this.url);
  }

  obtener(id: number): Observable<Proveedor> {
    return this.http.get<Proveedor>(`${this.url}/${id}`);
  }

  crear(dto: CreateProveedorDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateProveedorDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
