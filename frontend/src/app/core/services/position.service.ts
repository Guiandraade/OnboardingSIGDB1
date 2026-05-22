import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination.model';
import { PositionResponse, PositionFilter, PositionRequest } from '../models/position.model';

@Injectable({
  providedIn: 'root'
})
export class PositionService {

  private apiUrl = 'http://localhost:5099/positions';

  constructor(private http: HttpClient) { }

  getPositions(filter?: PositionFilter) : Observable<PagedResponse<PositionResponse>> {

      let params = new HttpParams();

      if (filter) {
        if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
        if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
        if (filter.description) params = params.set('description', filter.description);
      }
    return this.http.get<PagedResponse<PositionResponse>>(this.apiUrl, { params });
  }

  getPositionById(id: number): Observable<PositionResponse> {
    return this.http.get<PositionResponse>(`${this.apiUrl}/${id}`);
  }

  createPosition(request: PositionRequest): Observable<PositionResponse> {
    return this.http.post<PositionResponse>(this.apiUrl, request);
  }

  updatePosition(id: number, request: PositionRequest): Observable<PositionResponse> {
    return this.http.put<PositionResponse>(`${this.apiUrl}/${id}`, request);
  }

  deletePosition(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
