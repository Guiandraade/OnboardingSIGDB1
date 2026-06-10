import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { EmployeeFilter, EmployeeResponse } from 'src/app/_common/_models/employee.model';
import { Notification } from 'src/app/_common/_models/pagination.model';
import { EmployeeService } from 'src/app/_common/_services/employee.service';
import { ToastService } from 'src/app/_shared/toast.service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit {

  items: EmployeeResponse[] = [];
  isLoading = false;
  totalItems = 0;
  totalPages = 0;
  filter: EmployeeFilter = { pageNumber: 1, pageSize: 5 };
  pageSizeOptions = [5, 10, 20, 50];
  pendingDeleteId: number | null = null;
  pendingDeleteName = '';

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
    private employeeService: EmployeeService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.getItems();
  }

  getItems(): void {
    this.isLoading = true;
    this.employeeService.getAll(this.filter).subscribe({
      next: (resp) => {
        this.items = resp.data;
        this.totalItems = resp.total;
        this.totalPages = Math.ceil(resp.total / (this.filter.pageSize || 5));
        this.isLoading = false;
      },
      error: (err) => { this.handleError(err, 'Error loading employees.'); this.isLoading = false; }
    });
  }

  searchItems(): void {
    this.filter.pageNumber = 1;
    this.getItems();
  }

  clearFilters(): void {
    this.filter = { pageNumber: 1, pageSize: this.filter.pageSize };
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
    this.employeeService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Employee deleted successfully.');
        this.getItems();
      },
      error: (err) => { this.handleError(err, 'Error deleting employee.'); this.isLoading = false; }
    });
  }

  handleError(err: HttpErrorResponse, fallback: string): void {
    const message = Array.isArray(err.error)
      ? err.error.map((n: Notification) => n.message).join(', ')
      : fallback;
    this.toastService.error(message);
  }
}
