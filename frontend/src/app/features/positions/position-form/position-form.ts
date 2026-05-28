import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { BaseFormComponent } from 'src/app/core/base/base-form.component';
import { PositionRequest, PositionResponse } from 'src/app/core/models/position.model';
import { PositionService } from 'src/app/core/services/position.service';
import { ToastService } from 'src/app/core/services/toast.service';

@Component({
  selector: 'app-position-form',
  templateUrl: './position-form.html',
  styleUrls: ['./position-form.css']
})
export class PositionForm extends BaseFormComponent<PositionResponse> {

  form!: FormGroup;
  listRoute = '/positions';

  constructor(
    private fb: FormBuilder,
    private positionService: PositionService,
    route: ActivatedRoute,
    router: Router,
    toastService: ToastService
  ) {
    super(route, router, toastService);
  }

  protected initForm(): void {
    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]]
    });
  }

  protected getById(id: number): Observable<PositionResponse> {
    return this.positionService.getById(id);
  }

  protected create(data: PositionRequest): Observable<PositionResponse> {
    return this.positionService.create(data);
  }

  protected update(id: number, data: PositionRequest): Observable<PositionResponse> {
    return this.positionService.update(id, data);
  }

  protected buildPayload(): PositionRequest {
    return this.form.value;
  }

  protected patchForm(entity: PositionResponse): void {
    this.form.patchValue(entity);
  }

  get descriptionLength(): number {
    return this.form.get('description')?.value?.length ?? 0;
  }

  get nearLimit(): boolean {
    return this.descriptionLength > 80;
  }
}
