export interface CustomerCityCount {
  city: string;
  count: number;
}

export interface CustomerStats {
  totalCount: number;
  activeCount: number;
  inactiveCount: number;
  topCities: CustomerCityCount[];
}
