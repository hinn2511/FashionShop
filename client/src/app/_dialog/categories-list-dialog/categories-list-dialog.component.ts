import { Gender } from 'src/app/_models/category';
import { ManagerCatalogue } from './../../_models/category';
import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { fnGetArrayDepth } from 'src/app/_common/function/function';
import { SingleSelectedResult } from 'src/app/_models/dialog';

@Component({
  selector: 'app-categories-list-dialog',
  templateUrl: './categories-list-dialog.component.html',
  styleUrls: ['./categories-list-dialog.component.css']
})
export class CategoriesListDialogComponent implements OnInit {
  catalogues: ManagerCatalogue[];
  title: string;
  btnYesText: string;
  btnNoText: string;
  selectedResult: SingleSelectedResult = new SingleSelectedResult(false, 0, "");
  includeGender: string[] = [];
  showNoneOption: boolean;
  selectSubCategory: boolean;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    
    if(this.includeGender.length > 0)
      this.catalogues = this.catalogues.filter(x => this.includeGender.indexOf(x.genderTitle) > -1);
  }

  confirm() {
    this.selectedResult.result = true;
    this.bsModalRef.hide();
  }

  decline() {
    this.selectedResult.result = false;
    this.bsModalRef.hide();
  }

  select(id: number, name: string, gender: string)
  {
    this.selectedResult.selectedId = id;
    this.selectedResult.selectedValue = gender + " - " + name;
  }

  removeSelected()
  {
    this.selectedResult.selectedId = 0;
    this.selectedResult.selectedValue = "None";
  }

}
