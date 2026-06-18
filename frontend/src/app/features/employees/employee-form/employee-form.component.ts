import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject, forkJoin, of } from 'rxjs';
import { catchError, finalize, takeUntil } from 'rxjs/operators';
import { EmployeeService } from 'src/app/shared/services/employee.service';
import { CompanyService } from 'src/app/shared/services/company.service';
import { PositionService } from 'src/app/shared/services/position.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { EmployeeResponse } from 'src/app/shared/models/employee.model';
import { CompanyResponse } from 'src/app/shared/models/company.model';
import { PositionResponse } from 'src/app/shared/models/position.model';
import { PagedResponse } from 'src/app/shared/models/pagination.model';
import { Notification as ApiNotification } from 'src/app/shared/models/error.model';
import { cpfValidator, buildMinMaxRules, noFutureDateValidator } from 'src/app/shared/utils/validators';
import { openDatePicker, formatDate } from 'src/app/shared/utils/date.util';
import { applyNotificationsToForm, extractNotifications } from '../../../shared/utils/server-error.util';
import { ValidationRule } from 'src/app/shared/components/validation-rules/validation-rules.component';
import { SearchSelectItem } from 'src/app/shared/components/search-select/search-select.component';

@Component({
  selector: 'app-employee-form',
  templateUrl: './employee-form.component.html',
  styleUrls: ['./employee-form.component.css']
})
export class EmployeeFormComponent implements OnInit, OnDestroy {

  form!: FormGroup;
  isLoading = true;
  isSaving = false;
  editId: number | null = null;
  editEmployee: EmployeeResponse | null = null;
  errorMessage = '';
  private destroy$ = new Subject<void>();
  readonly openDatePicker = openDatePicker;
  readonly formatDate = formatDate;
  private readonly DROPDOWN_PAGE_SIZE = 100;

  companies: CompanyResponse[] = [];
  positions: PositionResponse[] = [];

  get companyItems(): SearchSelectItem[] {
    return this.companies.map(c => ({ id: c.id, displayName: c.name }));
  }

  get positionItems(): SearchSelectItem[] {
    return this.positions.map(p => ({ id: p.id, displayName: p.description }));
  }

  get isEdit(): boolean { return this.editId !== null; }
  get nameControl() { return this.form?.get('name'); }
  get cpfControl() { return this.form?.get('cpf'); }
  get hireDateControl() { return this.form?.get('hireDate'); }
  get companyIdControl() { return this.form?.get('companyId'); }
  get positionIdControl() { return this.form?.get('positionId'); }
  get cpfValue(): string { return this.cpfControl?.value ?? ''; }
  get cpfDigits(): string { return this.cpfValue.replace(/\D/g, ''); }

  get nameRules() {
    return buildMinMaxRules(this.nameControl, 3, 100);
  }

  get cpfRules(): ValidationRule[] {
    return [
      {
        text: 'Must contain 11 digits',
        passed: this.cpfDigits.length === 11,
        failed: !!this.cpfControl?.touched && this.cpfDigits.length !== 11
      },
      {
        text: 'Valid CPF check digits',
        passed: this.cpfDigits.length === 11 && !this.cpfControl?.hasError('cpfInvalid'),
        failed: !!this.cpfControl?.touched && this.cpfDigits.length === 11 && !!this.cpfControl?.hasError('cpfInvalid')
      }
    ];
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private employeeService: EmployeeService,
    private companyService: CompanyService,
    private positionService: PositionService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.editId = +id;
      this.form = this.fb.group({
        name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        cpf:  ['', [Validators.required, cpfValidator]]
      });
      this.loadEmployee(this.editId);
    } else {
      this.form = this.fb.group({
        name:       ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        cpf:        ['', [Validators.required, cpfValidator]],
        hireDate:   ['', [Validators.required, noFutureDateValidator]],
        companyId:  [null, [Validators.required]],
        positionId: [null, [Validators.required]]
      });
      this.loadDropdowns();
    }

