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
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { IdArray } from '../_models/adminRequest';
import { map, switchMap } from 'rxjs/operators';
import { FileUploader } from 'ng2-file-upload';
import { FileUploadedResponse } from 'src/app/_models/file';

@Injectable({
  providedIn: 'root',
})
export class ContentService {
  baseUrl = environment.apiUrl;
  fileUrl = environment.fileUrl;
  carousels: ManagerCarousel[] = [];
  managerCarouselParams: ManagerCarouselParams;

  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService
  ) {
    this.managerCarouselParams = new ManagerCarouselParams();
  }

  // uploadImage(file: File, accessToken: string) {
  //   let height = 600;
  //   let width = 1200;
  //   let uploadForm = new FormData();
  //   uploadForm.append('file', file, file.name);
  //   const options = {
  //     headers: new HttpHeaders({
  //       authority: 'localhost:5001',
  //       method: 'POST',
  //       scheme: 'https',
  //       Accept: '*/*',
  //       'Accept-Language': 'vi,en-US;q=0.9,en;q=0.8',
  //       Authorization: 'Bearer ' + accessToken,
  //     }),
  //   };
  //   return this.http.post<FileUploadedResponse>(
  //     this.fileUrl + '/image?width=1200&height=600',
  //     uploadForm,
  //     options
  //   );
  // }

  getManagerCarouselParams() {
    return this.managerCarouselParams;
  }

  setManagerCarouselParams(params: ManagerCarouselParams) {
    this.managerCarouselParams = params;
  }

  resetManagerCarouselParams() {
    this.managerCarouselParams = new ManagerCarouselParams();
    return this.managerCarouselParams;
  }

  getManagerCarousels(carouselParams: ManagerCarouselParams) {
    let params = getPaginationHeaders(
      carouselParams.pageNumber,
      carouselParams.pageSize
    );
    params = params.append('orderBy', carouselParams.orderBy);
    params = params.append('field', carouselParams.field);
    params = params.append('query', carouselParams.query);
    carouselParams.carouselStatus.forEach((element) => {
      params = params.append('carouselStatus', element);
    });

    return getPaginatedResult<ManagerCarousel[]>(
      this.baseUrl + 'configuration/carousels',
      params,
      this.http
    );
  }

  getManagerCarousel(id: number) {
    return this.http.get<ManagerCarousel>(
      this.baseUrl + 'configuration/carousel/detail/' + id
    );
  }

  addCarousel(imageUrl: string, carousel: ManagerCarousel) {
    carousel.imageUrl = imageUrl;
    return this.http.post<ManagerCarousel>(
      this.baseUrl + 'configuration/create-carousel',
      carousel
    );
  }

  editCarousel(id: number, carousel: ManagerCarousel) {
    return this.http.put<ManagerCarousel>(
      this.baseUrl + 'configuration/edit-carousel/' + id,
      carousel
    );
  }

  hideCarousels(ids: IdArray) {
    return this.http.put(
      this.baseUrl + 'configuration/hide-or-unhide-carousel',
      ids
    );
  }

  deleteCarousel(ids: number[]) {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: {
        ids,
      },
    };
    return this.http.delete(
      this.baseUrl + 'configuration/soft-delete-carousel',
      options
    );
  }

  getCustomerCarousels() {
    return this.http.get<ManagerCarousel[]>(this.baseUrl + 'content/carousels');
  }
}
