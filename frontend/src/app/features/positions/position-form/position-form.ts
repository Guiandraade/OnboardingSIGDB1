import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PositionService } from 'src/app/core/services/position.service';
import { Notification } from 'src/app/core/models/pagination.model';

@Component({
  selector: 'app-position-form',
  templateUrl: './position-form.html',
  styleUrls: ['./position-form.css']
})
export class PositionForm implements OnInit {

  form!: FormGroup;
  isEditMode = false;
  positionId: number | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private positionService: PositionService
  ) { }

  ngOnInit(): void {
    this.initializeForm();

    this.verifyEditMode();
  }

  private initializeForm(): void {
    this.form = this.fb.group({
      description: ['', [Validators.minLength(3), Validators.maxLength(100), Validators.required]]
    });
  }

  private verifyEditMode(): void {
    const idParam = this.route.snapshot.paramMap.get('id');

    if (idParam) {
      this.isEditMode = true;
      this.positionId = Number(idParam);

      this.isLoading = true;
      this.positionService.getPositionById(this.positionId).subscribe({
        next: (position) => {
          this.form.patchValue(position);
          this.isLoading = false;
        },
        error: (err) => {
          this.isLoading = false;
          if (Array.isArray(err.error)) {
            this.errorMessage = err.error.map((n: Notification) => n.message).join(', ');
          } else {
            this.errorMessage = 'Error fetching position';
          }
          console.error('Error fetching position:', err);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }
    this.isLoading = true;

    const positionData = this.form.value;

    if (this.isEditMode && this.positionId !== null) {
      this.positionService.updatePosition(this.positionId, positionData).subscribe({
        next: () => {
          this.isLoading = false;
          this.backToList();
        },
        error: (err) => {
          this.isLoading = false;
          if (Array.isArray(err.error)) {
            this.errorMessage = err.error.map((n: Notification) => n.message).join(', ');
          } else {
            this.errorMessage = 'Error updating position';
          }
          console.error('Error updating position:', err);
        }
      });
    } else {
      this.positionService.createPosition(positionData).subscribe({
        next: () => {
          this.isLoading = false;
          this.backToList();
        },
        error: (err) => {
          this.isLoading = false;
          if (Array.isArray(err.error)) {
            this.errorMessage = err.error.map((n: Notification) => n.message).join(', ');
          } else {
            this.errorMessage = 'Error creating position';
          }
          console.error('Error creating position:', err);
        }
      });
    }
  }

  private backToList(): void {
    this.router.navigate(['/positions']);
  }
}
