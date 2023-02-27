import { fnUpdateFormControlStringValue } from 'src/app/_common/function/function';
import { ToastrService } from 'ngx-toastr';
import { RoleService } from './../../_services/role.service';
import { DialogResult } from './../../_models/dialog';
import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-create-permission-dialog',
  templateUrl: './create-permission-dialog.component.html',
  styleUrls: ['./create-permission-dialog.component.css'],
})
export class CreatePermissionDialogComponent implements OnInit {
  result: DialogResult = new DialogResult(false);
  newPermissionForm: FormGroup;
  loading = false;
  permissionGroups: string[] = [];

  constructor(
    public bsModalRef: BsModalRef,
    private formBuilder: FormBuilder,
    private roleService: RoleService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    
  }

  onSubmit() {
    this.loading = true;
    this.roleService.createPermission(this.newPermissionForm.value).subscribe(
      (result) => {
        this.toastr.success(result.message, 'Success');
        this.loading = false;
        this.result.result = true;
        this.initializeForm();
      },
      (error) => {
        this.toastr.success(error, 'Error');
      }
    );
  }

  finish() {
    this.bsModalRef.hide();
  }

  decline() {
    this.bsModalRef.hide();
  }

  initializeForm() {
    this.newPermissionForm = this.formBuilder.group({
      permissionName: ['', Validators.required],
      permissionGroup: [''],
    });
  }

  selectGroup(permissionGroup: string){
    fnUpdateFormControlStringValue(this.newPermissionForm, 'permissionGroup', permissionGroup, false);
    
  }
}
