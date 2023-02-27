import { CreatePermissionRequest, CreateRoleRequest, Permission, Role, UpdateRolePermissionsRequest } from 'src/app/_models/user';
import { ResponseMessage } from './../_models/generic';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  baseUrl = environment.apiUrl;


  constructor(private http: HttpClient) {
  }


  getAllRoles(showNoRole: boolean)
  {
    let params = new HttpParams();    
    params = params.append('showNoRole', showNoRole);
    return this.http.get<Role[]>(
      this.baseUrl + 'admin/view-roles', {params: params}
    );
  }
  

  getRoleDetail(roleId: number)
  {
    return this.http.get<Role>(
      this.baseUrl + 'admin/view-role-detail/' + roleId
    );
  }

  getRolePermissions(roleId: number)
  {
    return this.http.get<Permission[]>(
      this.baseUrl + 'admin/view-role-permissions/' + roleId
    );
  }

  updateRolePermissions(roleId: number, updateRequest: UpdateRolePermissionsRequest)
  {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'admin/update-role-permissions/' + roleId, updateRequest
    );
  }


  createRole(role: CreateRoleRequest)
  {
    return this.http.post<ResponseMessage>(
      this.baseUrl + 'admin/create-role', role
    );
  }


  deleteRole(roleId: number)
  {
    return this.http.delete<ResponseMessage>(
      this.baseUrl + 'admin/delete-role/' + roleId
    );
  }




  getAllPermission()
  {
    return this.http.get<Permission[]>(
      this.baseUrl + 'admin/view-permissions'
    );
  }


  
  createPermission(permission: CreatePermissionRequest)
  {
    return this.http.post<ResponseMessage>(
      this.baseUrl + 'admin/create-permission', permission
    );
  }

 
}
