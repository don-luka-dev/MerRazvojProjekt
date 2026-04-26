import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
} from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import {
  Customer,
  CustomerFilter,
} from '../../../models/customer-models/customer.model';
import { Router } from '@angular/router';
import { CustomerService } from '../../../services/customer.service';
import { CustomersFiltersComponent } from '../customer-filters/customer-filters';
import { CustomersTableComponent } from '../customers-table/customers-table';
import { ButtonModule } from 'primeng/button';
import { CustomerStats } from '../../../models/customer-models/customer-stats.model';
import { CustomerStatsComponent } from '../customer-stats/customer-stats';

@Component({
  selector: 'app-customers-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CustomersFiltersComponent,
    CustomersTableComponent,
    CustomerStatsComponent,
    ButtonModule,
  ],
  templateUrl: './customers-page.html',
  styleUrl: './customers-page.scss',
})
export class CustomersPageComponent {
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly customerService = inject(CustomerService);

  customers = signal<Customer[]>([]);
  totalCount = signal(0);
  loading = signal(false);
  actionLoadingIds = signal<number[]>([]);
  showFilters = signal(false);

  pageNumber = signal(1);
  pageSize = signal(10);

  sortBy = signal('id');
  sortDirection = signal<'asc' | 'desc'>('asc');

  selectedCustomerIds = signal<number[]>([]);

  stats = signal<CustomerStats | null>(null);
  statsLoading = signal(false);

  filterForm: FormGroup;
  nameControl = new FormControl<string | null>(null);

  readonly activeOptions = [
    { label: 'Svi', value: null },
    { label: 'Aktivni', value: true },
    { label: 'Neaktivni', value: false },
  ];

  readonly citiesOptions = [
    { label: 'Svi', value: null },
    { label: 'Zagreb', value: 'Zagreb' },
    { label: 'Split', value: 'Split' },
    { label: 'Osijek', value: 'Osijek' },
    { label: 'Sarajevo', value: 'Sarajevo' },
    { label: 'Belgrade', value: 'Belgrade' },
    { label: 'Ljubljana', value: 'Ljubljana' },
    { label: 'Mostar', value: 'Mostar' },
    { label: 'Maribor', value: 'Maribor' },
  ];

  readonly pageSizeOptions = [5, 10, 20, 50, 100];
  readonly first = computed(() => (this.pageNumber() - 1) * this.pageSize());
  readonly selectedCount = computed(() => this.selectedCustomerIds().length);
  readonly hasSelectedCustomers = computed(() => this.selectedCount() > 0);

  readonly allVisibleSelected = computed(() => {
    const activeCustomers = this.customers().filter((c) => c.isActive);
    const selectedIds = this.selectedCustomerIds();

    if (activeCustomers.length === 0) return false;

    return activeCustomers.every((customer) =>
      selectedIds.includes(customer.id),
    );
  });

  constructor() {
    this.filterForm = this.fb.group({
      city: new FormControl<string | null>(null),
      country: new FormControl<string | null>(null),
      isActive: new FormControl<boolean | null>(null),
    });

    this.handleFilterChanges();
    this.loadCustomers();
    this.loadStats();
  }

