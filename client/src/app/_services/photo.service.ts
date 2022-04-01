import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
  baseUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }

  setMainPhoto(productId: number, photoId: number) {
    return this.http.put(this.baseUrl + 'product/set-main-product-photo/' + productId + '/' + photoId, {});
  }

  deletePhoto(productId: number, photoId: number) {
    return this.http.delete(this.baseUrl + 'product/delete-product-photo/' + productId + '/' + photoId);
  }
}
