import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';
import { Params } from '../_models/params';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
  baseUrl = environment.apiUrl;
  photoParams: Params;

  constructor(private http: HttpClient) {
    this.photoParams = new Params();
   }

   getPhotoParams() {
    return this.photoParams;
  }

  setPhotoParams(params: Params) {
    this.photoParams = params;
  }

  resetPhotoParams() {
    this.photoParams = new Params();
    return this.photoParams;
  }

  setMainPhoto(productId: number, photoId: number) {
    return this.http.put(this.baseUrl + 'product/set-main-product-photo/' + productId + '/' + photoId, {});
  }

  deletePhoto(productId: number, photoId: number) {
    return this.http.delete(this.baseUrl + 'product/delete-product-photo/' + productId + '/' + photoId);
  }

  getManagerPhotos(photoParams: Params) {
    let params = getPaginationHeaders(photoParams.pageNumber, photoParams.pageSize);
    params = params.append('orderBy', photoParams.orderBy);
    params = params.append('field', photoParams.field);
    return getPaginatedResult<Photo[]>(this.baseUrl + '/images', params, this.http);
  }
}
