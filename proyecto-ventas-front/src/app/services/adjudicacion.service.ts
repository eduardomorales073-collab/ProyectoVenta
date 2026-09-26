import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Adjudicacion, CreateAdjudicacionDTO, UpdateAdjudicacionDTO } from '../models/adjudicacion.model';

@Injectable({ providedIn: 'root' })
export class AdjudicacionService {
  private readonly url = `${environment.apiUrl}/AdjudicacionControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Adjudicacion[]> {
    return this.http.get<Adjudicacion[]>(this.url);
  }

  obtener(id: number): Observable<Adjudicacion> {
    return this.http.get<Adjudicacion>(`${this.url}/${id}`);
  }

  crear(dto: CreateAdjudicacionDTO): Observable<void> {
    return this.http.post<void>(this.url, dto);
  }

  actualizar(dto: UpdateAdjudicacionDTO): Observable<void> {
    return this.http.put<void>(this.url, dto);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
