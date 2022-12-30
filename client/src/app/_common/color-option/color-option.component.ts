import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

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

  constructor() { 
    //Not implement
  }

  ngOnInit(): void {
    //Not implement
  }

  selectColor()
  {    
    this.select.emit(true);
  }


}
