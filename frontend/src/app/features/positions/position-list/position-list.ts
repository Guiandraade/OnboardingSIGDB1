import { Component, OnInit } from '@angular/core';
import { PositionFilter, PositionResponse } from 'src/app/core/models/position.model';
import { PositionService } from 'src/app/core/services/position.service';
import { Notification } from 'src/app/core/models/pagination.model';

@Component({
  selector: 'app-position-list',
  templateUrl: './position-list.html',
  styleUrls: ['./position-list.css']
})
export class PositionList implements OnInit {

  positions: PositionResponse[] = [];
  isLoading = false;
  errorMessage = '';
  filter: PositionFilter = {
    pageNumber: 1,
    pageSize: 10,
  };
  totalItems = 0;
  totalPages = 0;

  constructor(private positionService: PositionService) { }

  ngOnInit(): void {
    this.getPositions();
  }

  getPositions(): void {
    this.isLoading = true;
    this.positionService.getPositions(this.filter).subscribe({
      next: (response) => {
        this.totalItems = response.total;
        this.totalPages = Math.ceil(response.total / this.filter.pageSize!);
        this.positions = response.data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching positions:', err);
        if (Array.isArray(err.error)) {
          this.errorMessage = err.error.map((n: Notification) => n.message).join(', ');
        } else {
          this.errorMessage = 'Error fetching positions';
        }
        this.isLoading = false;
      }
  });
  }

  deletePosition(id: number): void {
    if (confirm('Are you sure you want to delete this position?')) {
      this.isLoading = true;
      this.positionService.deletePosition(id).subscribe({
        next: () => {
          this.getPositions();
        },
        error: (err) => {
          console.error('Error deleting position:', err);
          if (Array.isArray(err.error)) {
            this.errorMessage = err.error.map((n: Notification) => n.message).join(', ');
          } else {
            this.errorMessage = 'Error deleting position';
          }
          this.isLoading = false;
        }
      });
    }
  }

  get currentPage(): number {
  return this.filter.pageNumber ?? 1;
}

  search(): void {
    this.filter.pageNumber = 1;
    this.getPositions();
  }

  changePage(page: number): void {
    this.filter.pageNumber = page;
    this.getPositions();
  }
}
