import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { AddProduct, EditProduct, Product } from '../_models/product';
import { ProductParams } from '../_models/productParams';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = environment.apiUrl;
  products: Product[] = [];
  productCache = new Map();
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

  getProducts(productParams: ProductParams) {
    var response = this.productCache.get(Object.values(productParams).join('-'));
    if (response) {
      return of(response);
    }
    let params = getPaginationHeaders(productParams.pageNumber, productParams.pageSize);
    params = params.append('category', productParams.category);
    params = params.append('gender', productParams.gender);
    params = params.append('orderBy', productParams.orderBy);
    return getPaginatedResult<Product[]>(this.baseUrl + 'product', params, this.http).pipe(
      map(response => {
        this.productCache.set(Object.values(productParams).join('-'), response);
        return response;
      })
    );
  }

  getProduct(id: number) {
    const product = [...this.productCache.values()]
      .reduce((arr, elm) => arr.concat(elm.result), [])
      .find((product: Product) => product.id === id);
    if (product) {
      return of(product);
    }
    return this.http.get<Product>(this.baseUrl + 'product/' + id);
  }

  removeProductCache(id: number) {
    for (let key of this.productCache.keys()) {   
      if(this.productCache.get(key).result.find((product: Product) => product.id === id))
        this.productCache.delete(key);
    }
  }

  addProduct(product: AddProduct) {
    return this.http.post<AddProduct>(this.baseUrl + 'product/add', product);
  }

  editProduct(product: EditProduct) {
    return this.http.put<EditProduct>(this.baseUrl + 'product/edit', product);
  }

  deleteProduct(id: number) {
    return this.http.delete(this.baseUrl + 'product/delete/' + id);
  }

}
