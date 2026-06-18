import { HttpParams } from '@angular/common/http';
import { BaseFilter } from '../models/pagination.model';

export function buildPaginationParams(filter: BaseFilter): HttpParams {
  let params = new HttpParams();
  if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
  if (filter.pageSize)   params = params.set('pageSize', filter.pageSize.toString());
  return params;
}

export function generatePageNumbers(totalPages: number, currentPage: number): (number | string)[] {
  if (totalPages <= 5) {
    return Array.from({ length: totalPages }, (_, i) => i + 1);
  }

  const pinnedPages = Array.from(new Set([1, 2, currentPage, totalPages])).sort((a, b) => a - b);
  const pageNumbers: (number | string)[] = [];

  for (let i = 0; i < pinnedPages.length; i++) {
    if (i > 0 && pinnedPages[i] - pinnedPages[i - 1] > 1) {
      pageNumbers.push('...');
    }
    pageNumbers.push(pinnedPages[i]);
  }

  return pageNumbers;
}

