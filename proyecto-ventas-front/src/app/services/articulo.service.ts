import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Articulo {
  id: number;
  nombre: string;
  descripcion: string;
}

export interface CreateArticuloDTO {
  nombre: string;
  descripcion: string;
}

export interface UpdateArticuloDTO {
  id: number;
  nombre: string;
  descripcion: string;
}

@Injectable({ providedIn: 'root' })
export class ArticuloService {
  private readonly url = `${environment.apiUrl}/ArticuloControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Articulo[]> {
    return this.http.get<Articulo[]>(this.url);
  }

  obtener(id: number): Observable<Articulo> {
    return this.http.get<Articulo>(`${this.url}/${id}`);
  }

  crear(dto: CreateArticuloDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateArticuloDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
