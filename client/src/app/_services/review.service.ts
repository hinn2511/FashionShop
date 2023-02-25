import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  getPaginatedResult,
  getPaginationHeaders,
} from '../_helpers/paginationHelper';
import { ResponseMessage } from '../_models/generic';
import { CreateUserReviewRequest, CustomerReviewParams, EditUserReviewRequest, OrderReviewedItem, ProductReview, ReviewSummary } from '../_models/review';

@Injectable({
  providedIn: 'root',
})
export class ReviewService {
  baseUrl = environment.apiUrl;
  customerReviewParams: CustomerReviewParams;

  constructor(private http: HttpClient) {
    this.customerReviewParams = new CustomerReviewParams();
  }

  getReviewParams() {
    return this.customerReviewParams;
  }

  setReviewParams(params: CustomerReviewParams) {
    this.customerReviewParams = params;
  }

  resetReviewParams() {
    this.customerReviewParams = new CustomerReviewParams();
    return this.customerReviewParams;
  }

  getProductReviews(productId: number, orderParams: CustomerReviewParams) {
    let params = getPaginationHeaders(
      orderParams.pageNumber,
      orderParams.pageSize
    );
    params = params.append('orderBy', orderParams.orderBy);
    params = params.append('field', orderParams.field);    
    params = params.append('score', orderParams.score);

    return getPaginatedResult<ProductReview[]>(
      this.baseUrl + 'review/' + productId,
      params,
      this.http
    );
  }

  getProductReviewSummaries(productId: number) {
    return this.http.get<ReviewSummary>(
      this.baseUrl + 'review/' + productId + '/summary');
  }

  getOrderReviewedItems(externalId: string) {
    return this.http.get<OrderReviewedItem[]>(
      this.baseUrl + 'review/' + externalId + '/reviewed');
  }

  createOrderReview(externalId: string, createUserReviewRequest: CreateUserReviewRequest) {
    return this.http.post<ResponseMessage>(
      this.baseUrl + 'review/' + externalId,
      createUserReviewRequest
    );
  }

  editOrderReview(externalId: string, editUserReviewRequest: EditUserReviewRequest) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'review/' + externalId,
      editUserReviewRequest
    );
  }

  
}
