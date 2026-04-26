export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string | null;
  city: string;
  country: string;
  isActive: boolean;
  createdAt: string;
  lastModifiedAt?: string | null;
}

export interface CustomerFilter {
  name?: string | null;
  city?: string | null;
  country?: string | null;
  isActive?: boolean | null;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}

export interface UpsertCustomerDto {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string | null;
  city: string;
  country: string;
}
