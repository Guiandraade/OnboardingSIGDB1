import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { CompanyAndEmployeesResponse } from 'src/app/shared/models/company.model';
import { generatePageNumbers } from 'src/app/shared/utils/pagination.util';
import { CompanyService } from 'src/app/shared/services/company.service';
import { EmployeeService } from 'src/app/shared/services/employee.service';
import { ToastService } from 'src/app/shared/services/toast.service';

type EmployeeHistoryRow = CompanyAndEmployeesResponse['employeesPositionHistory'][number];

@Component({
  selector: 'app-company-details',
  templateUrl: './company-details.component.html',
  styleUrls: ['./company-details.component.css']
})
export class CompanyDetailsComponent implements OnInit {

  company: CompanyAndEmployeesResponse | null = null;
  isLoading = false;
  loadError = false;
  companyId = 0;

  employeesPage = 1;
  readonly employeesPageSize = 5;

  pendingDeleteEmployeeId: number | null = null;
  pendingDeleteEmployeeName = '';
  isDeletingEmployee = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private companyService: CompanyService,
    private employeeService: EmployeeService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.companyId = +id;
      this.loadCompany();
    } else {
      this.router.navigate(['/companies']);
    }
  }

  loadCompany(): void {
    this.isLoading = true;
    this.loadError = false;
    this.companyService.getEmployees(this.companyId).subscribe({
      next: (resp) => {
        this.company = resp;
        this.employeesPage = 1;
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.toastService.handleHttpError(err, 'Error loading company details.');
        this.loadError = true;
        this.isLoading = false;
      }
    });
  }

  confirmDeleteEmployee(id: number, name: string): void {
    this.pendingDeleteEmployeeId = id;
    this.pendingDeleteEmployeeName = name;
  }

  cancelDeleteEmployee(): void {
    this.pendingDeleteEmployeeId = null;
    this.pendingDeleteEmployeeName = '';
  }

  deleteEmployee(id: number): void {
    this.pendingDeleteEmployeeId = null;
    this.pendingDeleteEmployeeName = '';
    this.isDeletingEmployee = true;
    this.employeeService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Employee removed successfully.');
        this.isDeletingEmployee = false;
        this.loadCompany();
      },
      error: (err: HttpErrorResponse) => {
        this.toastService.handleHttpError(err, 'Error removing employee.');
        this.isDeletingEmployee = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/companies']);
  }

  get employeeCount(): number {
    return this.company?.employeesPositionHistory?.length ?? 0;
  }

  get pagedEmployees() {
    if (!this.company) return [];
    const start = (this.employeesPage - 1) * this.employeesPageSize;
    return this.company.employeesPositionHistory.slice(start, start + this.employeesPageSize);
  }

  get employeesTotalPages(): number {
    return Math.ceil(this.employeeCount / this.employeesPageSize);
  }

  get employeesPageNumbers(): (number | string)[] {
    return generatePageNumbers(this.employeesTotalPages, this.employeesPage);
  }

  goToEmployeesPage(page: number): void {
    this.employeesPage = page;
  }

  trackByEmployeeId(_index: number, row: EmployeeHistoryRow): number {
    return row.employeeId;
  }

  trackByPage(_index: number, page: number | string): number | string {
    return page;
  }

  trackByIndex(index: number): number {
    return index;
  }
}
