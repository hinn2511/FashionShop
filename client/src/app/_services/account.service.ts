import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Product } from '../_models/product';
import { ProductParams } from '../_models/productParams';
import { Account } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;

  favoriteCache = new Map();
  productParams: ProductParams;

  constructor(private http: HttpClient) {
    this.productParams = new ProductParams();
  }

  getProductParams() {
    return this.productParams;
  }

  setProductParams(params: ProductParams) {
    this.productParams = params;
  }

  resetProductParams() {
    this.productParams = new ProductParams();
    return this.productParams;
  }

  getFavorites(productParams: ProductParams) {
    let response = this.favoriteCache.get(
      Object.values(productParams).join('-')
    );
    if (response) {
      return of(response);
    }
    let params = getPaginationHeaders(
      productParams.pageNumber,
      productParams.pageSize
    );
    params = params.append('orderBy', productParams.orderBy);
    params = params.append('field', productParams.field);
    return getPaginatedResult<Product[]>(
      this.baseUrl + 'user/favorite',
      params,
      this.http
    ).pipe(
      map((response) => {
        this.favoriteCache.set(Object.values(productParams).join('-'), response);
        return response;
      })
    );
  }

  clearFavoriteCache()
  {
    this.favoriteCache.clear();
  }

  getAccountInformation() {
    return this.http.get<Account>(this.baseUrl + 'user');
  }

  updateAccountInformation(accountUpdated: Account) {
    return this.http.put(this.baseUrl + 'user', accountUpdated);
  }

  addToFavorite(productId: number) {
    return this.http.post(this.baseUrl + 'user/favorite/' + productId, {});
  }

  removeFromFavorite(productId: number) {
    return this.http.delete(this.baseUrl + 'user/favorite/' + productId);
  }
}
