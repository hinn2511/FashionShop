import { RoleService } from './../../_services/role.service';
import { Role, UserDetailResponse } from './../../_models/user';
import { Component, OnInit } from '@angular/core';
import { IdArray } from 'src/app/_models/adminRequest';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import {
  fnGetObjectStateString,
  fnGetObjectStateStyle,
} from 'src/app/_common/function/style-class';
import { FileService } from 'src/app/_services/file.service';
import { UserService } from 'src/app/_services/user.service';
import { concatMap } from 'rxjs/operators';
import { SelectOption } from 'src/app/_models/dialog';
import { DialogService } from 'src/app/_services/dialog.service';

@Component({
  selector: 'app-admin-user-detail',
  templateUrl: './admin-user-detail.component.html',
  styleUrls: ['./admin-user-detail.component.css'],
})
export class AdminUserDetailComponent implements OnInit {
  id: IdArray;
  user: UserDetailResponse;

  roles: Role[] = [];

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,
    private roleService: RoleService,
    private dialogService: DialogService
  ) {}

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id');
    this.id = {
      ids: [+userId],
    };
    this.loadUserDetail(+userId);
    this.loadRoles();
  }

  loadUserDetail(userId: number) {
    this.userService.getUserDetail(userId).subscribe((result) => {
      this.user = result;
    });
  }

  getUserStateStyle() {
    return fnGetObjectStateStyle(this.user.status);
  }

  editUser() {
    this.router.navigateByUrl(
      '/administrator/user-manager/edit/' + this.id.ids[0]
    );
  }

  // hideUser() {
  //   this.userService.hideCategories(this.id).subscribe((result) => {
  //     this.loadUserDetail(this.id.ids[0]);
  //     this.toastr.success('User have been hidden or unhidden', 'Success');
  //   },
  //   error =>
  //   {
  //     this.toastr.error("Something wrong happen!", 'Error');
  //   });
  // }

  deleteUser() {
    this.userService.deleteUsers(this.id.ids).subscribe(
      (result) => {
        this.loadUserDetail(this.id.ids[0]);
        this.toastr.success('User have been deleted', 'Success');
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  editRole() {
    let options: SelectOption[] = [];
    this.roles.forEach((element) => {
      options.push(
        new SelectOption(
          element.id,
          element.roleName,
          element.roleName == this.user.role
        )
      );
    });

    let showMessage = true;

    this.dialogService
      .openSingleSelectDialog(
        options,
        'Role',
        'Select new role',
        '',
        'OK',
        'Cancel'
      )
      .pipe(
        concatMap((selectedResult) => {
          if (selectedResult.result)
            return this.userService.setUsersRole(
              this.id,
              selectedResult.selectedId
            );
          else showMessage = false;
        })
      )
      .subscribe(
        (result) => {
          this.toastr.success(result.message, 'Success');
          this.loadUserDetail(this.user.id);
          this.loadRoles();
        },
        (error) => {
          if (showMessage) this.toastr.error(error, 'Error');
        }
      );
  }

  deactivateUser() {
    this.userService.deactivateUsers(this.id).subscribe(
      (result) => {
        this.loadUserDetail(this.user.id);
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  activateUser() {
    this.userService.activateUsers(this.id).subscribe(
      (result) => {
        this.loadUserDetail(this.user.id);
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  loadRoles() {
    this.roleService.getAllRoles(false).subscribe((result) => {
      this.roles = result;
    });
  }

  changePassword() {
    this.dialogService
      .openChangePasswordDialog(this.user.id)
      .subscribe((_) => {});
  }
}
