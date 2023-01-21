import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ConfirmResult } from 'src/app/_models/dialog';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent implements OnInit {
  title: string;
  message: string;
  btnYesText: string;
  btnNoText: string;
  showReasonTextBox: boolean;

  confirmResult: ConfirmResult = new ConfirmResult(false, '');

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    //Not implemented
  }

  confirm() {
    this.confirmResult.result = true;
    this.bsModalRef.hide();
  }

  decline() {
    this.confirmResult.result = false;
    this.bsModalRef.hide();
  }

}
