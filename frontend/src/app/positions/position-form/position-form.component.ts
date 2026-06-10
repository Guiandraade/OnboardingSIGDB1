import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { PositionService } from 'src/app/_common/_services/position.service';
import { ToastService } from 'src/app/_shared/toast.service';
import { Notification } from 'src/app/_common/_models/pagination.model';

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
  hasServerError = false;
  cooldownSeconds = 0;
  private cooldownTimer: any = null;
  private errorStreak = 0;

  get isEdit(): boolean {
    return this.editId !== null;
  }

  get descriptionControl() {
    return this.form.get('description');
  }

  get descriptionLength(): number {
    return this.descriptionControl?.value?.length ?? 0;
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

    this.descriptionControl?.valueChanges.subscribe(() => {
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
    this.positionService.getById(id).subscribe({
      next: (item) => {
        this.form.patchValue({ description: item.description });
        this.isLoading = false;
      },
      error: (err) => {
        this.handleServerErrors(err, 'Error loading position.');
        this.isLoading = false;
      }
    });
  }

  save(): void {
    if (this.isSaving || this.cooldownSeconds > 0) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    const request = this.form.value;

    const operation = this.isEdit
      ? this.positionService.update(this.editId!, request)
      : this.positionService.create(request);

    operation.subscribe({
      next: () => {
        this.errorStreak = 0;
        this.toastService.success(
          this.isEdit ? 'Position updated successfully.' : 'Position created successfully.'
        );
        this.router.navigate(['/positions']);
      },
      error: (err) => {
        this.handleServerErrors(
          err,
          this.isEdit ? 'Error updating position.' : 'Error creating position.'
        );
        this.isSaving = false;
        this.startCooldown();
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/positions']);
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
    this.descriptionControl?.markAsTouched();
  }
}
