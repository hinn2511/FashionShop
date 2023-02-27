import { ChangePasswordDialogComponent } from './../_dialog/change-password-dialog/change-password-dialog.component';
import { CreatePermissionDialogComponent } from './../_dialog/create-permission-dialog/create-permission-dialog.component';
import { MultipleOptionDialogComponent } from './../_dialog/multiple-option-dialog/multiple-option-dialog.component';
import { DialogResult, MultipleSelectedResult } from 'src/app/_models/dialog';
import { SingleOptionDialogComponent } from './../_dialog/single-option-dialog/single-option-dialog.component';
import { ManagerCatalogue } from './../_models/category';
import { CategoriesListDialogComponent } from './../_dialog/categories-list-dialog/categories-list-dialog.component';
import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../_dialog/confirm-dialog/confirm-dialog.component';
import { ConfirmResult, SelectOption, SingleSelectedResult } from '../_models/dialog';
import { ImageSelectorComponent } from '../_dialog/image-selector/image-selector.component';
import { SelectPhoto } from '../_models/photo';

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

  openCategorySingleSelectorDialog(catalogues: ManagerCatalogue[], showNoneOption: boolean = false, selectSubCategory: boolean = false, includeGender: string[] = [], title = 'Category',
    btnYesText = 'OK', btnNoText = 'Cancel'): Observable<SingleSelectedResult> {
      const config = {
        initialState: {
          title,
          btnYesText,
          btnNoText,
          catalogues,
          includeGender,
          showNoneOption,
          selectSubCategory
        }
      }
    this.bsModalRef = this.modalService.show(CategoriesListDialogComponent, config);
    return new Observable<SingleSelectedResult>(this.getSelectResult());
  }

  openSingleSelectDialog(options: SelectOption[], optionTableTitle = '', title = '', message = '',
    btnYesText = 'OK', btnNoText = 'Cancel'): Observable<SingleSelectedResult> {
      const config = {
        initialState: {
          title,
          btnYesText,
          btnNoText,
          optionTableTitle,
          message, 
          options
        }
      }
    this.bsModalRef = this.modalService.show(SingleOptionDialogComponent, config);
    return new Observable<SingleSelectedResult>(this.getSelectResult());
  }

  openMultipleSelectDialog(options: SelectOption[], optionTableTitle = '', title = '', message = '',
    btnYesText = 'OK', btnNoText = 'Cancel'): Observable<MultipleSelectedResult> {
      const config = {
        initialState: {
          title,
          btnYesText,
          btnNoText,
          optionTableTitle,
          message, 
          options,
        }
      }
    this.bsModalRef = this.modalService.show(MultipleOptionDialogComponent, config);
    return new Observable<MultipleSelectedResult>(this.getSelectResult());
  }

  openChangePasswordDialog(userId: number): Observable<boolean> {
      const config = {
        initialState: {
          userId
        }
      }
    this.bsModalRef = this.modalService.show(ChangePasswordDialogComponent, config);
    return new Observable<boolean>(this.getFormDialogResult());
  }

  openCreatePermissionDialog(permissionGroups: string[]): Observable<DialogResult> {
    const config = {
      initialState: {
        permissionGroups
      }
    }
  this.bsModalRef = this.modalService.show(CreatePermissionDialogComponent, config);
  return new Observable<DialogResult>(this.getFormDialogResult());
  }

  openPhotoSelectorDialog(action = 'Photo selector',
    multiple = false,
    selectedIds: number[] = []): Observable<SelectPhoto[]> {
      const config = {
        initialState: {
          action,
          multiple,
          selectedIds
        }
      }
      this.bsModalRef = this.modalService.show(ImageSelectorComponent, config);
      this.bsModalRef.setClass('modal-lg');    
    return new Observable<SelectPhoto[]>(this.getPhotoSelectResult());
  }

  private getSelectResult() {
    return (observer) => {
      const subscription = this.bsModalRef.onHidden.subscribe(() =>{
        observer.next(this.bsModalRef.content.selectedResult);
        observer.complete();
      });
      
      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }

  private getFormDialogResult() {
    return (observer) => {
      const subscription = this.bsModalRef.onHidden.subscribe(() =>{
        observer.next(this.bsModalRef.content.result);
        observer.complete();
      });
      
      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }

  private getPhotoSelectResult() {
    return (observer) => {
      const subscription = this.bsModalRef.onHidden.subscribe(() =>{
        observer.next(this.bsModalRef.content.photos);
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
