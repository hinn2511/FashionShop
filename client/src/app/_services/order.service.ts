import { CancelOrderRequest, ReturnOrderRequest } from './../_models/order';
import {
  CustomerCardInformation,
  CustomerNewOrder,
  CustomerOrder,
  CustomerOrderParams,
  ManagerOrder,
  ManagerOrderParams,
  ManagerOrderSummary,
} from 'src/app/_models/order';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import {
  getPaginatedResult,
  getPaginationHeaders,
} from '../_helpers/paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  baseUrl = environment.apiUrl;
  private _selectedOrderId: string;
  orderCache = new Map();
  public get selectedOrderId(): string {
    return this._selectedOrderId;
  }
  public set selectedOrderId(value: string) {
    this._selectedOrderId = value;
  }
  customerOrderParams: CustomerOrderParams;

  managerOrderParams: ManagerOrderParams;

  constructor(private http: HttpClient) {
    this.customerOrderParams = new CustomerOrderParams();
    this.managerOrderParams = new ManagerOrderParams();
  }

  createOrder(order: CustomerNewOrder) {
    return this.http.post<string>(this.baseUrl + 'order/place-order', order);
  }

  payOrderByCard(orderId: string, cardInformation: CustomerCardInformation) {
    return this.http.post<string>(
      this.baseUrl + 'order/' + orderId + '/paid-by-card',
      cardInformation
    );
  }

  getOrderParams() {
    return this.customerOrderParams;
  }

  setOrderParams(params: CustomerOrderParams) {
    this.customerOrderParams = params;
  }

  resetOrderParams() {
    this.customerOrderParams = new CustomerOrderParams();
    return this.customerOrderParams;
  }

  getCustomerOrders(orderParams: CustomerOrderParams) {
    let params = getPaginationHeaders(
      orderParams.pageNumber,
      orderParams.pageSize
    );
    params = params.append('orderBy', orderParams.orderBy);
    params = params.append('field', orderParams.field);
    params = params.append('query', orderParams.query);
    orderParams.orderStatusFilter.forEach((element) => {
      params = params.append('orderStatusFilter', element);
    });
    params = params.append('from', orderParams.from.toISOString());
    params = params.append('to', orderParams.to.toISOString());

    return getPaginatedResult<CustomerOrder[]>(
      this.baseUrl + 'order',
      params,
      this.http
    ).pipe(
      map((response) => {
        this.orderCache.set(Object.values(orderParams).join('-'), response);
        return response;
      })
    );
  }

  getCustomerOrder(externalId: string) {
    const order = [...this.orderCache.values()]
      .reduce((arr, elm) => arr.concat(elm.result), [])
      .find((order: CustomerOrder) => order.externalId === externalId);
    if (order) {
      return of(order);
    }
    return this.http.get<CustomerOrder>(this.baseUrl + 'order/' + externalId);
  }

  clearCustomerOrderCache()
  {
    this.orderCache.clear();
  }

  getManagerOrderParams() {
    return this.managerOrderParams;
  }

  setManagerOrderParams(params: ManagerOrderParams) {
    this.managerOrderParams = params;
  }

  resetManagerOrderParams() {
    this.managerOrderParams = new ManagerOrderParams();
    return this.managerOrderParams;
  }

  getManagerOrders(managerOrderParams: ManagerOrderParams) {
    let params = getPaginationHeaders(
      managerOrderParams.pageNumber,
      managerOrderParams.pageSize
    );
    params = params.append('orderBy', managerOrderParams.orderBy);
    
    params = params.append('field', managerOrderParams.field);
    params = params.append('query', managerOrderParams.query);

    params = params.append('from', managerOrderParams.from.toISOString());
    params = params.append('to', managerOrderParams.to.toISOString());
    managerOrderParams.orderStatusFilter.forEach((element) => {
      params = params.append('orderStatusFilter', element);
    });

    managerOrderParams.paymentMethodFilter.forEach((element) => {
      params = params.append('paymentMethodFilter', element);
    });

    managerOrderParams.shippingMethodFilter.forEach((element) => {
      params = params.append('shippingMethodFilter', element);
    });

    return getPaginatedResult<ManagerOrder[]>(
      this.baseUrl + 'order/all',
      params,
      this.http
    );
  }

  getManagerOrderDetail(id: number) {
    return this.http.get<ManagerOrder>(
      this.baseUrl + 'order/' + id + '/detail'
    );
  }

  getManagerOrderSummary(managerOrderParams: ManagerOrderParams) {
    let params = new HttpParams();
    
    params = params.append('query', managerOrderParams.query);
    params = params.append('from', managerOrderParams.from.toISOString());
    params = params.append('to', managerOrderParams.to.toISOString());

    managerOrderParams.paymentMethodFilter.forEach((element) => {
      params = params.append('paymentMethodFilter', element);
    });

    managerOrderParams.shippingMethodFilter.forEach((element) => {
      params = params.append('shippingMethodFilter', element);
    });

    return this.http.get<ManagerOrderSummary[]>(this.baseUrl + 'order/summary', {params: params} );
  }

  verifyOrder(id: number) {
    return this.http.put(this.baseUrl + 'order/' + id + '/verify', {});
  }

  shippingOrder(id: number) {
    return this.http.put(this.baseUrl + 'order/' + id + '/shipping', {});
  }

  shippedOrder(id: number) {
    return this.http.put(this.baseUrl + 'order/' + id + '/shipped', {});
  }

  requestOrderReturnRequest(
    externalId: string,
    reason: string
  ) {
    return this.http.put(
      this.baseUrl + 'order/' + externalId + '/return-requested',
      { reason }
    );
  }

  requestOrderCancelRequest(
    externalId: string,
    reason: string
  ) {
    return this.http.put(
      this.baseUrl + 'order/' + externalId + '/cancel-requested',
      { reason }
    );
  }

  confirmDelivered(
    externalId: string
  ) {
    return this.http.put(
      this.baseUrl + 'order/' + externalId + '/confirm-delivered',
      { }
    );
  }

  acceptOrderReturnRequest(id: number) {
    return this.http.put(this.baseUrl + 'order/' + id + '/return-accepted', {});
  }

  acceptOrderCancelRequest(id: number) {
    return this.http.put(this.baseUrl + 'order/' + id + '/cancel-accepted', {});
  }

  cancelOrder(id: number, cancelOrderRequest: CancelOrderRequest) {
    return this.http.put(
      this.baseUrl + 'order/' + id + '/cancel',
      cancelOrderRequest
    );
  }
}
