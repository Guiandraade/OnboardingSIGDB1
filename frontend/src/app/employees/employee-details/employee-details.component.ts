import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { EmployeeAndPositionsResponse } from 'src/app/_common/_models/employee.model';
import { PositionResponse } from 'src/app/_common/_models/position.model';
import { Notification } from 'src/app/_common/_models/pagination.model';
import { EmployeeService } from 'src/app/_common/_services/employee.service';
import { PositionService } from 'src/app/_common/_services/position.service';
import { ToastService } from 'src/app/_shared/toast.service';

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

  showChangePositionModal = false;
  positions: PositionResponse[] = [];
  positionsLoading = false;
  selectedPositionId: number | null = null;
  isChangingPosition = false;

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
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        const message = Array.isArray(err.error)
          ? err.error.map((n: Notification) => n.message).join(', ')
          : 'Error loading employee details.';
        this.toastService.error(message);
        this.loadError = true;
        this.isLoading = false;
      }
    });
  }

  openChangePositionModal(): void {
    this.showChangePositionModal = true;
    this.selectedPositionId = null;
    if (this.positions.length === 0) {
      this.loadPositions();
    }
  }

  closeChangePositionModal(): void {
    this.showChangePositionModal = false;
    this.selectedPositionId = null;
  }

  loadPositions(): void {
    this.positionsLoading = true;
    this.positionService.getAll({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (resp) => {
        this.positions = resp.data;
        this.positionsLoading = false;
      },
      error: () => { this.positionsLoading = false; }
    });
  }

  confirmChangePosition(): void {
    if (!this.selectedPositionId || this.isChangingPosition) return;
    this.isChangingPosition = true;
    this.employeeService.changePosition(this.employeeId, this.selectedPositionId).subscribe({
      next: () => {
        this.toastService.success('Position changed successfully.');
        this.isChangingPosition = false;
        this.showChangePositionModal = false;
        this.selectedPositionId = null;
        this.loadEmployee();
      },
      error: (err: HttpErrorResponse) => {
        const message = Array.isArray(err.error)
          ? err.error.map((n: Notification) => n.message).join(', ')
          : 'Error changing position.';
        this.toastService.error(message);
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
}
