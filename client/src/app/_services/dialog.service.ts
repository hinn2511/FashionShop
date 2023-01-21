import { ManagerCatalogue } from './../_models/category';
import { CategoriesListDialogComponent } from './../_dialog/categories-list-dialog/categories-list-dialog.component';
import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../_common/confirm-dialog/confirm-dialog.component';
import { ConfirmResult, SingleSelectedResult } from '../_models/dialog';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  bsModalRef: BsModalRef;

  constructor(private modalService: BsModalService) { }

  openConfirmDialog(title = 'Confirmation',
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
    return new Observable<ConfirmResult>(this.getConfirmResult());
  }

  private getConfirmResult() {
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

  openCategorySingleSelectorDialog(catalogues: ManagerCatalogue[], showNoneOption: boolean = false, includeGender: string[] = [], title = 'Category',
    btnYesText = 'OK', btnNoText = 'Cancel'): Observable<SingleSelectedResult> {
      const config = {
        initialState: {
          title,
          btnYesText,
          btnNoText,
          catalogues,
          includeGender,
          showNoneOption
        }
      }
    this.bsModalRef = this.modalService.show(CategoriesListDialogComponent, config);
    return new Observable<SingleSelectedResult>(this.getCategorySingleSelectorResult());
  }

  private getCategorySingleSelectorResult() {
    return (observer) => {
      const subscription = this.bsModalRef.onHidden.subscribe(() =>{
        observer.next(this.bsModalRef.content.selectResult);
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
