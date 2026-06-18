import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { PositionService } from 'src/app/shared/services/position.service';
import { EmployeeService } from 'src/app/shared/services/employee.service';
import { CompanyService } from 'src/app/shared/services/company.service';
import { UserService } from 'src/app/core/services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  totalEmployees = 0;
  totalCompanies = 0;
  totalPositions = 0;
  loading = true;

  readonly today = new Date();

  constructor(
    private router: Router,
    private positionService: PositionService,
    private employeeService: EmployeeService,
    private companyService: CompanyService,
    readonly user: UserService
  ) {}

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  ngOnInit(): void {
    forkJoin({
      employees: this.employeeService.getAll({ pageNumber: 1, pageSize: 1 }),
      companies: this.companyService.getAll({ pageNumber: 1, pageSize: 1 }),
      positions: this.positionService.getAll({ pageNumber: 1, pageSize: 1 })
    }).subscribe({
      next: ({ employees, companies, positions }) => {
        this.totalEmployees = employees.total;
        this.totalCompanies = companies.total;
        this.totalPositions = positions.total;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
