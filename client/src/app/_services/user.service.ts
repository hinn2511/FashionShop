import { ResponseMessage } from './../_models/generic';
import { UserResponse, UserDetailResponse, ChangeUserPasswordRequest } from './../_models/user';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';
import { UserParams } from '../_models/user';
import { IdArray } from '../_models/adminRequest';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;
  userParams: UserParams;

  constructor(private http: HttpClient) {
    this.userParams = new UserParams();
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams();
    return this.userParams;
  }


  getUsers(userParams: UserParams) {
    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('orderBy', userParams.orderBy);
    params = params.append('field', userParams.field);
    params = params.append('query', userParams.query);
    params = params.append('roleId', userParams.roleId);

    return getPaginatedResult<UserResponse[]>(this.baseUrl + 'user/all', params, this.http);
  }   

  getUserDetail(userId: number) {
    return this.http.get<UserDetailResponse>(
      this.baseUrl + 'user/detail/' + userId
    );
  }

  setUsersRole(userIds: IdArray, roleId: number) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'user/set-role/' + roleId,
      userIds
    );
  }

  removeUsersRole(userIds: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'user/remove-role',
      userIds
    );
  }

  activateUsers(userIds: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'user/activate',
      userIds
    );
  }

  deactivateUsers(userIds: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'user/deactivate',
      userIds
    );
  }

  deleteUsers(userIds: number[]) {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: {
        userIds
      },
    };
    return this.http.delete<ResponseMessage>(
      this.baseUrl + 'user/delete',
      options
    );
  }

  changeUserPassword(userId: number, request: ChangeUserPasswordRequest)
  {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'user/change-user-password/' + userId,
      request
    );
  }

 
}
