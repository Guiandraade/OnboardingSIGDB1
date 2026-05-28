import { Directive, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Notification } from '../models/pagination.model';
import { ToastService } from '../services/toast.service';

@Directive()
export abstract class BaseFormComponent<TResponse> implements OnInit, OnDestroy {

  abstract form: FormGroup;

  isEditMode = false;
  isLoading = false;
  errorMessage = '';

  protected entityId: number | null = null;
  protected destroy$ = new Subject<void>();

  protected abstract listRoute: string;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.setupServerErrorClearing();
    this.verifyEditMode();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected abstract initForm(): void;
  protected abstract getById(id: number): Observable<TResponse>;
  protected abstract create(data: any): Observable<TResponse>;
  protected abstract update(id: number, data: any): Observable<TResponse>;
  protected abstract buildPayload(): any;
  protected abstract patchForm(entity: TResponse): void;

  private setupServerErrorClearing(): void {
    Object.keys(this.form.controls).forEach(key => {
      this.form.get(key)?.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
        const control = this.form.get(key);
        if (control?.errors?.['serverError']) {
          const errors = control.errors;
          if (!errors) {
            return;
          }

          const { serverError, ...rest } = errors;
          control.setErrors(Object.keys(rest).length ? rest : null);
        }
      });
    });
  }

  private verifyEditMode(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    this.isEditMode = true;
    this.entityId = Number(idParam);
    this.isLoading = true;

    this.getById(this.entityId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (entity) => {
        this.patchForm(entity);
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.handleServerErrors(err, 'Error fetching record');
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const payload = this.buildPayload();
    const request$: Observable<TResponse> = this.isEditMode && this.entityId !== null
      ? this.update(this.entityId, payload)
      : this.create(payload);

    request$.pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.isLoading = false;
        this.toastService.show(this.isEditMode ? 'Record updated successfully!' : 'Record created successfully!');
        this.router.navigate([this.listRoute]);
      },
      error: (err: HttpErrorResponse) => {
        this.handleServerErrors(err, 'Error saving record');
        this.isLoading = false;
      }
    });
  }

  protected handleServerErrors(err: HttpErrorResponse, fallback: string): void {
    this.errorMessage = '';

    if (Array.isArray(err.error)) {
      err.error.forEach((n: Notification) => {
        const camelKey = n.key.charAt(0).toLowerCase() + n.key.slice(1);
        const control = this.form.get(camelKey);
        if (control) {
          control.markAsTouched();
          control.setErrors({ ...control.errors, serverError: n.message });
        } else {
          this.errorMessage = this.errorMessage
            ? `${this.errorMessage}, ${n.message}`
            : n.message;
        }
      });
    } else {
      this.errorMessage = fallback;
    }
  }
}
