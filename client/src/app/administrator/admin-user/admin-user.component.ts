import { RotateAnimation } from './../../_common/animation/carousel.animations';
import { RoleService } from './../../_services/role.service';
import { SelectOption } from './../../_models/dialog';
import { DialogService } from './../../_services/dialog.service';
import { Role } from 'src/app/_models/user';
import { UserParams, UserResponse, UserStatus, UserStatusList } from './../../_models/user';
import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { fnGetObjectStateString, fnGetUserStateStyle } from 'src/app/_common/function/style-class';
import { IdArray } from 'src/app/_models/adminRequest';
import { Pagination } from 'src/app/_models/pagination';
import { concatMap } from 'rxjs/operators';

@Component({
  selector: 'app-admin-user',
  templateUrl: './admin-user.component.html',
  styleUrls: ['./admin-user.component.css'],
  animations: [ RotateAnimation ]
})
export class AdminUserComponent implements OnInit {
    users: UserResponse[];

    roles: Role[] = [];
    selectedRole: Role;

    pagination: Pagination;
    userParams: UserParams;
    selectAllUser: boolean;
    state: string = 'default';
  
    showStatusFilter: boolean;
  
    selectedIds: number[] = [];
    query: string;  
    userStatuses: UserStatus[] = UserStatusList;
    showRoleFilter: boolean = false;

    
  
    constructor(
      private userService: UserService,
      private roleService: RoleService,
      private router: Router,
      private route: ActivatedRoute,
      private dialogService: DialogService,
      private toastr: ToastrService
    ) {
      this.userParams = this.userService.getUserParams();
    }
  
    ngOnInit(): void {
      this.userParams.field = 'Id';
      this.userParams.orderBy = 0;
      this.roles = [...this.defaultRoles(), ...this.roles];   
      const roleId = this.route.snapshot.paramMap.get('id');
      this.selectedRole = this.roles[0];
      if (roleId != undefined)
        this.userParams.roleId = +roleId;
      else
        this.userParams.roleId = -1;
      this.userParams.userStatus = [0];
      this.selectAllUser = false;
      this.showStatusFilter = false;
      this.loadUsers();
      this.loadRoles();
    }
  
    rotate() {
      this.state = (this.state === 'default' ? 'rotated' : 'default');
    }
  
    loadUsers() {
      this.userService.setUserParams(this.userParams);
  
      this.userService
        .getUsers(this.userParams)
        .subscribe((response) => {
          this.users = response.result;
          this.pagination = response.pagination;
        });
    }
    loadRoles()
    {
      this.roleService.getAllRoles(false).subscribe(result => {
        this.roles = result;
        this.roles = [...this.defaultRoles(), ...this.roles];  
        this.selectedRole = this.roles.find(x => x.id == this.userParams.roleId);
      })
    };

    defaultRoles()
    {
      let defaultRoles = [
        new Role(-1, 'All roles', 0),
        new Role(0, 'No role', 0)
      ];
      return defaultRoles;
    }
  
    resetSelectedIds() {
      this.selectedIds = [];
    }
  
    viewDetail(userId: number) {
      this.router.navigateByUrl(
        '/administrator/user-manager/detail/' + userId
      );
    }
  
    editUser() {
      if (!this.isSingleSelected()) return;
      this.router.navigateByUrl(
        `/administrator/user-manager/edit/${this.selectedIds[0]}`
      );
    }
  
    deactivateUsers() {
      if (!this.isMultipleSelected()) return;
      let ids: IdArray = {
        ids: this.selectedIds,
      };
  
      this.userService.deactivateUsers(ids).subscribe((result) => {
        this.loadUsers();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      }, 
      error => 
      {
        this.toastr.error(error, 'Error');
      });
    }
  
    activateUsers()
    {
      if (!this.isMultipleSelected()) return;
      let ids: IdArray = {
        ids: this.selectedIds,
      };
  
      this.userService.activateUsers(ids).subscribe((result) => {
        this.loadUsers();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      }, 
      error => 
      {
        this.toastr.error(error, 'Error');
      });
    }
    
