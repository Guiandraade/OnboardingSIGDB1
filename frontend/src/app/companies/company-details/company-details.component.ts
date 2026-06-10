import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { CompanyAndEmployeesResponse } from 'src/app/_common/_models/company.model';
import { Notification } from 'src/app/_common/_models/pagination.model';
import { CompanyService } from 'src/app/_common/_services/company.service';
import { EmployeeService } from 'src/app/_common/_services/employee.service';
import { ToastService } from 'src/app/_shared/toast.service';

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
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        const message = Array.isArray(err.error)
          ? err.error.map((n: Notification) => n.message).join(', ')
          : 'Error loading company details.';
        this.toastService.error(message);
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
        const message = Array.isArray(err.error)
          ? err.error.map((n: Notification) => n.message).join(', ')
          : 'Error removing employee.';
        this.toastService.error(message);
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
}
