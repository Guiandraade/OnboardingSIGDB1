import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { generatePageNumbers } from 'src/app/shared/utils/pagination.util';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaginationComponent {
  @Input() currentPage!: number;
  @Input() totalPages!: number;
  @Input() totalItems!: number;
  @Input() pageSize!: number;
  @Input() isLoading = false;
  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  readonly pageSizeOptions = [5, 10, 20, 50];

  get pageNumbers(): (number | string)[] {
    return generatePageNumbers(this.totalPages, this.currentPage);
  }

  onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    this.pageSizeChange.emit(value);
  }
}
