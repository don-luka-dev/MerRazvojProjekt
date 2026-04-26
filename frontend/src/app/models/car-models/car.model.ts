export interface Car {
  id: number;
  make: string;
  model: string;
  year: number;
  price: number;
  color: string;
  isActive: boolean;
}

export interface CarFilter {
  make?: string | null;
  model?: string | null;
  year?: number | null;
  color?: string | null;
  priceMin?: number | null;
  priceMax?: number | null;
  isActive?: boolean | null;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}

export interface UpsertCarDto {
  make: string;
  model: string;
  year: number;
  price: number;
  color: string;
  isActive: boolean;
}