  private buildFilter(): CustomerFilter {
    const rawName = this.nameControl.value?.trim() || null;

    return {
      name: rawName && rawName.length >= 2 ? rawName : null,
      city: this.filterForm.value.city?.trim() || null,
      country: this.filterForm.value.country?.trim() || null,
      isActive: this.filterForm.value.isActive,
      sortBy: this.sortBy(),
      sortDirection: this.sortDirection(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }

  loadCustomers(): void {
    this.loading.set(true);

    this.customerService
      .getAll(this.buildFilter())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.customers.set(result.items);
          this.totalCount.set(result.totalCount);
          this.pageNumber.set(result.pageNumber);
          this.pageSize.set(result.pageSize);
          this.syncSelectionWithCurrentData(result.items);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Greška pri dohvaćanju customera', err);
          this.loading.set(false);
        },
      });
  }

  private handleFilterChanges(): void {
    this.filterForm.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageNumber.set(1);
        this.loadCustomers();
      });

    this.nameControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((value) => {
        const trimmed = value?.trim() ?? '';

        if (trimmed.length === 0 || trimmed.length >= 2) {
          this.pageNumber.set(1);
          this.loadCustomers();
        }
      });

    this.showFilters.update((show) => !show);
  }

  toggleFilters(): void {
    this.showFilters.update((value) => !value);
  }

  clearFilters(): void {
    this.nameControl.setValue(null, { emitEvent: false });
    this.filterForm.reset(
      {
        city: null,
        country: null,
        isActive: null,
      },
      { emitEvent: false },
    );

    this.sortBy.set('id');
    this.sortDirection.set('asc');
    this.pageNumber.set(1);
    this.pageSize.set(10);
    this.selectedCustomerIds.set([]);

    this.loadCustomers();
  }

  onPageChange(event: { first: number; rows: number }): void {
    this.pageNumber.set(Math.floor(event.first / event.rows) + 1);
    this.pageSize.set(event.rows);
    this.loadCustomers();
  }

  onSort(field: string): void {
    if (this.sortBy() === field) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortBy.set(field);
      this.sortDirection.set('asc');
    }

    this.pageNumber.set(1);
    this.loadCustomers();
  }

  toggleCustomerSelection(customerId: number, checked: boolean): void {
    const current = this.selectedCustomerIds();

    if (checked) {
      if (!current.includes(customerId)) {
        this.selectedCustomerIds.set([...current, customerId]);
      }
      return;
    }

    this.selectedCustomerIds.set(current.filter((id) => id !== customerId));
  }

  toggleSelectAllVisible(checked: boolean): void {
    const visibleIds = this.customers().map((customer) => customer.id);

    if (checked) {
      this.selectedCustomerIds.update((ids) => [
        ...new Set([...ids, ...visibleIds]),
      ]);
    } else {
      this.selectedCustomerIds.update((ids) =>
        ids.filter((id) => !visibleIds.includes(id)),
      );
    }
  }

  deactivateCustomer(customer: Customer): void {
    if (!customer.isActive) return;

    const confirmed = window.confirm(
      `Deactivate customer ${customer.firstName} ${customer.lastName}?`,
    );

    if (!confirmed) return;

    this.setActionLoading(customer.id, true);

    this.customerService
      .deactivate(customer.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.setActionLoading(customer.id, false);
          this.selectedCustomerIds.update((ids) =>
            ids.filter((id) => id !== customer.id),
          );
          this.loadCustomers();
          this.loadStats();
        },
        error: (err) => {
          console.error('Greška pri deaktivaciji customera', err);
          this.setActionLoading(customer.id, false);
        },
      });
  }

  bulkDeactivateSelected(): void {
    const ids = this.selectedCustomerIds();

    if (ids.length === 0) return;

    const confirmed = window.confirm(
      `Deactivate ${ids.length} selected customer(s)?`,
    );

    if (!confirmed) return;

    this.loading.set(true);

    this.customerService
      .bulkDeactivate(ids)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.selectedCustomerIds.set([]);
          this.loadCustomers();
          this.loadStats();
        },
        error: (err) => {
          console.error('Greška pri bulk deaktivaciji', err);
          this.loading.set(false);
        },
      });
  }

  onRowClick(customer: Customer): void {
    this.router.navigate(['/customers', customer.id]);
  }

  private setActionLoading(customerId: number, loading: boolean): void {
    if (loading) {
      if (!this.actionLoadingIds().includes(customerId)) {
        this.actionLoadingIds.set([...this.actionLoadingIds(), customerId]);
      }
      return;
    }

    this.actionLoadingIds.set(
      this.actionLoadingIds().filter((id) => id !== customerId),
    );
  }

  private syncSelectionWithCurrentData(customers: Customer[]): void {
    const activeCustomerIds = new Set(
      customers
        .filter((customer) => customer.isActive)
        .map((customer) => customer.id),
    );

    this.selectedCustomerIds.update((ids) =>
      ids.filter(
        (id) =>
          activeCustomerIds.has(id) ||
          !customers.some((customer) => customer.id === id),
      ),
    );
  }

  loadStats(): void {
    this.statsLoading.set(true);

    this.customerService
      .getStats()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.stats.set(result);
          this.statsLoading.set(false);
        },
        error: (err) => {
          console.error('Greška pri dohvaćanju statistike', err);
          this.statsLoading.set(false);
        },
      });
  }
}
