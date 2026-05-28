import { AbstractControl, ValidationErrors } from '@angular/forms';

export class CustomValidators {
  static cnpj(control: AbstractControl): ValidationErrors | null {
    const digits = (control.value ?? '').replace(/\D/g, '');
    if (!digits) {
      return null;
    }

    return digits.length === 14 ? null : { cnpjLength: true };
  }
}
