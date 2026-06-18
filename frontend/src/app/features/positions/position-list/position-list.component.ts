import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { PositionFilter, PositionResponse } from '../../../shared/models/position.model';
import { PositionService } from 'src/app/shared/services/position.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-position-list',
  templateUrl: './position-list.component.html',
  styleUrls: ['./position-list.component.css']
})
export class PositionListComponent implements OnInit {
  items: PositionResponse[] = [];
  isLoading = false;
  pendingDeleteId: number | null = null;
  pendingDeleteName: string = '';
  filter: PositionFilter = { pageNumber: 1, pageSize: 5 };
  totalItems = 0;
  totalPages = 0;

  constructor(
    private positionService: PositionService,
    private toastService: ToastService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.pipe(take(1)).subscribe(params => {
      if (params['description']) this.filter.description = params['description'];
      if (params['page'])        this.filter.pageNumber = +params['page'];
      if (params['size'])        this.filter.pageSize = +params['size'];
    });
    this.getItems();
  }

  private syncQueryParams(): void {
    const params: Record<string, string> = {};
    if (this.filter.description) params['description'] = this.filter.description;
    if ((this.filter.pageNumber ?? 1) > 1)  params['page'] = String(this.filter.pageNumber);
    if ((this.filter.pageSize ?? 5) !== 5)  params['size'] = String(this.filter.pageSize);
    this.router.navigate([], { relativeTo: this.route, queryParams: params, replaceUrl: true });
  }

  getItems(): void {
    this.syncQueryParams();
    this.isLoading = true;
    this.positionService.getAll(this.filter).subscribe({
      next: (resp) => {
        this.items = resp.data;
        this.totalItems = resp.total;
        this.totalPages = Math.ceil(resp.total / (this.filter.pageSize || 5));
        this.isLoading = false;
      },
      error: (err) => {
        this.toastService.handleHttpError(err, 'Error loading positions.');
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
        this.toastService.handleHttpError(err, 'Error deleting position.');
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

  changePageSize(pageSize: number): void {
    this.filter.pageSize = pageSize;
    this.filter.pageNumber = 1;
    this.getItems();
  }

  clearFilters(): void {
    this.filter = { pageNumber: 1, pageSize: this.filter.pageSize };
    this.getItems();
  }

  hasActiveFilters(): boolean {
    return !!(this.filter.description);
  }

  trackById(_index: number, item: PositionResponse): number {
    return item.id;
  }

  trackByIndex(index: number): number {
    return index;
  }
}
