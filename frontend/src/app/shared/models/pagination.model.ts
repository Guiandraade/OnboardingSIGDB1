export interface PagedResponse<T>{
  data: T[];
  total: number;
  pageNumber: number;
  pageSize: number;
}

export interface BaseFilter{
  pageNumber?: number;
  pageSize?: number;
}
