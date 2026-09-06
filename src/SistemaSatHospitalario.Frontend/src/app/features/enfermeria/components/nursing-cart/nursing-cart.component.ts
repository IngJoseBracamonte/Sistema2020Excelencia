import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartItem } from '../../enfermeria.component';
import { LucideAngularModule, Trash2, Edit3, ShoppingCart, Loader } from 'lucide-angular';

@Component({
  selector: 'app-nursing-cart',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './nursing-cart.component.html'
})
export class NursingCartComponent {
  @Input() items: CartItem[] = [];
  @Input() cartTotalUSD = 0;
  @Input() isSaving = false;
  @Input() patientName: string | null = null;

  @Output() removeItem = new EventEmitter<string>();
  @Output() editItem = new EventEmitter<string>();
  @Output() registerAll = new EventEmitter<void>();

  readonly icons = {
    Trash2,
    Edit3,
    ShoppingCart,
    Loader
  };
}
