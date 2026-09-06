import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filterByDay',
  standalone: true
})
export class FilterByDayPipe implements PipeTransform {
  transform(items: any[], day: number): any[] {
    if (!items) return [];
    return items.filter(item => item.diaSemana === day);
  }
}
