import { CommonModule } from '@angular/common';
import { Component, input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { CustomerStats } from '../../models/customer-stats.model';

@Component({
  selector: 'app-customer-stats',
  standalone: true,
  imports: [CommonModule, CardModule],
  templateUrl: './customer-stats.html',
  styleUrl: './customer-stats.scss',
})
export class CustomerStatsComponent {
  stats = input<CustomerStats | null>(null);
  loading = input<boolean>(false);
}
