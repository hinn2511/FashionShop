import { ToastrService } from 'ngx-toastr';
import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Account } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-account-information',
  templateUrl: './account-information.component.html',
  styleUrls: ['./account-information.component.css']
})
export class AccountInformationComponent implements OnInit {
  @Input() account: Account;
  accountInformationForm: FormGroup;
  isEditMode: boolean;

  constructor(private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.isEditMode = false;
    this.initializeForm();
    this.accountInformationForm.disable();
    this.loadAccountInformation();
  }

  initializeForm() {
    this.accountInformationForm = this.fb.group ({
      firstName: [{value: ''},Validators.required],
      lastName: [{value: ''},Validators.required],
      dateOfBirth: [{value: new Date()},Validators.required],
      email: [{value: ''},Validators.required],
      phoneNumber: [{value: ''},Validators.required],
      gender: [{value: ''}, [Validators.required]]
    })
  }


  loadAccountInformation()
  {
    this.accountService.getAccountInformation().subscribe(result =>
      {
        this.account = result;
        this.setFormValue(result);
      });
  }

  setFormValue(result: Account) {    
    this.accountInformationForm.patchValue({
      firstName: result.firstName,
      lastName: result.lastName,
      dateOfBirth: new Date(result.dateOfBirth),
      email: result.email,
      phoneNumber: result.phoneNumber,
      gender: result.gender
    });    
  }

  editModeToggle()
  {
    this.isEditMode = !this.isEditMode;
    this.isEditMode ? this.accountInformationForm.enable() : this.accountInformationForm.disable();
  }

  cancelEdit()
  {
    if(this.isEditMode)
      this.setFormValue(this.account);
    this.editModeToggle();
  }

  updateAccountInformation()
  {    
    this.accountService.updateAccountInformation(this.accountInformationForm.value).subscribe(result => 
      {
        this.editModeToggle();
        this.toastr.success("Your account information has been updated", "Success");
      },
      error => {
        this.toastr.error('Something wrong happen!', 'Error');
      });
  }

  getDefaultDate(date: Date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
  }
}