    deleteUsers() {
      if (!this.isMultipleSelected()) return;
      this.userService.deleteUsers(this.selectedIds).subscribe((result) => {
        this.loadUsers();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      }, 
      error => 
      {
        this.toastr.error(error, 'Error');
      });
    }
  
    pageChanged(event: any) {
      if (this.userParams.pageNumber !== event.page) {
        this.userParams.pageNumber = event.page;
        this.userService.setUserParams(this.userParams);
        this.loadUsers();
      }
    }
  
    sort(type: number) {
      this.userParams.orderBy = type;
      this.loadUsers();
    }
  
    filter(params: UserParams) {
      this.userParams = params;
      this.loadUsers();
    }
  
    resetFilter() {
      this.userParams = this.userService.resetUserParams();
      this.loadUsers();
    }
  
    orderBy(field: string) {
      this.userParams.field = field;
      if (this.userParams.orderBy == 0) this.userParams.orderBy = 1;
      else this.userParams.orderBy = 0;
      this.rotate();
      this.loadUsers();
    }
  
    selectAllUsers() {
      if (this.selectAllUser) {
        this.selectedIds = [];
      } else {
        this.selectedIds = this.users.map(({ id }) => id);
      }
      this.selectAllUser = !this.selectAllUser;
    }
  
    selectUser(id: number) {
      if (this.selectedIds.includes(id)) {
        this.selectedIds.splice(this.selectedIds.indexOf(id), 1);
      } else {
        this.selectedIds.push(id);
      }
    }
  
    isUserSelected(id: number) {
      if (this.selectedIds.indexOf(id) >= 0) return true;
      return false;
    }
  
    getUserState(user: UserResponse) {
       return fnGetObjectStateString(user.status);
    }
  
    getStateStyle(user: UserResponse) {
      return fnGetUserStateStyle(user.status);
    }
  
    isAllStatusIncluded() {
      return (
        this.userParams.userStatus.length == this.userStatuses.length
      );
    }
  
    isStatusIncluded(status: number) {
      return this.userParams.userStatus.indexOf(status) > -1;
    }
  
    selectStatus(status: number) {
      if (this.isStatusIncluded(status))
        this.userParams.userStatus =
          this.userParams.userStatus.filter((x) => x !== status);
      else this.userParams.userStatus.push(status);
      this.userParams.userStatus = [...this.userParams.userStatus].sort((a, b) => a - b);
      this.loadUsers();
    }
  
    selectAllUserStatus() {
      if(this.userParams.userStatus.length == 2)
        this.userParams.userStatus = [];
      else
        this.userParams.userStatus = [0, 1];
      this.loadUsers();
    }
  
    statusFilterToggle() {
      this.showStatusFilter = !this.showStatusFilter;
    }
  
    isSingleSelected() {
      return this.selectedIds.length == 1;
    }
  
    isMultipleSelected() {
      return this.selectedIds.length >= 1;
    }

    roleFilterToggle() {
      this.showRoleFilter = !this.showRoleFilter;
    }
  
    selectRole(role: Role) {      
      if (this.userParams.roleId == role.id)
        return;
      this.userParams.roleId = role.id;      
      this.selectedRole = role;
      this.loadUsers();
    }
    

    editRole()
    {
      if (!this.isMultipleSelected()) return;
      let options: SelectOption[] = [];
      this.roles.forEach(element => {
        options.push(new SelectOption(element.id, element.roleName, false));
      });
      options = options.slice(2);

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
            return this.userService.setUsersRole(new IdArray(this.selectedIds), selectedResult.selectedId);
        })
      )
      .subscribe(
        (result ) => {
          this.toastr.success(result.message, 'Success');
          this.loadUsers();
          this.loadRoles();
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
    }
  
    
  }