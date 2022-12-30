import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from 'src/app/_common/confirm-dialog/confirm-dialog.component';
import { ConfirmResult } from 'src/app/_models/dialog';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef: BsModalRef;

  constructor(private modalService: BsModalService) { }

  confirm(title = 'Confirmation',
    message = 'Are you sure you want to do this?',
    showReasonTextBox = false,
    btnYesText = 'Yes', btnNoText = 'Cancel'): Observable<ConfirmResult> {
      const config = {
        initialState: {
          title,
          message,
          btnYesText,
          btnNoText,
          showReasonTextBox
        }
      }
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent, config);
    return new Observable<ConfirmResult>(this.getResult());
  }

  private getResult() {
    return (observer) => {
      const subscription = this.bsModalRef.onHidden.subscribe(() =>{
        observer.next(this.bsModalRef.content.confirmResult);
        observer.complete();
      });
      
      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }
}
