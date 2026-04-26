import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { CarService } from '../../../services/car.service';
import { Router } from '@angular/router';
import { Form, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Car, CarFilter } from '../../../models/car-models/car.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-cars-page',
  standalone: true,
  imports: [],
  templateUrl: './cars-page.html',
  styleUrl: './cars-page.scss',
})
export class CarsPage {
  private readonly carService = inject(CarService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  cars = signal<Car[]>([]);
  totalCount = signal(0);
  loading = signal(false);
  actionLoadingIds = signal<number[]>([]);
  showFilters = signal(false);

  pageNumber = signal(1);
  pageSize = signal(10);

  sortBy = signal('id');
  sortDirection = signal<'asc' | 'desc'>('asc');

  selectedCarIds = signal<number[]>([]);

  readonly activeOptions = [
    { label: 'Svi', value: null },
    { label: 'Aktivni', value: true },
    { label: 'Neaktivni', value: false },
  ];

  readonly colorsOptions = [
    { label: 'Svi', value: null },
    { label: 'Crvena', value: 'Crvena' },
    { label: 'Plava', value: 'Plava' },
    { label: 'Zelena', value: 'Zelena' },
    { label: 'Crna', value: 'Crna' },
    { label: 'Bijela', value: 'Bijela' },
  ];

  readonly makesOptions = [
    { label: 'Svi', value: null },
    { label: 'Toyota', value: 'Toyota' },
    { label: 'Honda', value: 'Honda' },
    { label: 'Ford', value: 'Ford' },
    { label: 'Volkswagen', value: 'Volkswagen' },
    { label: 'BMW', value: 'BMW' },
  ];

  readonly pageSizeOptions = [5, 10, 20, 50, 100];
  readonly first = computed(() => (this.pageNumber() - 1) * this.pageSize());
  readonly selectedCount = computed(() => this.selectedCarIds().length);
  readonly hasSelectedCars = computed(() => this.selectedCount() > 0);

  readonly allVisibleSelected = computed(() => {
    const visibleCarIds = this.cars().map((car) => car.id);
    return (
      visibleCarIds.length > 0 &&
      visibleCarIds.every((id) => this.selectedCarIds().includes(id))
    );
  });

  filterForm: FormGroup;
  makeControl = new FormControl<string | null>(null);
  modelControl = new FormControl<string | null>(null);
  yearControl = new FormControl<number | null>(null);
  colorControl = new FormControl<string | null>(null);
  priceMinControl = new FormControl<number | null>(null);
  priceMaxControl = new FormControl<number | null>(null);
  isActiveControl = new FormControl<boolean | null>(null);

  constructor() {
    this.filterForm = this.fb.group({
      make: this.makeControl,
      model: this.modelControl,
      year: this.yearControl,
      color: this.colorControl,
      priceMin: this.priceMinControl,
      priceMax: this.priceMaxControl,
      isActive: this.isActiveControl,
    });
  }

  private buildFilter(): CarFilter {
    return {
      make: this.makeControl.value,
      model: this.modelControl.value,
      year: this.yearControl.value,
      color: this.colorControl.value,
      priceMin: this.priceMinControl.value,
      priceMax: this.priceMaxControl.value,
      isActive: this.isActiveControl.value,
      sortBy: this.sortBy(),
      sortDirection: this.sortDirection(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }

  loadCars(): void {
    this.loading.set(true);

    const filter = this.buildFilter();

    this.carService
      .getAll(filter)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.cars.set(result.items);
          this.totalCount.set(result.totalCount);
          this.pageNumber.set(result.pageNumber);
          this.pageSize.set(result.pageSize);
          // this.syncSelectionWithCurrentData(result.items);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Greška pri dohvaćanju automobila', err);
          this.loading.set(false);
        },
      });
  }

  private handleFilterChanges(): void {
    this.filterForm.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageNumber.set(1);
        this.loadCars();
      });

    this.showFilters.update((show) => !show);
  }

  toggleFilters(): void {
    this.showFilters.update((value) => !value);
  }

  clearFilters(): void {
    this.filterForm.reset(
      {
        make: null,
        model: null,
        year: null,
        color: null,
        priceMin: null,
        priceMax: null,
        isActive: null,
      },
      { emitEvent: false },
    );

    this.sortBy.set('id');
    this.sortDirection.set('asc');
    this.pageNumber.set(1);
    this.pageSize.set(10);
    this.loadCars();
  }

  onPageChange(event: { first: number; rows: number }): void {
    const newPageNumber = Math.floor(event.first / event.rows) + 1;
    this.pageNumber.set(newPageNumber);
    this.pageSize.set(event.rows);
    this.loadCars();
  }

  onSort(field: string): void {
    if (this.sortBy() === field) {
      this.sortDirection.update((dir) => (dir === 'asc' ? 'desc' : 'asc'));
    } else {
      this.sortBy.set(field);
      this.sortDirection.set('asc');
    }
    this.pageNumber.set(1);
    this.loadCars();
  }

  toggleCarSelection(carId: number, checked: boolean): void {
    this.selectedCarIds.update((ids) => {
      if (checked) {
        return ids.includes(carId) ? ids : [...ids, carId];
      }
      return ids.filter((id) => id !== carId);
    });
  }

  toggleSelectAllVisible(checked: boolean): void {
    const visibleCarIds = this.cars().map((car) => car.id);
    if (checked) {
      this.selectedCarIds.update((ids) => [
        ...new Set([...ids, ...visibleCarIds]),
      ]);
    } else {
      this.selectedCarIds.update((ids) =>
        ids.filter((id) => !visibleCarIds.includes(id)),
      );
    }
  }
}
