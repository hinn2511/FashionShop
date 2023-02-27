import { RoleService } from './../../_services/role.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { fnUpdateFormControlNumberValue } from 'src/app/_common/function/function';
import { Permission } from 'src/app/_models/user';
import { DialogService } from 'src/app/_services/dialog.service';
@Component({
  selector: 'app-admin-role-add',
  templateUrl: './admin-role-add.component.html',
  styleUrls: ['./admin-role-add.component.css']
})
export class AdminRoleAddComponent implements OnInit {
    newRoleForm: FormGroup;
    permissions: Permission[] = [];
    selectedPermissions: Permission[] = [];
    selectedPermissionIds: number[] = [];
    validationErrors: string[] = [];

    selectAll: boolean = false;

    isExpandTable: boolean = true;
    
    constructor(
      private fb: FormBuilder,
      private userService: UserService,
      private roleService: RoleService,
      private router: Router,
      private toastr: ToastrService,
      private dialogService: DialogService,
    ) {}
  
    ngOnInit(): void {
      this.initializeForm();
      this.loadPermission();
      
    }
  
    initializeForm() {
      this.newRoleForm = this.fb.group({
        roleName: ['', Validators.required],
        permissionIds: [this.selectedPermissionIds],
      });
    }
  
    addNewRole(event) {      
      if (this.selectedPermissionIds.length == 0)
        return;        
        
      
      this.roleService.createRole(this.newRoleForm.value).subscribe(
        (response) => {
          if (event.submitter.name == 'saveAndContinue')
            this.router.navigateByUrl('/administrator/role-manager/add');
          else this.router.navigateByUrl('/administrator/role-manager');
  
          this.toastr.success(response.message, 'Success');
        },
        (error) => {
          this.toastr.error(error, 'Error');
          this.validationErrors = error;
        }
      );
    }
  
    loadPermission() {
     
      this.roleService
        .getAllPermission()
        .subscribe(result => {
          this.permissions = result;
        });
    }
  
    backToList() {
      this.dialogService
        .openConfirmDialog(
          'Confirm',
          'Are you sure to cancel? Every change will be lost.',
          false,
          'Yes',
          'No'
        )
        .subscribe((result) => {
          if (result.result) {
            this.router.navigateByUrl('/administrator/role-manager');
          }
        });
    }
  
    selectPermission(permission: Permission)
    {
      let index = this.selectedPermissions.indexOf(permission);
      if(index > -1)
        this.selectedPermissions.splice(index, 1);
      else
        this.selectedPermissions.push(permission);
      this.updateForm();
    }

    isPermissionSelected(permission: Permission)
    {
      return this.selectedPermissions.indexOf(permission) > -1;
    }

    selectAllPermissions()
    {
      if (this.selectAll)
      {
        this.selectedPermissions = [];
        this.selectAll = false;
      }
      else
      {
        this.selectedPermissions = this.permissions;
        this.selectAll = true;
      }
      this.updateForm();
    }

    updateForm()
    {
      this.selectedPermissionIds = this.selectedPermissions.map(x => x.id);
      this.newRoleForm.controls['permissionIds'].setValue(this.selectedPermissionIds, { emitEvent: false });
    }

    openCreatePermissionDialog()
    {
      let permissionGroupNames = [...new Set(this.permissions.map(item => item.permissionGroup))].filter(x => x != undefined && x != '');
      this.dialogService.openCreatePermissionDialog(permissionGroupNames).subscribe(result => {
        if(result.result)
        {
          this.loadPermission();
        }
      })
    }

    expandTableToggle()
    {
      this.isExpandTable = !this.isExpandTable;
    }
  }
  