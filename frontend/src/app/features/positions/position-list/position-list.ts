import { Component } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseListComponent } from 'src/app/core/base/base-list.component';
import { PositionFilter, PositionResponse } from 'src/app/core/models/position.model';
import { PositionService } from 'src/app/core/services/position.service';
import { ToastService } from 'src/app/core/services/toast.service';

@Component({
  selector: 'app-position-list',
  templateUrl: './position-list.html',
  styleUrls: ['./position-list.css']
})
export class PositionList extends BaseListComponent<PositionResponse, PositionFilter> {

  filter: PositionFilter = { pageNumber: 1, pageSize: 10 };
  pendingDeleteId: number | null = null;

  constructor(private positionService: PositionService, toastService: ToastService) {
    super(toastService);
  }

  getItems(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.positionService.getPositions(this.filter).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        const pageSize = this.filter.pageSize || 10;
        this.totalItems = res.total;
        this.totalPages = Math.ceil(res.total / pageSize);
        this.items = res.data;
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error fetching positions');
        this.isLoading = false;
      }
    });
  }

  deleteItem(id: number): void {
    this.pendingDeleteId = id;
  }

  confirmDelete(): void {
    if (this.pendingDeleteId === null) {
      return;
    }

    const id = this.pendingDeleteId;
    this.pendingDeleteId = null;
    this.isLoading = true;
    this.positionService.delete(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.toastService.show('Record deleted successfully!');
        this.getItems();
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error deleting position');
        this.isLoading = false;
      }
    });
  }

  cancelDelete(): void {
    this.pendingDeleteId = null;
  }

  get hasActiveFilter(): boolean {
    return !!this.filter.description?.trim();
  }

  clearSearchFilter(): void {
    this.filter.description = undefined;
  }
}
