import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Relacion, CreateRelacionDTO } from '../models/relacion.model';

@Injectable({ providedIn: 'root' })
export class RelacionService {
  private apiUrl = `${environment.apiUrl}/RelacionControlador`;

  constructor(private http: HttpClient) { }

  listar(): Observable<Relacion[]> {
    return this.http.get<Relacion[]>(this.apiUrl);
  }

  crear(dto: CreateRelacionDTO): Observable<any> {
    return this.http.post(this.apiUrl, dto);
  }

  eliminar(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
