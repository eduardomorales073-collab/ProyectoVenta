import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { OfertaProveedor, CreateOfertaProveedorDTO } from '../models/oferta-proveedor.model';

@Injectable({ providedIn: 'root' })
export class OfertaProveedorService {
  private readonly url = `${environment.apiUrl}/OfertaProveedorControlador`;

  constructor(private http: HttpClient) { }

  /** Obtener las ofertas del proveedor logueado */
  misOfertas(): Observable<OfertaProveedor[]> {
    return this.http.get<OfertaProveedor[]>(`${this.url}/mis-ofertas`);
  }

  /** Crear una nueva oferta */
  ofertar(dto: CreateOfertaProveedorDTO): Observable<any> {
    return this.http.post<any>(`${this.url}/ofertar`, dto);
  }

  /** Actualizar una oferta existente */
  actualizar(dto: any): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  /** Eliminar una oferta */
  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}