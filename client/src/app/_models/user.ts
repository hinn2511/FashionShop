
import { GenericStatus } from './generic';
import { Params } from './params';
export class User {
  id: number;
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  jwtToken?: string;
  roles: string[];
  constructor()
  {
    this.id = 0,
    this.username = '',
    this.password = '',
    this.firstName = '',
    this.lastName = '',
    this.roles = []
  }
}

export class Account {
  dateOfBirth: Date;
  firstName: string;
  lastName: string;
  gender: string;
  phoneNumber: string;
  email: string;
}

export class RegisterAccount extends Account {
  username: string;
  password: string;
}

export class ChangeUserPasswordRequest {
  password: string;
  constructor(password: string)
  {
    this.password = password;
  }
}

export class CreateSystemAccountRequest extends RegisterAccount
{
    roleName: string;
}

export interface CreateRoleRequest {
  roleName: string;
  permissions: string[];
}

export interface CreatePermissionRequest {
  permissionName: string;
}

export class UpdateRolePermissionsRequest {
  permissionIds: number[];

  constructor(permissionIds: number[])
  {
    this.permissionIds = permissionIds;
  }
}



export class UserResponse {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  status: number;
  statusString: string;
  role: string;
}

export class UserDetailResponse extends UserResponse {
  dateOfBirth: Date;
  gender: string;
  phoneNumber: string;
  email: string;
  totalAmount: number;
  totalOrder: number;
}


export class UserParams extends Params {
  roleId = 0;
  userStatus = [0];
}

export class Role {
  id: number;
  roleName: string;
  totalUser: number;

  constructor(id: number, roleName: string,
    totalUser: number)
    {
      this.id = id;
      this.roleName = roleName;
      this.totalUser = totalUser;
    }
}

export class Permission {
  id: number;
  permissionName: string;
  permissionGroup: string;
  isAllowed: boolean;
}


export class UserStatus extends GenericStatus {
}

export const UserStatusList: UserStatus[] = [
  new GenericStatus(0, 'Active'),
  new GenericStatus(1, 'Deactivated')
];
