import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CategoriaProveedor,
  CreateCategoriaProveedorDTO,
  UpdateCategoriaProveedorDTO
} from '../models/categoria-proveedor.model';

@Injectable({ providedIn: 'root' })
export class CategoriaProveedorService {
  private readonly url = `${environment.apiUrl}/CategoriaProveedorControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<CategoriaProveedor[]> {
    return this.http.get<CategoriaProveedor[]>(this.url);
  }

  obtener(id: number): Observable<CategoriaProveedor> {
    return this.http.get<CategoriaProveedor>(`${this.url}/${id}`);
  }

  crear(dto: CreateCategoriaProveedorDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateCategoriaProveedorDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}