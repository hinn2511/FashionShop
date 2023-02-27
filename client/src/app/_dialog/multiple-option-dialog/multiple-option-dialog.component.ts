import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { MultipleSelectedResult, OptionSelected, SelectOption } from 'src/app/_models/dialog';

@Component({
  selector: 'app-multiple-option-dialog',
  templateUrl: './multiple-option-dialog.component.html',
  styleUrls: ['./multiple-option-dialog.component.css'],
})
export class MultipleOptionDialogComponent implements OnInit {
  options: SelectOption[];
  title: string;
  message: string;
  btnYesText: string;
  btnNoText: string;
  selectedResult: MultipleSelectedResult = new MultipleSelectedResult(
    false,
    [],
  );
  optionTableTitle: string;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {
    // Not implement
  }

  confirm() {
    let result = this.options.filter(x => x.isSelected);
    result.forEach(element => {
      this.selectedResult.optionsSelected.push(new OptionSelected(element.id, element.value))
    });
    this.selectedResult.result = true;
    this.bsModalRef.hide();
  }

  decline() {
    this.selectedResult.result = false;
    this.bsModalRef.hide();
  }

  selectOption(option: SelectOption) {
    option.isSelected = !option.isSelected;
  }

}
