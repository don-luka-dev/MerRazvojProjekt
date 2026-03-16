import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'customers',
    pathMatch: 'full',
  },
  {
    path: 'customers',
    loadComponent: () =>
      import('./components/customers-page/customers-page').then(
        (m) => m.CustomersPageComponent,
      ),
  },
  {
    path: 'customers/:id',
    loadComponent: () =>
      import('./components/customer-details/customer-details').then(
        (m) => m.CustomerDetailsComponent,
      ),
  },
  {
    path: '**',
    redirectTo: 'customers',
  },
];
