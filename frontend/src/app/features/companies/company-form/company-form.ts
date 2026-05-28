import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { BaseFormComponent } from 'src/app/core/base/base-form.component';
import { CompanyRequest, CompanyResponse } from 'src/app/core/models/company.model';
import { CompanyService } from 'src/app/core/services/company.service';
import { ToastService } from 'src/app/core/services/toast.service';
import { CustomValidators } from 'src/app/core/validators/custom-validators';

@Component({
  selector: 'app-company-form',
  templateUrl: './company-form.html',
  styleUrls: ['./company-form.css']
})
export class CompanyForm extends BaseFormComponent<CompanyResponse> {

  form!: FormGroup;
  listRoute = '/companies';

  constructor(
    private fb: FormBuilder,
    private companyService: CompanyService,
    route: ActivatedRoute,
    router: Router,
    toastService: ToastService
  ) {
    super(route, router, toastService);
  }

  protected initForm(): void {
    this.form = this.fb.group({
      name:           ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150)]],
      cnpj:           ['', [Validators.required, CustomValidators.cnpj]],
      foundationDate: ['']
    });
  }

  protected getById(id: number): Observable<CompanyResponse> {
    return this.companyService.getById(id);
  }

  protected create(data: CompanyRequest): Observable<CompanyResponse> {
    return this.companyService.create(data);
  }

  protected update(id: number, data: CompanyRequest): Observable<CompanyResponse> {
    return this.companyService.update(id, data);
  }

  protected buildPayload(): CompanyRequest {
    const raw = this.form.value;
    return {
      ...raw,
      cnpj: (raw.cnpj ?? '').replace(/\D/g, ''),
      foundationDate: raw.foundationDate || undefined
    };
  }

  protected patchForm(entity: CompanyResponse): void {
    this.form.patchValue({
      ...entity,
      foundationDate: entity.foundationDate ? entity.foundationDate.substring(0, 10) : ''
    });
  }

  get nameLength(): number {
    return this.form.get('name')?.value?.length ?? 0;
  }

  get nameNearLimit(): boolean {
    return this.nameLength > 120;
  }

  get cnpjDigitCount(): number {
    return (this.form.get('cnpj')?.value ?? '').replace(/\D/g, '').length;
  }
}
