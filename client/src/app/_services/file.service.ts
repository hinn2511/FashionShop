import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  HttpClient,
  HttpHeaders,
} from '@angular/common/http';
import { FileUploadedResponse } from 'src/app/_models/file';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  baseUrl = environment.apiUrl;

  options = {
    headers: new HttpHeaders({
      authority: '/',
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

  uploadImage(image: File, width: number, height: number, ratio:number, cropOption: string, accessToken: string) {
    let uploadForm = new FormData();
    uploadForm.append('file', image, image.name);
    this.addToken(accessToken);
    return this.http.post<FileUploadedResponse>(
      this.baseUrl + `file/image?width=${width}&height=${height}&ratio=${ratio}&cropOption=${cropOption}`,
      uploadForm,
      this.options
    );
  }

  private addToken(accessToken: string) {
    this.options.headers = this.options.headers.set('Authorization', 'Bearer ' + accessToken);
  }
}
