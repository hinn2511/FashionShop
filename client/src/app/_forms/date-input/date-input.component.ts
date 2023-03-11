import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() maxDate: Date;
  @Input() minDate: Date;
  locale = 'vi';
  bsConfig: Partial<BsDatepickerConfig>;
  
  constructor(@Self() public ngControl: NgControl, private localeService: BsLocaleService) {
    this.ngControl.valueAccessor = this;
    this.localeService.use(this.locale);
    this.bsConfig = {
      containerClass: 'theme-green',
      // dateInputFormat: 'DD MMMM YYYY'
      dateInputFormat: 'DD/MM/YYYY',
      adaptivePosition: true
    }
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

}
