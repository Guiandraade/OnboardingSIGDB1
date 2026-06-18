import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { PositionRequest } from 'src/app/shared/models/position.model';
import { PositionService } from 'src/app/shared/services/position.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { buildMinMaxRules } from 'src/app/shared/utils/validators';
import { extractNotifications, applyNotificationsToForm, buildFormErrorMessage } from 'src/app/shared/utils/server-error.util';

@Component({
  selector: 'app-position-form',
  templateUrl: './position-form.component.html',
  styleUrls: ['./position-form.component.css']
})
export class PositionFormComponent implements OnInit, OnDestroy {

  form!: FormGroup;
  isLoading = false;
  isSaving = false;
  editId: number | null = null;
  errorMessage = '';
  private destroy$ = new Subject<void>();

  get isEdit(): boolean {
    return this.editId !== null;
  }

  get descriptionControl() {
    return this.form.get('description');
  }

  get descriptionRules() {
    return buildMinMaxRules(this.descriptionControl, 3, 100);
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private positionService: PositionService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]]
    });

    this.form.get('description')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.errorMessage = '';
        const ctrl = this.descriptionControl;
        if (ctrl?.hasError('serverError')) {
          const { serverError, ...rest } = ctrl.errors ?? {};
          ctrl.setErrors(Object.keys(rest).length ? rest : null);
        }
      });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.editId = +id;
      this.loadItem(this.editId);
    }
  }

  loadItem(id: number): void {
    this.isLoading = true;
    this.positionService.getById(id).subscribe({
      next: (item) => {
        this.form.patchValue({ description: item.description });
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err);
        this.isLoading = false;
      }
    });
  }

  save(): void {
    if (this.isSaving) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    const request: PositionRequest = {
      description: this.descriptionControl?.value
    };

    const operation = this.isEdit
      ? this.positionService.update(this.editId!, request)
      : this.positionService.create(request);

    operation.pipe(
      finalize(() => {
        this.isSaving = false;
      })
    ).subscribe({
      next: () => {
        this.toastService.success(
          this.isEdit ? 'Position updated successfully.' : 'Position created successfully.'
        );
        this.router.navigate(['/positions']);
      },
      error: (err) => {
        this.handleServerErrors(err);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/positions']);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  handleServerErrors(err: HttpErrorResponse): void {
    this.errorMessage = '';
    const notifications = extractNotifications(err.error);
    if (notifications.length === 0) {
      this.errorMessage = buildFormErrorMessage(err);
      this.form.get('description')?.markAsTouched();
      return;
    }
    const keyMap: { [k: string]: string } = { description: 'description' };
    const unmapped = applyNotificationsToForm(this.form, notifications, keyMap);
    if (unmapped.length) this.errorMessage = unmapped.join('\n');
  }
}
