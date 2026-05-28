import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination.model';
import { PositionFilter, PositionRequest, PositionResponse } from '../models/position.model';
import { environment } from 'src/environments/environment';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class PositionService extends BaseApiService<PositionResponse, PositionRequest> {

  protected apiUrl = `${environment.apiUrl}/positions`;

  constructor(http: HttpClient) { super(http); }

  getPositions(filter?: PositionFilter): Observable<PagedResponse<PositionResponse>> {
    return this.getAll(filter ? this.buildParams(filter as Record<string, unknown>) : undefined);
  }
}
