import { Component } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseListComponent } from 'src/app/core/base/base-list.component';
import { PositionFilter, PositionResponse } from 'src/app/core/models/position.model';
import { PositionService } from 'src/app/core/services/position.service';

@Component({
  selector: 'app-position-list',
  templateUrl: './position-list.html',
  styleUrls: ['./position-list.css']
})
export class PositionList extends BaseListComponent<PositionResponse, PositionFilter> {

  filter: PositionFilter = { pageNumber: 1, pageSize: 10 };

  constructor(private positionService: PositionService) {
    super();
  }

  getItems(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.positionService.getPositions(this.filter).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.totalItems = res.total;
        this.totalPages = Math.ceil(res.total / this.filter.pageSize!);
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
    if (!confirm('Are you sure you want to delete this position?')) return;
    this.isLoading = true;
    this.positionService.delete(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => this.getItems(),
      error: (err) => {
        this.handleServerErrors(err, 'Error deleting position');
        this.isLoading = false;
      }
    });
  }

  get hasActiveFilter(): boolean {
    return !!this.filter.description?.trim();
  }

  clearSearchFilter(): void {
    this.filter.description = undefined;
  }
}