    ['name', 'cpf', 'hireDate', 'companyId', 'positionId'].forEach(n =>
      this.form.get(n)?.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => this.clearServerError(n))
    );
  }

  loadDropdowns(): void {
    const emptyCompanies: PagedResponse<CompanyResponse> = {
      data: [], total: 0, pageNumber: 1, pageSize: this.DROPDOWN_PAGE_SIZE
    };
    const emptyPositions: PagedResponse<PositionResponse> = {
      data: [], total: 0, pageNumber: 1, pageSize: this.DROPDOWN_PAGE_SIZE
    };

    forkJoin({
      companies: this.companyService.getAll({ pageNumber: 1, pageSize: this.DROPDOWN_PAGE_SIZE }).pipe(
        catchError((err: HttpErrorResponse) => { this.appendLoadError('companies', err); return of(emptyCompanies); })
      ),
      positions: this.positionService.getAll({ pageNumber: 1, pageSize: this.DROPDOWN_PAGE_SIZE }).pipe(
        catchError((err: HttpErrorResponse) => { this.appendLoadError('positions', err); return of(emptyPositions); })
      )
    }).subscribe(({ companies, positions }) => {
      this.companies = companies.data;
      this.positions = positions.data;
      this.isLoading = false;
    });
  }

  private appendLoadError(name: string, err: HttpErrorResponse): void {
    const detail = Array.isArray(err?.error)
      ? (err.error as ApiNotification[]).map(n => n.message).join('; ')
      : (err?.error?.title || err?.error?.message || err?.message || `HTTP ${err?.status}`);
    this.errorMessage = this.errorMessage
      ? `${this.errorMessage}\n${name}: ${detail}`
      : `${name}: ${detail}`;
  }

  loadEmployee(id: number): void {
    this.employeeService.getById(id).subscribe({
      next: (emp) => {
        this.editEmployee = emp;
        this.form.patchValue({ name: emp.name, cpf: this.formatCpfValue(emp.cpf) });
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error loading employee.');
        this.isLoading = false;
      }
    });
  }

  private formatCpfValue(value: string): string {
    const v = value.replace(/\D/g, '').substring(0, 11);
    if (v.length === 11) return v.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})$/, '$1.$2.$3-$4');
    return value;
  }

  formatCpf(event: Event): void {
    const input = event.target as HTMLInputElement;
    let v = input.value.replace(/\D/g, '').substring(0, 11);
    if (v.length > 9)       v = v.replace(/^(\d{3})(\d{3})(\d{3})(\d{0,2}).*/, '$1.$2.$3-$4');
    else if (v.length > 6)  v = v.replace(/^(\d{3})(\d{3})(\d{0,3}).*/, '$1.$2.$3');
    else if (v.length > 3)  v = v.replace(/^(\d{3})(\d{0,3}).*/, '$1.$2');
    this.cpfControl?.setValue(v, { emitEvent: false });
  }

  trackByIndex(index: number): number {
    return index;
  }

  save(): void {
    if (this.isSaving) return;
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const name = this.nameControl?.value;
    const cpf = this.cpfControl?.value;
    if (!name || !cpf) { this.form.markAllAsTouched(); return; }

    this.isSaving = true;
    this.errorMessage = '';

    const operation = this.isEdit
      ? this.employeeService.update(this.editId!, { name, cpf })
      : (() => {
          const hireDate = this.hireDateControl?.value;
          const companyId = this.companyIdControl?.value;
          const positionId = this.positionIdControl?.value;
          if (!hireDate || companyId == null || positionId == null) {
            this.form.markAllAsTouched();
            this.isSaving = false;
            return null;
          }
          return this.employeeService.create({
            name, cpf, hireDate,
            companyId: Number(companyId),
            positionId: Number(positionId)
          });
        })();

    if (!operation) return;

    operation.pipe(finalize(() => { this.isSaving = false; })).subscribe({
      next: () => {
        this.toastService.success(this.isEdit ? 'Employee updated successfully.' : 'Employee created successfully.');
        this.router.navigate(['/employees']);
      },
      error: (err) => {
        this.handleServerErrors(err, this.isEdit ? 'Error updating employee.' : 'Error creating employee.');
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  handleServerErrors(err: HttpErrorResponse, fallback: string): void {
    this.errorMessage = '';
    const notifications = extractNotifications(err.error);
    if (notifications.length === 0) { this.errorMessage = fallback; return; }

    const keyMap: { [k: string]: string } = {
      name: 'name', cpf: 'cpf', hiredate: 'hireDate',
      companyid: 'companyId', positionid: 'positionId'
    };
    const unmapped = applyNotificationsToForm(this.form, notifications, keyMap);
    if (unmapped.length) this.errorMessage = unmapped.join('\n');
  }

  private clearServerError(controlName: string): void {
    const ctrl = this.form.get(controlName);
    if (!ctrl?.hasError('serverError')) return;
    const rest = { ...(ctrl.errors ?? {}) };
    delete rest['serverError'];
    ctrl.setErrors(Object.keys(rest).length ? rest : null);
    if (!Object.values(this.form.controls).some(c => c.hasError('serverError'))) {
      this.errorMessage = '';
    }
  }
}
