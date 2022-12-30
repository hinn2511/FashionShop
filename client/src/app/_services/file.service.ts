import { AuthenticationService } from './authentication.service';
import {
  ManagerCarousel,
  ManagerCarouselParams,
} from 'src/app/_models/carousel';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  HttpClient,
  HttpErrorResponse,
  HttpEventType,
  HttpHeaders,
} from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';
import { IdArray } from '../_models/adminRequest';
import { map, switchMap } from 'rxjs/operators';
import { FileUploader } from 'ng2-file-upload';
import { FileUploadedResponse } from 'src/app/_models/file';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  fileUrl = environment.fileUrl;
  options = {
    headers: new HttpHeaders({
      authority: 'localhost:5001',
      method: 'POST',
      scheme: 'https',
      Accept: '*/*',
      'Accept-Language': 'vi,en-US;q=0.9,en;q=0.8',
    }),
  };

  constructor(private http: HttpClient) {}

  uploadFile(file: File, url: string, accessToken: string) {
    let uploadForm = new FormData();
    uploadForm.append('file', file, file.name);
    this.addToken(accessToken);
    return this.http.post<FileUploadedResponse>(url, uploadForm, this.options);
  }

  uploadMultipleFile(files: File[], url: string, accessToken: string) {
    let uploadForm = new FormData();
    for(let file of files)
    {
      uploadForm.append('file', file, file.name);
    }
    this.addToken(accessToken);
    return this.http.post<FileUploadedResponse>(url, uploadForm, this.options);
  }

  uploadImage(image: File, width: number, height: number, accessToken: string) {
    let uploadForm = new FormData();
    uploadForm.append('file', image, image.name);
    this.addToken(accessToken);
    return this.http.post<FileUploadedResponse>(
      this.fileUrl + '/image?width=' + width + '&height=' + height,
      uploadForm,
      this.options
    );
  }

  private addToken(accessToken: string) {
    this.options.headers = this.options.headers.set('Authorization', 'Bearer ' + accessToken);
  }
}
