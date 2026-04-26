import { Component, input, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { Customer } from '../../../models/customer-models/customer.model';

@Component({
  selector: 'app-customers-table',
  standalone: true,
  imports: [TableModule, ButtonModule, TagModule, PaginatorModule],
  templateUrl: './customers-table.html',
  styleUrl: './customers-table.scss',
})
export class CustomersTableComponent {
  customers = input.required<Customer[]>();
  loading = input.required<boolean>();
  totalCount = input.required<number>();
  selectedCount = input.required<number>();
  selectedCustomerIds = input.required<number[]>();
  allVisibleSelected = input.required<boolean>();
  pageSize = input.required<number>();
  first = input.required<number>();
  pageSizeOptions = input.required<number[]>();
  sortBy = input.required<string>();
  sortDirection = input.required<'asc' | 'desc'>();
  actionLoadingIds = input.required<number[]>();

  pageChange = output<{ first: number; rows: number }>();
  sortChange = output<string>();
  rowClick = output<Customer>();
  toggleSelectAll = output<boolean>();
  toggleSelection = output<{ customerId: number; checked: boolean }>();
  deactivate = output<Customer>();

  isSelected(customerId: number): boolean {
    return this.selectedCustomerIds().includes(customerId);
  }

  isActionLoading(customerId: number): boolean {
    return this.actionLoadingIds().includes(customerId);
  }

  getSortIcon(field: string): string {
    if (this.sortBy() !== field) return 'pi pi-sort-alt';
    return this.sortDirection() === 'asc'
      ? 'pi pi-sort-amount-up-alt'
      : 'pi pi-sort-amount-down';
  }

  onHeaderCheckboxChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.toggleSelectAll.emit(checked);
  }

  onRowCheckboxChange(customerId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.toggleSelection.emit({ customerId, checked });
  }

  onDeactivateClick(customer: Customer, event: Event): void {
    event.stopPropagation();
    this.deactivate.emit(customer);
  }

  onEyeClick(customer: Customer, event: Event): void {
    event.stopPropagation();
    this.rowClick.emit(customer);
  }

  onSortClick(field: string): void {
    this.sortChange.emit(field);
  }

  onPageChanged(event: PaginatorState): void {
    this.pageChange.emit({ first: event.first ?? 0, rows: event.rows ?? 10 });
  }

  onRowClicked(customer: Customer): void {
    this.rowClick.emit(customer);
  }
}
