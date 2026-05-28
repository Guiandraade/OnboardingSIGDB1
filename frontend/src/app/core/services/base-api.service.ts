import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination.model';

export abstract class BaseApiService<TResponse, TRequest> {

  protected abstract apiUrl: string;

  constructor(protected http: HttpClient) {}

  getById(id: number): Observable<TResponse> {
    return this.http.get<TResponse>(`${this.apiUrl}/${id}`);
  }

  create(request: TRequest): Observable<TResponse> {
    return this.http.post<TResponse>(this.apiUrl, request);
  }

  update(id: number, request: TRequest): Observable<TResponse> {
    return this.http.put<TResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  protected getAll(params?: HttpParams): Observable<PagedResponse<TResponse>> {
    return this.http.get<PagedResponse<TResponse>>(this.apiUrl, { params });
  }

  protected buildParams(filter: Record<string, unknown>): HttpParams {
    return Object.entries(filter)
      .filter(([, v]) => v !== undefined && v !== null && v !== '')
      .reduce((params, [k, v]) => params.set(k, String(v)), new HttpParams());
  }
}
