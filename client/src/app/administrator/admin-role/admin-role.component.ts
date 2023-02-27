import { DialogService } from './../../_services/dialog.service';
import { ToastrService } from 'ngx-toastr';
import { RoleService } from './../../_services/role.service';
import { Component, OnInit } from '@angular/core';
import { Role } from 'src/app/_models/user';
import { Router } from '@angular/router';
import { concatMap } from 'rxjs/operators';

@Component({
  selector: 'app-admin-role',
  templateUrl: './admin-role.component.html',
  styleUrls: ['./admin-role.component.css'],
})
export class AdminRoleComponent implements OnInit {
  roles: Role[];
  constructor(
    private roleService: RoleService,
    private dialogService: DialogService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles() {
    this.roleService.getAllRoles(true).subscribe((result) => {
      this.roles = result;
    });
  }

  viewDetail(roleId: number) {
    if (roleId == 0) return;
    this.router.navigateByUrl('/administrator/role-manager/detail/' + roleId);
  }

  deleteRole(roleId: number) {
    let showMessage = true;
    this.dialogService
      .openConfirmDialog(
        'Confirm',
        'Are you sure you want to delete this role? Every one in this role will lost all permissions associate with this role',
        false
      )
      .pipe(
        concatMap((confirmResult) => {
          if (confirmResult.result) return this.roleService.deleteRole(roleId);
          else showMessage = false;
        })
      )
      .subscribe(
        (result) => {
          this.loadRoles();
          this.toastr.success(result.message, 'Success');
        },
        (error) => {
          if (showMessage) this.toastr.error(error, 'Error');
        }
      );
  }
}
