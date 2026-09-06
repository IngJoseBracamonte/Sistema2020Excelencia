import { Directive, ElementRef, HostListener, inject } from '@angular/core';

@Directive({
  selector: '[appSearchFocus]',
  standalone: true
})
export class SearchFocusDirective {
  private el = inject(ElementRef<HTMLInputElement>);

  @HostListener('window:keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    const isAltK = event.altKey && event.key.toLowerCase() === 'k';
    const isCtrlShiftK = (event.ctrlKey || event.metaKey) && event.shiftKey && event.key.toLowerCase() === 'k';

    if (isAltK || isCtrlShiftK) {
      event.preventDefault();
      event.stopPropagation();
      this.el.nativeElement.focus();
      this.el.nativeElement.select();
    }
  }
}
