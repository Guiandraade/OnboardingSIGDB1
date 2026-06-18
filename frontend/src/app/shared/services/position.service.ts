import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PositionResponse, PositionFilter, PositionRequest } from '../models/position.model';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination.model';
import { ApiService } from '../../core/services/api.service';
import { buildPaginationParams } from '../utils/pagination.util';

@Injectable({ providedIn: 'root' })
export class PositionService {
  private readonly apiUrl = `${environment.apiUrl}/positions`;

  constructor(private api: ApiService) {}

  getAll(filter: PositionFilter): Observable<PagedResponse<PositionResponse>> {
    let params = buildPaginationParams(filter);
    if (filter.description) params = params.set('description', filter.description);
    return this.api.get<PagedResponse<PositionResponse>>(this.apiUrl, params);
  }

  getById(id: number): Observable<PositionResponse> {
    return this.api.get<PositionResponse>(`${this.apiUrl}/${id}`);
  }

  create(request: PositionRequest): Observable<PositionResponse> {
    return this.api.post<PositionResponse>(this.apiUrl, request);
  }

  update(id: number, request: PositionRequest): Observable<PositionResponse> {
    return this.api.put<PositionResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.api.delete(`${this.apiUrl}/${id}`);
  }
}
