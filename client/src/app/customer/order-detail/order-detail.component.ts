import { ToastrService } from 'ngx-toastr';
import { PaymentMethodList, isAllowCancelRequest, isAllowReturnRequest, isAllowConfirmDelivered, isAllowReview } from 'src/app/_models/order';
import { CustomerOrder } from 'src/app/_models/order';
import { Component, OnInit } from '@angular/core';
import { OrderService } from 'src/app/_services/order.service';
import { ActivatedRoute, Router } from '@angular/router';
import { DialogService } from 'src/app/_services/dialog.service';
import { concatMap } from 'rxjs/operators';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css'],
})
export class OrderDetailComponent implements OnInit {
  order: CustomerOrder;
  paymentMethodString: string;
  allowCancelRequest: boolean = false;
  allowReturnRequest: boolean = false;
  allowConfirmDelivered: boolean = false;
  allowReview: boolean = false;

  constructor(private orderService: OrderService, private dialogService: DialogService, private toastr: ToastrService, private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.getOrderDetail(this.route.snapshot.queryParams['id']);
  }

  convertPaymentMethodToString() {
    return PaymentMethodList[this.order.paymentMethod];
  }

  backToOrderHistories() {
    this.router.navigate(['account/'], { queryParams: { tab: 'orders' } });
  }

  getOrderDetail(orderId: string) {
    
    this.orderService
      .getCustomerOrder(orderId)
      .subscribe((response) => {
        this.order = response;
        this.convertPaymentMethodToString();
        this.showButton();
      });
  }

  private showButton() {
    this.allowCancelRequest = isAllowCancelRequest(this.order.currentStatus);
    this.allowReturnRequest = isAllowReturnRequest(this.order.currentStatus);
    this.allowConfirmDelivered = isAllowConfirmDelivered(this.order.currentStatus);
    this.allowReview = isAllowReview(this.order.currentStatus);
  }

  requestCancelOrder()
  {
    let showNotification = true;
    this.dialogService
    .openConfirmDialog('Confirm', 'Are you sure you want to cancel this order? If you do, please provide the reason for this cancellation.', true)
    .pipe(
      concatMap((confirmResult) => {
        if (confirmResult.result)
          return this.orderService.requestOrderCancelRequest(this.order.externalId, confirmResult.reason);
        else
          showNotification = false;
      }),
      concatMap((_) => {
        this.orderService.clearCustomerOrderCache();
        return this.orderService.getCustomerOrder(this.order.externalId)
      })
    )
    .subscribe(
      (order) => {
        this.order = order;
        this.showButton();
        this.toastr.success('Request order cancellation successfully.', 'Success');
      },
      (error) => {
        if (showNotification)
          this.toastr.error('Can not send cancel request for this order', 'Error');
      }
    );
  }

  requestReturnOrder()
  {
    let showNotification = true;
    this.dialogService
    .openConfirmDialog('Confirm', 'Are you sure you want to request refund this order? If you do, please provide the reason for this request.', true)
    .pipe(
      concatMap((confirmResult) => {
        if (confirmResult.result)
          return this.orderService.requestOrderReturnRequest(this.order.externalId, confirmResult.reason);
        else
          showNotification = false;
      }),
      concatMap((_) => {
        this.orderService.clearCustomerOrderCache();
        return this.orderService.getCustomerOrder(this.order.externalId)
      })
    )
    .subscribe(
      (order) => {
        this.order = order;
        this.showButton();
        this.toastr.success('Request to refund order successfully.', 'Success');
      },
      (error) => {
        if (showNotification)
          this.toastr.error('Can not send refund request for this order', 'Error');
      }
    );
  }

  confirmDelivered()
  {
    let showNotification = true;
    this.dialogService
    .openConfirmDialog('Confirm', 'Are you sure you want to confirm this order delivered?', false)
    .pipe(
      concatMap((confirmResult) => {
        if (confirmResult.result)
          return this.orderService.confirmDelivered(this.order.externalId);
        else
          showNotification = false;
      }),
      concatMap((_) => {
        this.orderService.clearCustomerOrderCache();
        return this.orderService.getCustomerOrder(this.order.externalId)
      })
    )
    .subscribe(
      (order) => {
        this.order = order;
        this.showButton();
        this.toastr.info('Your order has been received by you. Thank for using our service!', 'Success');
      },
      (error) => {
        if (showNotification)
          this.toastr.error(error, 'Error');
      }
    );
  }

  reviewOrder()
  {
    this.orderService.selectedOrderId = this.order.externalId;    
      this.router.navigate(
        ['order/review' ],
        { queryParams: { id: this.order.externalId } }
      );
    
  }
}
