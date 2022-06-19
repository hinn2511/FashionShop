import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject ,BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
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

  private clickedImage = new BehaviorSubject<string>(null);

  changeEmitted$ = this.clickedImage.asObservable();
  
  emitChange(change: string) {
      this.clickedImage.next(change);
  }

  getCurrentPhoto() {
    return this.changeEmitted$;
  }

  clearChange() {
    this.clickedImage.next(null);
}
}
