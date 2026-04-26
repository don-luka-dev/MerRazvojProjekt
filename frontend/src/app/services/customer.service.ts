import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Customer,
  CustomerFilter,
  UpsertCustomerDto,
} from '../models/customer-models/customer.model';
import { PagedResultDto } from '../models/pagedResultDto.model';
import { CustomerStats } from '../models/customer-models/customer-stats.model';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'https://localhost:7271/api/Customer';

  getAll(filter: CustomerFilter): Observable<PagedResultDto<Customer>> {
    let params = new HttpParams();

    const addParam = (key: string, value: unknown) => {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    };

    addParam('Name', filter.name);
    addParam('City', filter.city);
    addParam('Country', filter.country);
    addParam('IsActive', filter.isActive);
    addParam('SortBy', filter.sortBy);
    addParam('SortDirection', filter.sortDirection);
    addParam('PageNumber', filter.pageNumber);
    addParam('PageSize', filter.pageSize);

    return this.http.get<PagedResultDto<Customer>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.baseUrl}/${id}`);
  }

  update(id: number, body: UpsertCustomerDto): Observable<Customer> {
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
