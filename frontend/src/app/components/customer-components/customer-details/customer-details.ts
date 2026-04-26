import { Component, DestroyRef, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';

import {
  Customer,
  UpsertCustomerDto,
} from '../../../models/customer-models/customer.model';
import { CustomerService } from '../../../services/customer.service';

import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-customer-details',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    CardModule,
    ToastModule,
  ],
  providers: [MessageService],
  templateUrl: './customer-details.html',
  styleUrl: './customer-details.scss',
})
export class CustomerDetailsComponent {
  private readonly destroyRef = inject(DestroyRef);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly customerService = inject(CustomerService);
  private readonly fb = inject(FormBuilder);
  private readonly messageService = inject(MessageService);

  customerId = signal<number | null>(null);
  loading = signal(false);
  saving = signal(false);
  customer = signal<Customer | null>(null);

  form = this.fb.group({
    firstName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2)],
    }),
    lastName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(2)],
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    phone: new FormControl<string | null>(null),
    city: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    country: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id || Number.isNaN(id)) {
      this.router.navigate(['/customers']);
      return;
    }

    this.customerId.set(id);
    this.loadCustomer(id);
  }

  loadCustomer(id: number): void {
    this.loading.set(true);

    this.customerService
      .getById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (customer) => {
          this.customer.set(customer);

          this.form.patchValue({
            firstName: customer.firstName,
            lastName: customer.lastName,
            email: customer.email,
            phone: customer.phone ?? null,
            city: customer.city,
            country: customer.country,
          });

          this.loading.set(false);
        },
        error: (err) => {
          console.error('Greška pri dohvaćanju customer details', err);
          this.loading.set(false);
          this.router.navigate(['/customers']);
        },
      });
  }

  saveChanges(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const id = this.customerId();
    if (!id) return;

    const request: UpsertCustomerDto = {
      firstName: this.form.controls.firstName.value,
      lastName: this.form.controls.lastName.value,
      email: this.form.controls.email.value,
      phone: this.form.controls.phone.value?.trim() || null,
      city: this.form.controls.city.value,
      country: this.form.controls.country.value,
    };

    this.saving.set(true);

    this.customerService
      .update(id, request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (updatedCustomer) => {
          this.customer.set(updatedCustomer);
          this.form.markAsPristine();
          this.saving.set(false);
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Customer updated successfully.',
          });
        },
        error: (err) => {
          console.error('Greška pri spremanju customera', err);
          this.saving.set(false);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update customer.',
          });
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/customers']);
  }
}
