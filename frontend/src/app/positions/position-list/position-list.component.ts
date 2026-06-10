import { HttpErrorResponse } from '@angular/common/http';
import { PositionFilter, PositionResponse } from '../../_common/_models/position.model';
import { Component, OnInit } from '@angular/core';
import { PositionService } from 'src/app/_common/_services/position.service';
import { Notification } from 'src/app/_common/_models/pagination.model';
import { ToastService } from 'src/app/_shared/toast.service';

@Component({
  selector: 'app-position-list',
  templateUrl: './position-list.component.html',
  styleUrls: ['./position-list.component.css']
})
export class PositionListComponent implements OnInit {

  items: PositionResponse[] = [];
  isLoading = false;
  errorMessage: string = '';
  pendingDeleteId: number | null = null;
  pendingDeleteName: string = '';
  filter: PositionFilter = { pageNumber: 1, pageSize: 5 };
  pageSizeOptions = [5, 10, 20, 50];
  totalItems = 0;
  totalPages = 0;

  get pageNumbers(): (number | string)[] {
    const total = this.totalPages;
    const current = this.filter.pageNumber!;
    if (total <= 5) return Array.from({ length: total }, (_, i) => i + 1);
    const pinned = Array.from(new Set([1, 2, current, total])).sort((a, b) => a - b);
    const result: (number | string)[] = [];
    for (let i = 0; i < pinned.length; i++) {
      if (i > 0 && pinned[i] - pinned[i - 1] > 1) result.push('...');
      result.push(pinned[i]);
    }
    return result;
  }

  constructor(
    private positionService: PositionService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.getItems();
  }

  getItems(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.positionService.getAll(this.filter).subscribe({
      next: (resp) => {
        this.items = resp.data;
        this.totalItems = resp.total;
        this.totalPages = Math.ceil(resp.total / (this.filter.pageSize || 5));
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error loading positions');
        this.isLoading = false;
      }
    });
  }

  confirmDelete(id: number, name: string): void {
    this.pendingDeleteId = id;
    this.pendingDeleteName = name;
  }

  cancelDelete(): void {
    this.pendingDeleteId = null;
    this.pendingDeleteName = '';
  }

  deleteItem(id: number): void {
    this.pendingDeleteId = null;
    this.pendingDeleteName = '';
    this.isLoading = true;
    this.positionService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Position deleted successfully.');
        this.getItems();
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error deleting position.');
        this.isLoading = false;
      }
    });
  }

  searchItems(): void {
    this.filter.pageNumber = 1;
    this.getItems();
  }

  goToPage(page: number): void {
    this.filter.pageNumber = page;
    this.getItems();
  }

  changePageSize(): void {
    this.filter.pageNumber = 1;
    this.getItems();
  }

  handleServerErrors(err: HttpErrorResponse, fallback: string): void {
    if (Array.isArray(err.error)) {
      this.errorMessage = err.error.map((n: Notification) => n.message).join(', ');
    } else {
      this.errorMessage = fallback;
    }
    this.toastService.error(this.errorMessage);
  }
}
