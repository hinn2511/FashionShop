import { ReviewService } from './../../_services/review.service';
import { Component, Input, OnInit } from '@angular/core';
import { CustomerReviewParams, ProductReview, ReviewSummary, UserReview } from 'src/app/_models/review';
import { Pagination } from 'src/app/_models/pagination';
import { CustomerFilterOrder } from 'src/app/_models/productParams';

@Component({
  selector: 'app-product-rating',
  templateUrl: './product-rating.component.html',
  styleUrls: ['./product-rating.component.css'],
})
export class ProductRatingComponent implements OnInit {
  @Input() productId: number;

  reviews: ProductReview[];

  pagination: Pagination;
  reviewParams: CustomerReviewParams;
  filterOrders: CustomerFilterOrder[] = [
    new CustomerFilterOrder(1, 'Score', 0, 'Score (low-high)'),
    new CustomerFilterOrder(2, 'Score', 1, 'Score (high-low)'),
    new CustomerFilterOrder(3, 'DateCreated', 0, 'Oldest'),
    new CustomerFilterOrder(4, 'DateCreated', 1, 'Newest'),
  ];
  

  selectedOrder: string;

  selectedScore: number = 0;

  productReviewSummary: ReviewSummary;
 
  constructor(private reviewService: ReviewService) {
    this.reviewParams = this.reviewService.getReviewParams();
  }

  ngOnInit(): void {
    this.getOrderReview();
    this.getOrderReviewSummary();
  }
  getOrderReviewSummary() {
    this.reviewService
    .getProductReviewSummaries(this.productId)
    .subscribe((result) => {
      this.productReviewSummary = result;
      });
  }

  getOrderReview() {
    this.reviewService.setReviewParams(this.reviewParams);

    this.reviewService
      .getProductReviews(this.productId, this.reviewParams)
      .subscribe((response) => {
        this.reviews = response.result;
        this.pagination = response.pagination;
      });
  }

  getReviewStar(score: number) {
    return Math.floor(score);
  }

  pageChanged(event: any) {
    if (this.reviewParams.pageNumber !== event.page) {
      this.reviewParams.pageNumber = event.page;
      this.reviewService.setReviewParams(this.reviewParams);
      this.getOrderReview();
    }
  }

  sort(type: number) {
    let filterOrder = this.filterOrders[type];
    this.selectedOrder = filterOrder.filterName;
    this.reviewParams.orderBy = filterOrder.orderBy;
    this.reviewParams.field = filterOrder.field;
    this.getOrderReview();
  }

  selectScore(score: number)
  {
    if(this.selectedScore == 0 || this.selectedScore != score)
      this.selectedScore = score;
    else  
      this.selectedScore = 0;
    this.reviewParams.score = this.selectedScore;
    this.getOrderReview();
  }
  
}
