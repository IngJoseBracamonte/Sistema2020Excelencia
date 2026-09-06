import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StepperMode } from '../../enfermeria.component';

@Component({
  selector: 'app-dynamic-stepper',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dynamic-stepper.component.html'
})
export class DynamicStepperComponent {
  @Input() currentStep = 1;
  @Input() stepperMode: StepperMode = 'catalog';

  get stepLabels(): string[] {
    switch (this.stepperMode) {
      case 'consulta':
        return ['Búsqueda', 'Médico', 'Confirmación'];
      case 'lab-rx':
        return ['Búsqueda', 'Precios', 'Confirmación'];
      case 'medicamento':
        return ['Búsqueda', 'Cantidad', 'Confirmación'];
      case 'procedimiento':
        return ['Búsqueda', 'Ajustes', 'Confirmación'];
      default:
        return ['Búsqueda', 'Configuración', 'Confirmación'];
    }
  }
}
