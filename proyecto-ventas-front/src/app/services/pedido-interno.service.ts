import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PedidoInterno, CreatePedidoInternoDTO, UpdatePedidoInternoDTO } from '../models/pedido-interno.model';

@Injectable({ providedIn: 'root' })
export class PedidoInternoService {
  private readonly url = `${environment.apiUrl}/PedidoInternoControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<PedidoInterno[]> {
    return this.http.get<PedidoInterno[]>(this.url);
  }

  obtener(id: number): Observable<PedidoInterno> {
    return this.http.get<PedidoInterno>(`${this.url}/${id}`);
  }

  crear(dto: CreatePedidoInternoDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdatePedidoInternoDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
