import { Component, OnInit, OnDestroy } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { CompanyService } from 'src/app/_common/_services/company.service';
import { ToastService } from 'src/app/_shared/toast.service';
import { Notification } from 'src/app/_common/_models/pagination.model';

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
  hasServerError = false;
  cooldownSeconds = 0;
  private cooldownTimer: any = null;
  private errorStreak = 0;

  get isEdit(): boolean { return this.editId !== null; }
  get nameControl() { return this.form.get('name'); }
  get cnpjControl() { return this.form.get('cnpj'); }
  get foundationDateControl() { return this.form.get('foundationDate'); }
  get nameLength(): number { return this.nameControl?.value?.length ?? 0; }
  get cnpjValue(): string { return this.cnpjControl?.value ?? ''; }
  get cnpjDigits(): string { return this.cnpjValue.replace(/\D/g, ''); }

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
      cnpj: ['', [Validators.required, this.cnpjValidator]],
      foundationDate: ['', [Validators.required]]
    });

    this.nameControl?.valueChanges.subscribe(() => {
      this.hasServerError = false;
      this.errorMessage = '';
    });

    this.cnpjControl?.valueChanges.subscribe(() => {
      this.hasServerError = false;
      this.errorMessage = '';
    });

    this.foundationDateControl?.valueChanges.subscribe(() => {
      this.hasServerError = false;
      this.errorMessage = '';
    });

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
        this.form.patchValue({ name: item.name, cnpj: formattedCnpj, foundationDate: item.foundationDate });
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error loading company.');
        this.isLoading = false;
      }
    });
  }

  private cnpjValidator(control: AbstractControl): ValidationErrors | null {
    const digits = (control.value ?? '').replace(/\D/g, '');
    if (digits.length !== 14) return { cnpjLength: true };
    if (/^(\d)\1+$/.test(digits)) return { cnpjInvalid: true };

    const calc = (len: number, weights: number[]) => {
      let sum = 0;
      for (let i = 0; i < len; i++) sum += +digits[i] * weights[i];
      const rem = sum % 11;
      return rem < 2 ? 0 : 11 - rem;
    };

    const d13 = calc(12, [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);
    if (+digits[12] !== d13) return { cnpjInvalid: true };

    const d14 = calc(13, [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);
    if (+digits[13] !== d14) return { cnpjInvalid: true };

    return null;
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
    if (this.isSaving || this.cooldownSeconds > 0) return;
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.isSaving = true;
    this.errorMessage = '';
    const request = this.form.value;

    const operation = this.isEdit
      ? this.companyService.update(this.editId!, request)
      : this.companyService.create(request);

    operation.subscribe({
      next: () => {
        this.errorStreak = 0;
        this.toastService.success(this.isEdit ? 'Company updated successfully.' : 'Company created successfully.');
        this.router.navigate(['/companies']);
      },
      error: (err) => {
        this.handleServerErrors(err, this.isEdit ? 'Error updating company.' : 'Error creating company.');
        this.isSaving = false;
        this.startCooldown();
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/companies']);
  }

  private startCooldown(): void {
    this.errorStreak++;
    this.cooldownSeconds = Math.min(this.errorStreak * 5, 30);
    clearInterval(this.cooldownTimer);
    this.cooldownTimer = setInterval(() => {
      this.cooldownSeconds--;
      if (this.cooldownSeconds <= 0) {
        this.cooldownSeconds = 0;
        clearInterval(this.cooldownTimer);
      }
    }, 1000);
  }

  ngOnDestroy(): void {
    clearInterval(this.cooldownTimer);
  }

  handleServerErrors(err: HttpErrorResponse, fallback: string): void {
    this.errorMessage = Array.isArray(err.error)
      ? err.error.map((n: Notification) => n.message).join('\n')
      : fallback;
    this.hasServerError = true;
    this.nameControl?.markAsTouched();
  }
}
