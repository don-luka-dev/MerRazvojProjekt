import { CommonModule } from '@angular/common';
import { Component, inject, input, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { Customer } from '../../models/customer.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customers-table',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TagModule,
    PaginatorModule,
  ],
  templateUrl: './customers-table.html',
  styleUrl: './customers-table.scss',
})
export class CustomersTableComponent {
  private readonly router = inject(Router);

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
  isActionLoadingFn = input.required<(customerId: number) => boolean>();
  getSortIconFn = input.required<(field: string) => string>();

  pageChange = output<{ first: number; rows: number }>();
  sortChange = output<string>();
  rowClick = output<Customer>();
  toggleSelectAll = output<boolean>();
  toggleSelection = output<{ customerId: number; checked: boolean }>();
  deactivate = output<Customer>();

  isSelected(customerId: number): boolean {
    return this.selectedCustomerIds().includes(customerId);
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
    this.router.navigate(['/customers', customer.id]);
  }

  onSortClick(field: string): void {
    this.sortChange.emit(field);
  }

  onPageChanged(event: any): void {
    this.pageChange.emit({ first: event.first, rows: event.rows });
  }

  onRowClicked(customer: Customer): void {
    this.rowClick.emit(customer);
  }
}
