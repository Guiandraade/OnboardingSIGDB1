import { AbstractControl, ValidationErrors } from '@angular/forms';
import { ValidationRule } from '../components/validation-rules/validation-rules.component';

export function buildMinMaxRules(
  control: AbstractControl | null,
  min: number,
  max: number
): ValidationRule[] {
  const len = control?.value?.length ?? 0;
  const touched = !!control?.touched;
  return [
    { text: `At least ${min} characters`, passed: len >= min, failed: touched && len > 0 && len < min },
    { text: `Maximum ${max} characters`,  passed: len <= max && len > 0, failed: touched && len > max }
  ];
}

export function noFutureDateValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) return null;
  const match = /^(\d{4})-(\d{2})-(\d{2})/.exec(control.value as string);
  if (!match) return null;
  const inputDate = new Date(+match[1], +match[2] - 1, +match[3]);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return inputDate > today ? { futureDate: true } : null;
}

export function cnpjValidator(control: AbstractControl): ValidationErrors | null {
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

export function cpfValidator(control: AbstractControl): ValidationErrors | null {
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
