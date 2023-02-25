import { ToastrService } from 'ngx-toastr';
import { fnGetFormControlValue } from 'src/app/_common/function/function';
import { ChangeUserPasswordRequest } from './../../_models/user';
import { UserService } from 'src/app/_services/user.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ConfirmResult } from 'src/app/_models/dialog';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-password-dialog.component.html',
  styleUrls: ['./change-password-dialog.component.css']
})
export class ChangePasswordDialogComponent implements OnInit, OnDestroy {


  userId: number;
  changePasswordForm: FormGroup;
  confirmPasswordSubscription$: Subscription;
  selectedResult: boolean;

  constructor(public bsModalRef: BsModalRef, private userService: UserService, private toastr: ToastrService, private formBuilder: FormBuilder) { }
  ngOnDestroy(): void {
    this.confirmPasswordSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.changePasswordForm = this.formBuilder.group({
      password: ['', Validators.required],
      confirmPassword: ['', [Validators.required,
      this.matchValues('password')]],
    });
    this.confirmPasswordSubscription$ = this.changePasswordForm.controls.password.valueChanges.subscribe(() => {
      this.changePasswordForm.controls['confirmPassword'].updateValueAndValidity();
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
    }
  }

  confirm() {
    if (!this.changePasswordForm.valid)
      return;
    this.userService.changeUserPassword(this.userId, new ChangeUserPasswordRequest(fnGetFormControlValue(this.changePasswordForm, 'password'))).subscribe(result => 
      {
        this.toastr.success(result.message, "Success");
        this.bsModalRef.hide();
      },
      error => 
      {
        this.toastr.error( error, "Error");
      });
  }

  decline() {
    this.bsModalRef.hide();
  }

  

}
