import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { EmployeeAndPositionsResponse } from 'src/app/shared/models/employee.model';
import { PositionResponse } from 'src/app/shared/models/position.model';
import { SearchSelectItem } from 'src/app/shared/components/search-select/search-select.component';
import { generatePageNumbers } from 'src/app/shared/utils/pagination.util';
import { EmployeeService } from 'src/app/shared/services/employee.service';
import { PositionService } from 'src/app/shared/services/position.service';
import { ToastService } from 'src/app/shared/services/toast.service';

type PositionHistoryRow = EmployeeAndPositionsResponse['positionHistory'][number];

@Component({
  selector: 'app-employee-details',
  templateUrl: './employee-details.component.html',
  styleUrls: ['./employee-details.component.css']
})
export class EmployeeDetailsComponent implements OnInit {

  employee: EmployeeAndPositionsResponse | null = null;
  isLoading = false;
  loadError = false;
  employeeId = 0;

  historyPage = 1;
  readonly historyPageSize = 5;
  private readonly DROPDOWN_PAGE_SIZE = 100;

  showChangePositionModal = false;
  positions: PositionResponse[] = [];
  positionsLoading = false;
  positionSelectControl = new FormControl(null as number | null);
  isChangingPosition = false;

  get positionItems(): SearchSelectItem[] {
    return this.positions.map(p => ({ id: p.id, displayName: p.description }));
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private employeeService: EmployeeService,
    private positionService: PositionService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.employeeId = +id;
      this.loadEmployee();
    } else {
      this.router.navigate(['/employees']);
    }
  }

  loadEmployee(): void {
    this.isLoading = true;
    this.loadError = false;
    this.employeeService.getPositionHistory(this.employeeId).subscribe({
      next: (resp) => {
        this.employee = resp;
        this.historyPage = 1;
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.toastService.handleHttpError(err, 'Error loading employee details.');
        this.loadError = true;
        this.isLoading = false;
      }
    });
  }

  openChangePositionModal(): void {
    this.showChangePositionModal = true;
    this.positionSelectControl.reset();
    if (this.positions.length === 0) {
      this.loadPositions();
    }
  }

  closeChangePositionModal(): void {
    this.showChangePositionModal = false;
    this.positionSelectControl.reset();
  }

  loadPositions(): void {
    this.positionsLoading = true;
    this.positionService.getAll({ pageNumber: 1, pageSize: this.DROPDOWN_PAGE_SIZE }).subscribe({
      next: (resp) => {
        this.positions = resp.data;
        this.positionsLoading = false;
      },
      error: () => { this.positionsLoading = false; }
    });
  }

  confirmChangePosition(): void {
    const positionId = this.positionSelectControl.value;
    if (!positionId || this.isChangingPosition) return;
    this.isChangingPosition = true;
    this.employeeService.changePosition(this.employeeId, positionId).subscribe({
      next: () => {
        this.toastService.success('Position changed successfully.');
        this.isChangingPosition = false;
        this.showChangePositionModal = false;
        this.positionSelectControl.reset();
        this.loadEmployee();
      },
      error: (err: HttpErrorResponse) => {
        this.toastService.handleHttpError(err, 'Error changing employee position.');
        this.isChangingPosition = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/employees']);
  }

  get positionCount(): number {
    return this.employee?.positionHistory?.length ?? 0;
  }

  get pagedHistory() {
    if (!this.employee) return [];
    const start = (this.historyPage - 1) * this.historyPageSize;
    return this.employee.positionHistory.slice(start, start + this.historyPageSize);
  }

  get historyTotalPages(): number {
    return Math.ceil(this.positionCount / this.historyPageSize);
  }

  get historyPageNumbers(): (number | string)[] {
    return generatePageNumbers(this.historyTotalPages, this.historyPage);
  }

  goToHistoryPage(page: number): void {
    this.historyPage = page;
  }

  trackByPositionId(_index: number, position: PositionResponse): number {
    return position.id;
  }

  trackByHistory(_index: number, row: PositionHistoryRow): string {
    return `${row.positionName}-${row.startDate}`;
  }

  trackByPage(_index: number, page: number | string): number | string {
    return page;
  }

  trackByIndex(index: number): number {
    return index;
  }
}
