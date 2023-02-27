import { Component, Input, Self, EventEmitter, Output } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';


@Component({
  selector: 'app-icon-text-input',
  templateUrl: './icon-text-input.component.html',
  styleUrls: ['./icon-text-input.component.css']
})
export class IconTextInputComponent implements ControlValueAccessor {

  @Input() label: string;
  @Input() type = 'text';
  @Input() icon = 'fa-search';
  @Input() separatorCount = 0;

  @Output() clearInput = new EventEmitter<boolean>();

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }
  writeValue(obj: any): void {
    // Not implemented
  }
  registerOnChange(fn: any): void {
    // Not implemented
  }
  registerOnTouched(fn: any): void {
    // Not implemented
  }  

  ngOnInit(): void {
    // Not implemented
  }

  clear()
  {
    this.clearInput.emit(true);
  }

}
