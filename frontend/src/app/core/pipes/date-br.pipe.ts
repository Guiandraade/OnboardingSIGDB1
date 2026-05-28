import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'dateBr' })
export class DateBrPipe implements PipeTransform {
  transform(value: string | null): string {
    if (!value) {
      return '-';
    }

    const [year, month, day] = value.substring(0, 10).split('-');
    return `${day}/${month}/${year}`;
  }
}
