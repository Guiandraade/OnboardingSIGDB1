import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';

export interface SearchSelectItem {
  id: number;
  displayName: string;
}

@Component({
  selector: 'app-search-select',
  templateUrl: './search-select.component.html',
  styleUrls: ['./search-select.component.css']
})
export class SearchSelectComponent {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() inputId = '';
  @Input() items: SearchSelectItem[] = [];
  @Input() control: AbstractControl | null = null;
  @Input() requiredMessage = 'This field is required.';
  @Input() emptyMessage = 'No results found';

  searchText = '';
  showDropdown = false;
  private selectedItem: SearchSelectItem | null = null;

  get filteredItems(): SearchSelectItem[] {
    const q = this.searchText.toLowerCase();
    return q ? this.items.filter(i => i.displayName.toLowerCase().includes(q)) : this.items;
  }

  onInput(event: Event): void {
    this.searchText = (event.target as HTMLInputElement).value;
    this.showDropdown = true;
  }

  selectItem(item: SearchSelectItem): void {
    this.selectedItem = item;
    this.control?.setValue(item.id);
    this.searchText = item.displayName;
    this.showDropdown = false;
  }

  onBlur(): void {
    this.control?.markAsTouched();
    setTimeout(() => {
      this.showDropdown = false;
      this.searchText = this.selectedItem?.displayName ?? '';
    }, 150);
  }

  trackByIndex(index: number): number {
    return index;
  }
}
