import { Component } from '@angular/core';
import { takeUntil } from 'rxjs/operators';
import { BaseListComponent } from 'src/app/core/base/base-list.component';
import { CompanyFilter, CompanyResponse } from 'src/app/core/models/company.model';
import { CompanyService } from 'src/app/core/services/company.service';
import { ToastService } from 'src/app/core/services/toast.service';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.html',
  styleUrls: ['./company-list.css']
})
export class CompanyList extends BaseListComponent<CompanyResponse, CompanyFilter> {

  filter: CompanyFilter = { pageNumber: 1, pageSize: 10 };
  pendingDeleteId: number | null = null;

  constructor(private companyService: CompanyService, toastService: ToastService) {
    super(toastService);
  }

  getItems(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.companyService.getCompanies(this.filter).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        const pageSize = this.filter.pageSize || 10;
        this.totalItems = res.total;
        this.totalPages = Math.ceil(res.total / pageSize);
        this.items = res.data;
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error fetching companies');
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
    this.companyService.delete(id).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.toastService.show('Record deleted successfully!');
        this.getItems();
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error deleting company');
        this.isLoading = false;
      }
    });
  }

  cancelDelete(): void {
    this.pendingDeleteId = null;
  }

  get hasActiveFilter(): boolean {
    return !!this.filter.name?.trim();
  }

  clearSearchFilter(): void {
    this.filter.name = undefined;
  }
}
