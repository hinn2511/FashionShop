import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { SelectOption, SingleSelectedResult } from 'src/app/_models/dialog';

@Component({
  selector: 'app-single-option-dialog',
  templateUrl: './single-option-dialog.component.html',
  styleUrls: ['./single-option-dialog.component.css']
})
export class SingleOptionDialogComponent implements OnInit {

    options: SelectOption[];
    title: string;
    message: string;
    btnYesText: string;
    btnNoText: string;
    selectedResult: SingleSelectedResult = new SingleSelectedResult(false, 0, "");

    optionTableTitle: string;
  
    constructor(public bsModalRef: BsModalRef) { }
  
    ngOnInit(): void {
    }
  
    confirm() {
      this.selectedResult.result = true;      
      this.bsModalRef.hide();
    }
  
    decline() {
      this.selectedResult.result = false;
      this.bsModalRef.hide();
    }
  
    selectOption(option: SelectOption)
    {
      this.selectedResult.selectedId = option.id;
      this.selectedResult.selectedValue = option.value;
    }
  
  }