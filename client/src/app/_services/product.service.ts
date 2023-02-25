import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IdArray } from 'src/app/_models/adminRequest';
import { AddProduct, Brand, UpdateProduct, Product, ManagerProduct } from 'src/app/_models/product';
import { CustomerColorFilter, ManagerProductParams, ProductParams } from 'src/app/_models/productParams';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';
import { ResponseMessage } from '../_models/generic';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = environment.apiUrl;
  recentProducts: Product[] = [];

  productCache = new Map();
  productParams: ProductParams;
  managerProductParams: ManagerProductParams;

  constructor(private http: HttpClient) {
    this.productParams = new ProductParams();
    this.managerProductParams = new ManagerProductParams();
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

  public getSelectedProductId(): number {
    return +(localStorage.getItem("selectedProductId"));
  }

  public setSelectedProductId(value: number) {
    localStorage.setItem("selectedProductId", value.toString())
  }

  public removeSelectedProductId() {
    localStorage.removeItem("selectedProductId")
  }

  getColorFilter(productParams: ProductParams)
  {
    let params = getPaginationHeaders(productParams.pageNumber, productParams.pageSize);
    params = params.append('category', productParams.category);
    params = params.append('gender', productParams.gender);
    params = params.append('orderBy', productParams.orderBy);
    params = params.append('size', productParams.size);
    params = params.append('field', productParams.field);
    params = params.append('query', productParams.query);
    params = params.append('minPrice', productParams.minPrice);
    params = params.append('maxPrice', productParams.maxPrice);

    return this.http.get<CustomerColorFilter[]>(this.baseUrl + 'filter/color', { params: params})
  }

  addToRecent(product: Product)
  {
    if(this.recentProducts.length >= 5)
    {
      this.recentProducts = this.recentProducts.splice(this.recentProducts.length - 1, 1);
    }
    this.recentProducts.unshift(product);
  }

  getRecent()
  {
    return this.recentProducts;
  }

  getProducts(productParams: ProductParams) {
    let response = this.productCache.get(Object.values(productParams).join('-'));
    if (response) {
      return of(response);
    }
    let params = getPaginationHeaders(productParams.pageNumber, productParams.pageSize);
    params = params.append('category', productParams.category);
    params = params.append('gender', productParams.gender);
    params = params.append('orderBy', productParams.orderBy);
    params = params.append('colorCode', productParams.colorCode);
    params = params.append('size', productParams.size);
    params = params.append('field', productParams.field);
    params = params.append('query', productParams.query);
    params = params.append('minPrice', productParams.minPrice);
    params = params.append('maxPrice', productParams.maxPrice);
    return getPaginatedResult<Product[]>(this.baseUrl + 'product', params, this.http).pipe(
      map(response => {
        this.productCache.set(Object.values(productParams).join('-'), response);
        return response;
      })
    );
  }
  
  getProduct(id: number) {
    return this.http.get<Product>(this.baseUrl + 'product/' + id);
  }

  removeProductCache(id: number) {
    for (let key of this.productCache.keys()) {
      if (this.productCache.get(key).result.find((product: Product) => product.id === id))
        this.productCache.delete(key);
    }
  }

  removeCache() {
    this.productCache.clear();
  }

  getManagerProductParams() {
    return this.managerProductParams;
  }

  setManagerProductParams(params: ManagerProductParams) {
    this.managerProductParams = params;
  }

  resetManagerProductParams() {
    this.managerProductParams = new ManagerProductParams();
    return this.managerProductParams;
  }

  getManagerProducts(productParams: ManagerProductParams) {
    let params = getPaginationHeaders(productParams.pageNumber, productParams.pageSize);
    params = params.append('category', productParams.category);
    params = params.append('gender', productParams.gender);
    params = params.append('orderBy', productParams.orderBy);
    params = params.append('field', productParams.field);
    params = params.append('query', productParams.query);
    productParams.productStatus.forEach(element => {
      params = params.append('productStatus', element);
    });

    return getPaginatedResult<ManagerProduct[]>(this.baseUrl + 'product/all', params, this.http);
  }

  getManagerProduct(id: number) {
    return this.http.get<ManagerProduct>(this.baseUrl + 'product/detail/' + id);
  }

  addProduct(product: AddProduct) {
    return this.http.post<AddProduct>(this.baseUrl + 'product/create', product);
  }

  editProduct(id: number, product: UpdateProduct) {
    return this.http.put<UpdateProduct>(this.baseUrl + 'product/edit/' + id, product);
  }

  hideProducts(ids: IdArray) {
    return this.http.put<ResponseMessage>(this.baseUrl + 'product/hide', ids);
  }

  activateProducts(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'product/activate',
      ids
    );
  }

  promoteProducts(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'product/promote',
      ids
    );
  }

  demoteProducts(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'product/demote',
      ids
    );
  }

  deleteProduct(ids: number[]) {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: {
        ids
      },
    };
    return this.http.delete(this.baseUrl + 'product/soft-delete', options);
  }

  getBrands() {
    return this.http.get<Brand[]>(this.baseUrl + 'brand');
  }

  hideProductImage(ids: IdArray) {
    return this.http.put(this.baseUrl + 'product/hide-product-photo', ids);
  }

  unhideProductImage(ids: IdArray) {
    return this.http.put(this.baseUrl + 'product/unhide-product-photo', ids);
  }

  deleteProductImage(ids: number[]) {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: {
        ids
      },
    };
    return this.http.delete(this.baseUrl + 'product/delete-product-photo', options);
  }

  setMainProductImage(productId: number, photoId: number) {
    return this.http.put(this.baseUrl + 'product/set-main-product-photo/' + productId + "/" + photoId, {});
  }

}
