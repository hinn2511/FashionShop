import { AuthenticationService } from './authentication.service';
import {
  ManagerCarousel,
  ManagerCarouselParams,
} from 'src/app/_models/carousel';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  HttpClient,
  HttpHeaders,
} from '@angular/common/http';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';
import { IdArray } from '../_models/adminRequest';

@Injectable({
  providedIn: 'root',
})
export class ContentService {
  baseUrl = environment.apiUrl;
  carousels: ManagerCarousel[] = [];
  managerCarouselParams: ManagerCarouselParams;

  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService
  ) {
    this.managerCarouselParams = new ManagerCarouselParams();
  }


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
