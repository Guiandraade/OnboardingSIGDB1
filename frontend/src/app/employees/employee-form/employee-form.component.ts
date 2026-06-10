import { Component, OnInit, OnDestroy } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmployeeService } from 'src/app/_common/_services/employee.service';
import { CompanyService } from 'src/app/_common/_services/company.service';
import { PositionService } from 'src/app/_common/_services/position.service';
import { ToastService } from 'src/app/_shared/toast.service';
import { EmployeeResponse } from 'src/app/_common/_models/employee.model';
import { CompanyResponse } from 'src/app/_common/_models/company.model';
import { PositionResponse } from 'src/app/_common/_models/position.model';
import { Notification } from 'src/app/_common/_models/pagination.model';

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
  cooldownSeconds = 0;
  private cooldownTimer: any = null;
  private errorStreak = 0;

  companies: CompanyResponse[] = [];
  positions: PositionResponse[] = [];

  companySearch = '';
  positionSearch = '';
  showCompanyDropdown = false;
  showPositionDropdown = false;
  private selectedCompany: CompanyResponse | null = null;
  private selectedPosition: PositionResponse | null = null;

  get filteredCompanies(): CompanyResponse[] {
    const q = this.companySearch.toLowerCase();
    return q ? this.companies.filter(c => c.name.toLowerCase().includes(q)) : this.companies;
  }

  get filteredPositions(): PositionResponse[] {
    const q = this.positionSearch.toLowerCase();
    return q ? this.positions.filter(p => p.description.toLowerCase().includes(q)) : this.positions;
  }

  get isEdit(): boolean { return this.editId !== null; }
  get nameControl() { return this.form?.get('name'); }
  get cpfControl() { return this.form?.get('cpf'); }
  get nameLength(): number { return this.nameControl?.value?.length ?? 0; }
  get cpfValue(): string { return this.cpfControl?.value ?? ''; }
  get cpfDigits(): string { return this.cpfValue.replace(/\D/g, ''); }

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
        cpf:  ['', [Validators.required, this.cpfValidator]]
      });
      this.loadEmployee(this.editId);
    } else {
      this.form = this.fb.group({
        name:       ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        cpf:        ['', [Validators.required, this.cpfValidator]],
        hireDate:   ['', [Validators.required]],
        companyId:  [null, [Validators.required]],
        positionId: [null, [Validators.required]]
      });
      this.loadDropdowns();
    }

    ['name', 'cpf', 'hireDate', 'companyId', 'positionId'].forEach(key => {
      this.form.get(key)?.valueChanges.subscribe(() => this.clearServerError(key));
    });
  }

  loadDropdowns(): void {
    const empty = { data: [] as any[], total: 0, pageNumber: 1, pageSize: 100 };

    forkJoin({
      companies: this.companyService.getAll({ pageNumber: 1, pageSize: 100 }).pipe(
        catchError((err: HttpErrorResponse) => {
          this.appendLoadError('companies', err);
          return of(empty as any);
        })
      ),
      positions: this.positionService.getAll({ pageNumber: 1, pageSize: 100 }).pipe(
        catchError((err: HttpErrorResponse) => {
          this.appendLoadError('positions', err);
          return of(empty as any);
        })
      )
    }).subscribe(({ companies, positions }) => {
      this.companies = companies.data;
      this.positions = positions.data;
      this.isLoading = false;
    });
  }

  private appendLoadError(name: string, err: HttpErrorResponse): void {
    const detail = Array.isArray(err?.error)
      ? (err.error as Notification[]).map(n => n.message).join('; ')
      : (err?.error?.title || err?.error?.message || err?.message || `HTTP ${err?.status}`);
    this.errorMessage = this.errorMessage
      ? `${this.errorMessage}\n${name}: ${detail}`
      : `${name}: ${detail}`;
  }

  loadEmployee(id: number): void {
    this.employeeService.getById(id).subscribe({
      next: (emp) => {
        this.editEmployee = emp;
        this.form.patchValue({
          name: emp.name,
          cpf:  this.formatCpfValue(emp.cpf)
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error loading employee.');
        this.isLoading = false;
      }
    });
  }

  private cpfValidator(control: AbstractControl): ValidationErrors | null {
    const digits = (control.value ?? '').replace(/\D/g, '');
    if (digits.length !== 11) return { cpfLength: true };
    if (/^(\d)\1+$/.test(digits)) return { cpfInvalid: true };

    const calc = (len: number, weights: number[]) => {
      let sum = 0;
      for (let i = 0; i < len; i++) sum += +digits[i] * weights[i];
      const rem = sum % 11;
      return rem < 2 ? 0 : 11 - rem;
    };

    const d10 = calc(9,  [10, 9, 8, 7, 6, 5, 4, 3, 2]);
    if (+digits[9]  !== d10) return { cpfInvalid: true };

    const d11 = calc(10, [11, 10, 9, 8, 7, 6, 5, 4, 3, 2]);
    if (+digits[10] !== d11) return { cpfInvalid: true };

    return null;
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

  save(): void {
    if (this.isSaving || this.cooldownSeconds > 0) return;
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.isSaving = true;
    this.errorMessage = '';

    const operation = this.isEdit
      ? this.employeeService.update(this.editId!, {
          name: this.nameControl!.value,
          cpf:  this.cpfControl!.value
        })
      : this.employeeService.create({
          name:       this.nameControl!.value,
          cpf:        this.cpfControl!.value,
          hireDate:   this.form.get('hireDate')!.value,
          companyId:  +this.form.get('companyId')!.value,
          positionId: +this.form.get('positionId')!.value
        });

    operation.subscribe({
      next: () => {
        this.errorStreak = 0;
        this.toastService.success(this.isEdit ? 'Employee updated successfully.' : 'Employee created successfully.');
        this.router.navigate(['/employees']);
      },
      error: (err) => {
        this.handleServerErrors(err, this.isEdit ? 'Error updating employee.' : 'Error creating employee.');
        this.isSaving = false;
        this.startCooldown();
      }
    });
  }

  selectCompany(company: CompanyResponse): void {
    this.selectedCompany = company;
    this.form.get('companyId')!.setValue(company.id);
    this.companySearch = company.name;
    this.showCompanyDropdown = false;
  }

  onCompanyBlur(): void {
    this.form.get('companyId')?.markAsTouched();
    setTimeout(() => {
      this.showCompanyDropdown = false;
      this.companySearch = this.selectedCompany?.name ?? '';
    }, 150);
  }

  selectPosition(position: PositionResponse): void {
    this.selectedPosition = position;
    this.form.get('positionId')!.setValue(position.id);
    this.positionSearch = position.description;
    this.showPositionDropdown = false;
  }

  onPositionBlur(): void {
    this.form.get('positionId')?.markAsTouched();
    setTimeout(() => {
      this.showPositionDropdown = false;
      this.positionSearch = this.selectedPosition?.description ?? '';
    }, 150);
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }

  private startCooldown(): void {
    this.errorStreak++;
    this.cooldownSeconds = Math.min(this.errorStreak * 5, 30);
    clearInterval(this.cooldownTimer);
    this.cooldownTimer = setInterval(() => {
      this.cooldownSeconds--;
      if (this.cooldownSeconds <= 0) { this.cooldownSeconds = 0; clearInterval(this.cooldownTimer); }
    }, 1000);
  }

  ngOnDestroy(): void {
    clearInterval(this.cooldownTimer);
  }

  handleServerErrors(err: HttpErrorResponse, fallback: string): void {
    this.errorMessage = '';

    if (!Array.isArray(err.error) || err.error.length === 0) {
      this.errorMessage = fallback;
      return;
    }

    const keyMap: { [k: string]: string } = {
      name: 'name', cpf: 'cpf', hiredate: 'hireDate',
      companyid: 'companyId', positionid: 'positionId'
    };

    const unmapped: string[] = [];
    for (const n of err.error as Notification[]) {
      const controlName = keyMap[(n.key ?? '').toLowerCase()];
      const ctrl = controlName ? this.form.get(controlName) : null;
      if (ctrl) {
        ctrl.setErrors({ ...(ctrl.errors ?? {}), serverError: n.message });
        ctrl.markAsTouched();
      } else {
        unmapped.push(n.message);
      }
    }

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
