import { Directive, OnDestroy, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject } from 'rxjs';
import { BaseFilter, Notification } from '../models/pagination.model';
import { ToastService } from '../services/toast.service';

@Directive()
export abstract class BaseListComponent<T, F extends BaseFilter> implements OnInit, OnDestroy {

  items: T[] = [];
  isLoading = false;
  errorMessage = '';
  totalItems = 0;
  totalPages = 0;
  pageSizeOptions = [5, 10, 25, 50];

  abstract filter: F;

  protected destroy$ = new Subject<void>();

  constructor(protected toastService: ToastService) {}

  ngOnInit(): void {
    this.getItems();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  abstract getItems(): void;
  abstract deleteItem(id: number): void;
  abstract get hasActiveFilter(): boolean;
  abstract clearSearchFilter(): void;

  search(): void {
    this.filter.pageNumber = 1;
    this.getItems();
  }

  clearSearch(): void {
    this.clearSearchFilter();
    this.filter.pageNumber = 1;
    this.getItems();
  }

  changePageSize(): void {
    this.filter.pageNumber = 1;
    this.getItems();
  }

  changePage(page: number): void {
    this.filter.pageNumber = page;
    this.getItems();
  }

  protected handleServerErrors(err: HttpErrorResponse, fallback: string): void {
    if (Array.isArray(err.error)) {
      this.errorMessage = err.error.map((n: Notification) => n.message).join(', ');
    } else {
      this.errorMessage = fallback;
    }
  }

  get currentPage(): number {
    return this.filter.pageNumber ?? 1;
  }

  get pageNumbers(): (number | string)[] {
    const total = this.totalPages;
    const current = this.currentPage;

    if (total <= 7) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    const pages: (number | string)[] = [1];

    if (current > 3) {
      pages.push('...');
    }

    const start = Math.max(2, current - 1);
    const end   = Math.min(total - 1, current + 1);

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    if (current < total - 2) {
      pages.push('...');
    }

    pages.push(total);
    return pages;
  }
}
