import { Injectable  } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams  } from '@angular/common/http';
import { PositionResponse, PositionFilter, PositionRequest } from '../_models/position.model';
import { Observable } from 'rxjs';
import { PagedResponse } from '../_models/pagination.model';

@Injectable({ providedIn: 'root'})
export class PositionService{
  private readonly apiUrl = `${environment.apiUrl}/positions`;

  constructor(private http: HttpClient){}

  getAll(filter: PositionFilter) : Observable<PagedResponse<PositionResponse>>{

    let params = new HttpParams();

    if(filter){
      if(filter.description) params = params.set('description', filter.description);
      if(filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if(filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
    }

    return this.http.get<PagedResponse<PositionResponse>>(this.apiUrl, { params })
  }

  getById(id: number) : Observable<PositionResponse>{
    return this.http.get<PositionResponse>(`${this.apiUrl}/${id}`);
  }

  create(request: PositionRequest) : Observable<PositionResponse>{
    return this.http.post<PositionResponse>(this.apiUrl, request);
  }

  update(id: number, request: PositionRequest) : Observable<PositionResponse>{
    return this.http.put<PositionResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number) : Observable<void>{
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
