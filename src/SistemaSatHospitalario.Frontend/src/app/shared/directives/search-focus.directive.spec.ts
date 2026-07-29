import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SearchFocusDirective } from './search-focus.directive';

@Component({
  standalone: true,
  imports: [SearchFocusDirective],
  template: `<input #searchInput appSearchFocus type="text" />`
})
class TestHostComponent {}

describe('SearchFocusDirective', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let inputEl: HTMLInputElement;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [TestHostComponent]
    });
    fixture = TestBed.createComponent(TestHostComponent);
    fixture.detectChanges();
    inputEl = fixture.nativeElement.querySelector('input');
  });

  it('debe hacer focus en el input al presionar Alt + K', () => {
    const focusSpy = spyOn(inputEl, 'focus');
    const selectSpy = spyOn(inputEl, 'select');

    const event = new KeyboardEvent('keydown', {
      key: 'k',
      altKey: true,
      bubbles: true,
      cancelable: true
    });

    window.dispatchEvent(event);

    expect(focusSpy).toHaveBeenCalled();
    expect(selectSpy).toHaveBeenCalled();
  });

  it('debe hacer focus en el input al presionar Ctrl + Shift + K', () => {
    const focusSpy = spyOn(inputEl, 'focus');
    const selectSpy = spyOn(inputEl, 'select');

    const event = new KeyboardEvent('keydown', {
      key: 'k',
      ctrlKey: true,
      shiftKey: true,
      bubbles: true,
      cancelable: true
    });

    window.dispatchEvent(event);

    expect(focusSpy).toHaveBeenCalled();
    expect(selectSpy).toHaveBeenCalled();
  });
});
