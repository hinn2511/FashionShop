import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IdArray } from 'src/app/_models/adminRequest';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;
  // managerOrderParams: ManagerOrderParams;

  constructor(private http: HttpClient) {
    // this.managerOrderParams = new ManagerOrderParams();
  }

  public getSelectedOrderId(): number {
    return +(localStorage.getItem("selectedOrderId"));
  }

  public setSelectedOrderId(value: number) {
    localStorage.setItem("selectedOrderId", value.toString())
  }

  public removeSelectedOrderId() {
    localStorage.removeItem("selectedOrderId")
  }

  // getOrderParams() {
  //   return this.managerOrderParams;
  // }

  // setOrderParams(params: ManagerOrderParams) {
  //   this.managerOrderParams = params;
  // }

  // resetOrderParams() {
  //   this.managerOrderParams = new ManagerOrderParams();
  //   return this.managerOrderParams;
  // }


  // getManagerOrders(optionParams: ManagerOrderParams) {
  //   let params = getPaginationHeaders(optionParams.pageNumber, optionParams.pageSize);
  //   params = params.append('orderBy', optionParams.orderBy);
  //   params = params.append('field', optionParams.field);
  //   params = params.append('query', optionParams.query);
  //   optionParams.productOrderStatus.forEach(element => {
  //     params = params.append('productOrderStatus', element);
  //   });

  //   return getPaginatedResult<ManagerOrder[]>(this.baseUrl + 'productOrder/all', params, this.http);
  // }

  // getManagerOrder(id: number) {
  //   return this.http.get<ManagerOrder>(this.baseUrl + 'productOrder/' + id + '/detail');
  // }

  // addOrder(option: CreateOrder) {
  //   return this.http.post<CreateOrder>(this.baseUrl + 'productOrder/create', option);
  // }

  // editOrder(id: number, option: UpdateOrder) {
  //   return this.http.put<UpdateOrder>(this.baseUrl + 'productOrder/edit/' + id, option);
  // }

  // hideOrders(ids: IdArray) {
  //   return this.http.put(this.baseUrl + 'productOrder/hide-or-unhide', ids);
  // }

  // deleteOrder(ids: number[]) {
  //   const options = {
  //     headers: new HttpHeaders({
  //       'Content-Type': 'application/json',
  //     }),
  //     body: {
  //       ids
  //     },
  //   };
  //   return this.http.delete(this.baseUrl + 'productOrder/soft-delete', options);
  // }
  
  // getManagerColorOrder() {
  //   return this.http.get<ManagerOrderColor[]>(this.baseUrl + 'color/all');
  // }

  // getManagerSizeOrder() {
  //   return this.http.get<ManagerOrderSize[]>(this.baseUrl + 'size/all');
  // }

  // getCustomerProductOrder(productId: number) {
  //   return this.http.get<CustomerOrder[]>(this.baseUrl + 'productOrder/' + productId);
  // }

}
