import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Customer,
  CustomerFilter,
  PagedResultDto,
  UpdateCustomerDto,
} from '../models/customer.model';
import { CustomerStats } from '../models/customer-stats.model';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'https://localhost:7271/api/Customer';

  getAll(filter: CustomerFilter): Observable<PagedResultDto<Customer>> {
    let params = new HttpParams();

    if (filter.name) params = params.set('Name', filter.name);
    if (filter.city) params = params.set('City', filter.city);
    if (filter.country) params = params.set('Country', filter.country);
    if (filter.isActive !== null && filter.isActive !== undefined) {
      params = params.set('IsActive', filter.isActive);
    }
    if (filter.sortBy) params = params.set('SortBy', filter.sortBy);
    if (filter.sortDirection)
      params = params.set('SortDirection', filter.sortDirection);
    if (filter.pageNumber) params = params.set('PageNumber', filter.pageNumber);
    if (filter.pageSize) params = params.set('PageSize', filter.pageSize);

    return this.http.get<PagedResultDto<Customer>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.baseUrl}/${id}`);
  }

  update(id: number, body: UpdateCustomerDto): Observable<Customer> {
    return this.http.put<Customer>(`${this.baseUrl}/${id}`, body);
  }

  deactivate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  bulkDeactivate(customerIds: number[]): Observable<{ updatedCount: number }> {
    return this.http.post<{ updatedCount: number }>(
      `${this.baseUrl}/bulk-deactivate`,
      customerIds,
    );
  }

  getStats(): Observable<CustomerStats> {
    return this.http.get<CustomerStats>(`${this.baseUrl}/stats`);
  }
}
