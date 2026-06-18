import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { CompanyFilter, CompanyResponse } from 'src/app/shared/models/company.model';
import { CompanyService } from 'src/app/shared/services/company.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { openDatePicker, formatDate } from 'src/app/shared/utils/date.util';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.css']
})
export class CompanyListComponent implements OnInit {
  items: CompanyResponse[] = [];
  isLoading = false;
  totalItems = 0;
  totalPages = 0;
  filter: CompanyFilter = { pageNumber: 1, pageSize: 5 };
  pendingDeleteId: number | null = null;
  pendingDeleteName = '';
  readonly openDatePicker = openDatePicker;
  readonly formatDate = formatDate;

  constructor(
    private companyService: CompanyService,
    private toastService: ToastService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.pipe(take(1)).subscribe(params => {
      if (params['name'])         this.filter.name = params['name'];
      if (params['cnpj'])         this.filter.cnpj = params['cnpj'];
      if (params['foundedIn'])    this.filter.foundedIn = params['foundedIn'];
      if (params['foundedUntil']) this.filter.foundedUntil = params['foundedUntil'];
      if (params['page'])         this.filter.pageNumber = +params['page'];
      if (params['size'])         this.filter.pageSize = +params['size'];
    });
    this.getItems();
  }

  private syncQueryParams(): void {
    const params: Record<string, string> = {};
    if (this.filter.name)         params['name'] = this.filter.name;
    if (this.filter.cnpj)         params['cnpj'] = this.filter.cnpj;
    if (this.filter.foundedIn)    params['foundedIn'] = this.filter.foundedIn;
    if (this.filter.foundedUntil) params['foundedUntil'] = this.filter.foundedUntil;
    if ((this.filter.pageNumber ?? 1) > 1)   params['page'] = String(this.filter.pageNumber);
    if ((this.filter.pageSize ?? 5) !== 5)   params['size'] = String(this.filter.pageSize);
    this.router.navigate([], { relativeTo: this.route, queryParams: params, replaceUrl: true });
  }

  getItems(): void {
    this.syncQueryParams();
    this.isLoading = true;
    this.companyService.getAll(this.filter).subscribe({
      next: (resp) => {
        this.items = resp.data;
        this.totalItems = resp.total;
        this.totalPages = Math.ceil(resp.total / (this.filter.pageSize || 5));
        this.isLoading = false;
      },
      error: (err) => { this.toastService.handleHttpError(err, 'Error loading companies.'); this.isLoading = false; }
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

  hasActiveFilters(): boolean {
    return !!(this.filter.name || this.filter.cnpj || this.filter.foundedIn || this.filter.foundedUntil);
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
    this.companyService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Company deleted successfully.');
        this.getItems();
      },
      error: (err) => { this.toastService.handleHttpError(err); this.isLoading = false; }
    });
  }

  trackById(_index: number, item: CompanyResponse): number {
    return item.id;
  }

  trackByIndex(index: number): number {
    return index;
  }
}
