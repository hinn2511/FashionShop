import { ResponseMessage } from './../_models/generic';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Setting } from '../_models/setting';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class SettingService {
  baseUrl = environment.apiUrl;
  private settingSubject = new BehaviorSubject<Setting>({
    clientLoginBackground: '',
    clientLoginPhotoId: 0,
    clientRegisterBackground: '',
    clientRegisterPhotoId: 0,
    administratorLoginBackground: '',
    administratorLoginPhotoId: 0,
  });

  constructor(private http: HttpClient) {}

  public setSetting(setting: Setting) {
    this.settingSubject.next(setting);
  }

  public get settingValue(): Setting {
    return this.settingSubject.getValue();
  }

  getSettings() {
    return this.http.get<Setting>(
      this.baseUrl + 'configuration/client-settings'
    ).pipe(
      map((response) => {
        if (response) {
          this.settingSubject.next(response);
          return response;
        }
      })
    );
  }

  updateBackground(photoId: number, type: string) {
    
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'configuration/background/' + photoId + '?type=' + type,
      {}
    );
  }

  updateCustomerRegisterBackground(photoId: number) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'configuration/customer-register-background/' + photoId,
      {}
    );
  }

  updateAdministratorLoginBackground(photoId: number) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'configuration/administrator-login-background/' + photoId,
      {}
    );
  }
}
