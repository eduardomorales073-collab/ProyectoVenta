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

export interface OrdenCompraConContadores {
  id: number;
  descripcion: string;
  fecha_Creacion: string;
  fecha_Limite: string;
  fecha_limite_ofertas: string | null;
  tipo_Orden: number;
  totalPedidos: number;
  pedidosAdjudicados: number;
  pedidosPendientes: number;
  estado: string;  // "Abierta" | "Cerrada"
}

export interface PedidoDeOrden {
  id: number;
  codigo: string;
  cantidad: number;
  id_Departamento: number;
  nombreDepartamento: string | null;
  id_OrdenCompra: number | null;
  fecha_Solicitada: string;
  fecha_Ingreso: string;
  nombreSucursal: string | null;
  id_Sucursal: number | null;
  urgente: boolean;
  totalOfertas: number;
  adjudicado: boolean;
  idProveedorGanador: number | null;
  nombreProveedorGanador: string | null;
  precioAdjudicado: number | null;
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

  listarConContadores(): Observable<OrdenCompraConContadores[]> {
    return this.http.get<OrdenCompraConContadores[]>(`${this.url}/con-contadores`);
  }

  obtenerPedidosDeOrden(idOrden: number): Observable<PedidoDeOrden[]> {
    return this.http.get<PedidoDeOrden[]>(`${this.url}/${idOrden}/pedidos`);
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