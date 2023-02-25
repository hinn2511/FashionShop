import { DialogService } from 'src/app/_services/dialog.service';
import { RoleService } from './../../_services/role.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Permission, Role, UpdateRolePermissionsRequest } from 'src/app/_models/user';
import { SelectOption } from 'src/app/_models/dialog';
import { concatMap } from 'rxjs/operators';

@Component({
  selector: 'app-admin-role-detail',
  templateUrl: './admin-role-detail.component.html',
  styleUrls: ['./admin-role-detail.component.css'],
})
export class AdminRoleDetailComponent implements OnInit {
  role: Role;
  permissions: Permission[] = [];
  rolePermissions: Permission[] = [];

  roleName: string = "";

  isEditMode: boolean = false;

  constructor(
    private dialogService: DialogService,
    private roleService: RoleService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    const roleId = this.route.snapshot.paramMap.get('id');
    this.loadRoleDetail(+roleId);
    this.loadRolePermissions(+roleId);
  }

  loadRoleDetail(roleId: number) {
    this.roleService.getRoleDetail(roleId).subscribe((result) => {
      this.role = result;
      this.roleName = this.role.roleName;
    });
  }

  loadRolePermissions(roleId: number) {
    this.roleService.getRolePermissions(roleId).subscribe((result) => {
      this.permissions = result;
      this.rolePermissions = result.filter((x) => x.isAllowed);
    });
  }

  deleteRole() {
    this.roleService.deleteRole(this.role.id).subscribe(
      (result) => {
        this.toastr.success(result.message, 'Success');
        this.router.navigateByUrl('/administrator/role-manager');
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  openCreatePermissionDialog() {
    let permissionGroupNames = [...new Set(this.permissions.map(item => item.permissionGroup))].filter(x => x != undefined && x != '');
    this.dialogService.openCreatePermissionDialog(permissionGroupNames).subscribe(() => {
      this.loadRolePermissions(this.role.id);
    });
  }

  openEditPermissionDialog() {
    let options: SelectOption[] = [];
    this.permissions.forEach((element) => {
      options.push(new SelectOption(element.id, element.permissionName, element.isAllowed));
    });

    let showMessage = true;

    this.dialogService
      .openMultipleSelectDialog(
        options,
        'Permission',
        'Edit permission',
        '',
        'OK',
        'Cancel'
      )
      .pipe(
        concatMap((selectedResult) => {
          if (selectedResult.result)
            return this.roleService.updateRolePermissions(
              this.role.id,
              new UpdateRolePermissionsRequest(selectedResult.optionsSelected.map(x => x.id))
            );
          else  
              showMessage = false;
        })
      )
      .subscribe(
        (result) => {
          this.toastr.success(result.message, 'Success');
          this.loadRoleDetail(this.role.id);
          this.loadRolePermissions(this.role.id);
        },
        (error) => {
          if(showMessage)
            this.toastr.error(error, 'Error');
        }
      );
  }

  editModeToggle()
  {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode)
      this.roleName = this.role.roleName;
  }
}
