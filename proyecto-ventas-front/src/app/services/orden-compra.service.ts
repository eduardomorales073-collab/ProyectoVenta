import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface OrdenCompra {
  id: number;
  descripcion: string;
  fecha_Creacion: string;
  fecha_Limite: string;
  fecha_limite_ofertas: string | null;
  tipo_Orden: number;
}

export interface CreateOrdenCompraDTO {
  descripcion: string;
  fecha_Creacion: string;
  fecha_Limite: string;
  fecha_limite_ofertas: string | null;
  tipo_Orden: number;
}

export interface UpdateOrdenCompraDTO {
  id: number;
  descripcion: string;
  fecha_Creacion: string;
  fecha_Limite: string;
  tipo_Orden: number;
}

@Injectable({ providedIn: 'root' })
export class OrdenCompraService {
  private readonly url = `${environment.apiUrl}/OrdenCompraControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<OrdenCompra[]> {
    return this.http.get<OrdenCompra[]>(this.url);
  }

  obtener(id: number): Observable<OrdenCompra> {
    return this.http.get<OrdenCompra>(`${this.url}/${id}`);
  }

  crear(dto: CreateOrdenCompraDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateOrdenCompraDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}