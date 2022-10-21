import { Component, Input, OnInit, Self } from '@angular/core';
import { NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-area-input',
  templateUrl: './text-area-input.component.html',
  styleUrls: ['./text-area-input.component.css']
})
export class TextAreaInputComponent implements OnInit {

  @Input() label: string;
  @Input() rows: number;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }
  
  registerOnTouched(fn: any): void {
  }

  ngOnInit(): void {
  }

}
