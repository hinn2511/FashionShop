import { Subscription } from 'rxjs';
import { ReviewService } from './../../_services/review.service';
import {
  fnUpdateFormControlNumberValue,
  fnGetFormControlValue,
} from 'src/app/_common/function/function';
import {
  Component,
  OnInit,
  OnChanges,
  SimpleChanges,
  OnDestroy,
} from '@angular/core';
import { FormArray, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CustomerOrder } from 'src/app/_models/order';
import { OrderService } from 'src/app/_services/order.service';
import { OrderReviewedItem } from 'src/app/_models/review';

@Component({
  selector: 'app-order-review',
  templateUrl: './order-review.component.html',
  styleUrls: ['./order-review.component.css'],
})
export class OrderReviewComponent implements OnInit {
  order: CustomerOrder;
  reviewedOrderItems: OrderReviewedItem[] = [];
  stars: number[] = [1, 2, 3, 4, 5];

  reviewForm: FormGroup;
  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    private reviewService: ReviewService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.initForm();
    let orderId = this.route.snapshot.queryParams['id'];
    this.getOrderDetail(orderId);
    this.getOrderReviewedItem(orderId);
  }

  initForm() {
    this.reviewForm = this.fb.group({
      reviewItems: this.fb.array([]),
    });
  }

  getReviewRating(index: number) {
    let score = this.reviewItems.value[index].score;
    return this.getRating(score);
  }

  getRating(score: number) {
    switch (score) {
      case 1:
        return 'Very bad';
      case 2:
        return 'Bad';
      case 3:
        return 'Fine';
      case 4:
        return 'Good';
      case 5:
        return 'Very good';
      default:
        return '';
    }
  }

  addReviewItem(optionId: number) {
    const reviewItemForm = this.fb.group({
      score: [0, [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: ['', [Validators.maxLength(200)]],
      optionId: [optionId, Validators.required],
    });

    this.reviewItems.push(reviewItemForm);
  }

  get reviewItems() {
    return this.reviewForm.controls['reviewItems'] as FormArray;
  }

  createReview(index: number) {
    this.reviewService
      .createOrderReview(
        this.order.externalId,
        this.getReviewFormGroup(index).value
      )
      .subscribe(
        (result) => {
          this.deleteReviewForm(index);
          this.toastr.success(result.message, 'Success');
          this.getOrderReviewedItem(this.order.externalId);
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  getReviewFormGroup(index: number) {
    let reviewArrayControl = this.reviewForm.get('reviewItems') as FormArray;
    return reviewArrayControl.at(index) as FormGroup;
  }

  getReviewFormGroupLength(index: number) {
    return fnGetFormControlValue(this.getReviewFormGroup(index), 'comment')
      .length;
  }

  deleteReviewForm(index: number) {
    this.reviewItems.removeAt(index);
  }

  getOrderDetail(orderId: string) {
    this.orderService.getCustomerOrder(orderId).subscribe((response) => {
      this.order = response;
      this.order.orderDetails.forEach((element) => {
        if (!element.isReviewed) this.addReviewItem(element.optionId);
      });
    });
  }

  getOrderReviewedItem(orderId: string) {
    this.reviewService.getOrderReviewedItems(orderId).subscribe((response) => {
      this.reviewedOrderItems = response;
    });
  }

  backToOrderDetail() {
    this.orderService.selectedOrderId = this.order.externalId;
    this.router.navigate(['order'], {
      queryParams: { id: this.order.externalId },
    });
  }

  selectStar(value: number, reviewFormIndex: number): void {
    fnUpdateFormControlNumberValue(
      this.getReviewFormGroup(reviewFormIndex),
      'score',
      value + 1,
      false
    );
  }

  getStar(reviewFormIndex: number) {
    return fnGetFormControlValue(
      this.getReviewFormGroup(reviewFormIndex),
      'score'
    );
  }
}
