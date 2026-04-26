import { Component, input } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-customers-filters',
  standalone: true,
  imports: [ReactiveFormsModule, InputTextModule, SelectModule],
  templateUrl: './customer-filters.html',
  styleUrl: './customer-filters.scss',
})
export class CustomersFiltersComponent {
  filterForm = input.required<FormGroup>();
  nameControl = input.required<FormControl<string | null>>();
  activeOptions = input.required<
    {
      label: string;
      value: boolean | null;
    }[]
  >();

  citiesOptions = input.required<
    {
      label: string;
      value: string | null;
    }[]
  >();
}
