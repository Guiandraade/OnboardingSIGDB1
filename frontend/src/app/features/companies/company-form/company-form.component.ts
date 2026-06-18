import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { CompanyService } from 'src/app/shared/services/company.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { CompanyRequest } from 'src/app/shared/models/company.model';
import { cnpjValidator, buildMinMaxRules, noFutureDateValidator } from 'src/app/shared/utils/validators';
import { openDatePicker, formatDate } from 'src/app/shared/utils/date.util';
import { extractNotifications, applyNotificationsToForm } from '../../../shared/utils/server-error.util';
import { ValidationRule } from 'src/app/shared/components/validation-rules/validation-rules.component';

@Component({
  selector: 'app-company-form',
  templateUrl: './company-form.component.html',
  styleUrls: ['./company-form.component.css']
})
export class CompanyFormComponent implements OnInit, OnDestroy {

  form!: FormGroup;
  isLoading = false;
  isSaving = false;
  editId: number | null = null;
  errorMessage = '';
  private destroy$ = new Subject<void>();
  readonly openDatePicker = openDatePicker;
  readonly formatDate = formatDate;

  get isEdit(): boolean { return this.editId !== null; }
  get nameControl() { return this.form.get('name'); }
  get cnpjControl() { return this.form.get('cnpj'); }
  get foundationDateControl() { return this.form.get('foundationDate'); }
  get cnpjValue(): string { return this.cnpjControl?.value ?? ''; }
  get cnpjDigits(): string { return this.cnpjValue.replace(/\D/g, ''); }

  get nameRules() {
    return buildMinMaxRules(this.nameControl, 3, 100);
  }

  get cnpjRules(): ValidationRule[] {
    return [
      {
        text: 'Must contain 14 digits',
        passed: this.cnpjDigits.length === 14,
        failed: !!this.cnpjControl?.touched && this.cnpjDigits.length !== 14
      },
      {
        text: 'Valid CNPJ check digits',
        passed: this.cnpjDigits.length === 14 && !this.cnpjControl?.hasError('cnpjInvalid'),
        failed: !!this.cnpjControl?.touched && this.cnpjDigits.length === 14 && !!this.cnpjControl?.hasError('cnpjInvalid')
      }
    ];
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private companyService: CompanyService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      cnpj: ['', [Validators.required, cnpjValidator]],
      foundationDate: ['', [noFutureDateValidator]]
    });

    ['name', 'cnpj', 'foundationDate'].forEach(n =>
      this.form.get(n)?.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
        this.errorMessage = '';
        this.clearServerError(n);
      })
    );

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.editId = +id;
      this.loadItem(this.editId);
    }
  }

  loadItem(id: number): void {
    this.isLoading = true;
    this.companyService.getById(id).subscribe({
      next: (item) => {
        const formattedCnpj = this.formatCnpjValue(item.cnpj);
        const dateOnly = item.foundationDate ? item.foundationDate.substring(0, 10) : '';
        this.form.patchValue({ name: item.name, cnpj: formattedCnpj, foundationDate: dateOnly });
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error loading company.');
        this.isLoading = false;
      }
    });
  }

  private formatCnpjValue(value: string): string {
    const v = value.replace(/\D/g, '').substring(0, 14);
    if (v.length === 14) return v.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
    return value;
  }

  formatCnpj(event: Event): void {
    const input = event.target as HTMLInputElement;
    let v = input.value.replace(/\D/g, '').substring(0, 14);
    if (v.length > 12)      v = v.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{0,2}).*/, '$1.$2.$3/$4-$5');
    else if (v.length > 8)  v = v.replace(/^(\d{2})(\d{3})(\d{3})(\d{0,4}).*/, '$1.$2.$3/$4');
    else if (v.length > 5)  v = v.replace(/^(\d{2})(\d{3})(\d{0,3}).*/, '$1.$2.$3');
    else if (v.length > 2)  v = v.replace(/^(\d{2})(\d{0,3}).*/, '$1.$2');
    this.cnpjControl?.setValue(v, { emitEvent: false });
  }

  save(): void {
    if (this.isSaving) return;
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.isSaving = true;
    this.errorMessage = '';
    const request: CompanyRequest = {
      name: this.nameControl?.value,
      cnpj: this.cnpjControl?.value,
      foundationDate: this.toDateOnly(this.foundationDateControl?.value)
    };

    const operation = this.isEdit
      ? this.companyService.update(this.editId!, request)
      : this.companyService.create(request);

    operation.pipe(
      finalize(() => {
        this.isSaving = false;
      })
    ).subscribe({
      next: () => {
        this.toastService.success(this.isEdit ? 'Company updated successfully.' : 'Company created successfully.');
        this.router.navigate(['/companies']);
      },
      error: (err) => {
        this.handleServerErrors(err, this.isEdit ? 'Error updating company.' : 'Error creating company.');
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/companies']);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  handleServerErrors(err: HttpErrorResponse, fallback: string): void {
    this.errorMessage = '';
    const notifications = extractNotifications(err.error);
    if (notifications.length === 0) { this.errorMessage = fallback; return; }
    const keyMap: { [k: string]: string } = { name: 'name', cnpj: 'cnpj', foundationdate: 'foundationDate' };
    const unmapped = applyNotificationsToForm(this.form, notifications, keyMap);
    if (unmapped.length) this.errorMessage = unmapped.join('\n');
  }

  private toDateOnly(value: string | null | undefined): string | null {
    if (!value) return null;
    if (/^\d{2}\/\d{2}\/\d{4}$/.test(value)) {
      const [d, m, y] = value.split('/');
      return `${y}-${m}-${d}`;
    }
    return value.substring(0, 10);
  }

  private clearServerError(controlName: string): void {
    const ctrl = this.form.get(controlName);
    if (!ctrl?.hasError('serverError')) return;
    const rest = { ...(ctrl.errors ?? {}) };
    delete rest['serverError'];
    ctrl.setErrors(Object.keys(rest).length ? rest : null);
  }
}
