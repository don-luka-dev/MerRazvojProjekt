import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Car, CarFilter, UpsertCarDto } from '../models/car-models/car.model';
import { PagedResultDto } from '../models/pagedResultDto.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CarService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'https://localhost:7271/api/Car';

  getAll(filter: CarFilter): Observable<PagedResultDto<Car>> {
    let params = new HttpParams();

    const addParam = (key: string, value: unknown) => {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    };

    addParam('Make', filter.make);
    addParam('Model', filter.model);
    addParam('Year', filter.year);
    addParam('Color', filter.color);
    addParam('PriceMin', filter.priceMin);
    addParam('PriceMax', filter.priceMax);
    addParam('IsActive', filter.isActive);
    addParam('SortBy', filter.sortBy);
    addParam('SortDirection', filter.sortDirection);
    addParam('PageNumber', filter.pageNumber);
    addParam('PageSize', filter.pageSize);

    return this.http.get<PagedResultDto<Car>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Car> {
    return this.http.get<Car>(`${this.baseUrl}/${id}`);
  }

  add(car: UpsertCarDto): Observable<Car> {
    return this.http.post<Car>(this.baseUrl, car);
  }

  update(id: number, car: UpsertCarDto): Observable<Car> {
    return this.http.put<Car>(`${this.baseUrl}/${id}`, car);
  }

  deactivate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  bulkDeactivate(carIds: number[]): Observable<{ updatedCount: number }> {
    return this.http.post<{ updatedCount: number }>(
      `${this.baseUrl}/bulk-deactivate`,
      carIds,
    );
  }
}
