import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  HistorialCompra,
  RankingProveedor,
  OfertaComparativa,
  PedidoPendiente,
  OrdenActiva,
  GastoDepartamental,
  EficienciaCompra,
  VariacionPrecio
} from '../models/reporte.model';

@Injectable({ providedIn: 'root' })
export class ReporteService {
  private readonly url = `${environment.apiUrl}/ReporteControlador`;

  constructor(private http: HttpClient) { }

  historialPorArticulo(idArticulo: number): Observable<HistorialCompra[]> {
    return this.http.get<HistorialCompra[]>(`${this.url}/historial-articulo/${idArticulo}`);
  }

  rankingProveedores(desde: string, hasta: string): Observable<RankingProveedor[]> {
    const params = new HttpParams().set('desde', desde).set('hasta', hasta);
    return this.http.get<RankingProveedor[]>(`${this.url}/ranking-proveedores`, { params });
  }

  ofertasPorOrden(idOrdenCompra: number): Observable<OfertaComparativa[]> {
    return this.http.get<OfertaComparativa[]>(`${this.url}/ofertas-orden/${idOrdenCompra}`);
  }

  pedidosPendientes(): Observable<PedidoPendiente[]> {
    return this.http.get<PedidoPendiente[]>(`${this.url}/pedidos-pendientes`);
  }

  ordenesActivas(): Observable<OrdenActiva[]> {
    return this.http.get<OrdenActiva[]>(`${this.url}/ordenes-activas`);
  }

  gastoDepartamental(idSucursal: number, anio: number): Observable<GastoDepartamental[]> {
    return this.http.get<GastoDepartamental[]>(`${this.url}/gasto-departamental/${idSucursal}/${anio}`);
  }

  eficienciaProceso(): Observable<EficienciaCompra> {
    return this.http.get<EficienciaCompra>(`${this.url}/eficiencia-proceso`);
  }

  variacionPrecios(idArticulo: number, idProveedor: number): Observable<VariacionPrecio[]> {
    return this.http.get<VariacionPrecio[]>(`${this.url}/variacion-precios/${idArticulo}/${idProveedor}`);
  }
}
