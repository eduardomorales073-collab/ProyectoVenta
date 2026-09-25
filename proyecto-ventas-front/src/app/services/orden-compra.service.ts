import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface OrdenCompra {
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
}
