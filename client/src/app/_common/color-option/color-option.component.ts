import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { fnIsLightColor } from '../function/function';

@Component({
  selector: 'app-color-option',
  templateUrl: './color-option.component.html',
  styleUrls: ['./color-option.component.css']
})
export class ColorOptionComponent implements OnInit {
  @Input() colorCode: string;
  @Input() colorName: string;
  @Input() isSelected: boolean;
  @Input() isShowColorOnly: boolean = false;
  @Input() isDisabled: boolean = false;
  @Output() select = new EventEmitter<boolean>();

  checkMarkColorCode: string = '#fff';

  constructor() { 
    //Not implement
  }

  ngOnInit(): void {
    //Not implement
    if (fnIsLightColor(this.colorCode))
      this.checkMarkColorCode = '#000'
    else
      this.checkMarkColorCode = '#fff';
  }

  selectColor()
  {    
    this.select.emit(true);
  }


}
