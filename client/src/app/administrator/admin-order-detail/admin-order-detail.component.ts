import { DialogService } from './../../_services/dialog.service';
import { ToastrService } from 'ngx-toastr';
import { concatMap } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from 'src/app/_services/order.service';
import { Component, OnInit } from '@angular/core';
import { CancelOrderRequest, isAllowCancel, isAllowCancelAccept, isAllowReturnAccept, isAllowShipped, isAllowShipping, isAllowVerify, ManagerOrder, OrderStatusList } from 'src/app/_models/order';
import { fnGetOrderStateStyle } from 'src/app/_common/function/style-class';
import { fnGetOrderStateString } from 'src/app/_common/function/function';

@Component({
  selector: 'app-admin-order-detail',
  templateUrl: './admin-order-detail.component.html',
  styleUrls: ['./admin-order-detail.component.css'],
})
export class AdminOrderDetailComponent implements OnInit {
  order: ManagerOrder;
  showCancelButton: boolean = false;
  showVerifyButton: boolean = false;
  showShippingButton: boolean = false;
  showShippedButton: boolean = false;
  showAcceptReturnButton: boolean = false;
  showAcceptCancelButton: boolean = false;

  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private dialogService: DialogService
  ) {}

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    this.loadOrderDetail(+orderId);
  }

  loadOrderDetail(orderId: number) {
    this.orderService.getManagerOrderDetail(orderId).subscribe((result) => {
      this.order = result;
      this.displayButton();
    });
  }

  verifyingOrder() {
    this.dialogService
      .openConfirmDialog('Confirm', 'Are you sure you want to verify this order?', false)
      .pipe(
        concatMap((confirmResult) => {
          if (confirmResult.result)
            return this.orderService.verifyOrder(this.order.id);
        }),
        concatMap((_) => this.orderService.getManagerOrderDetail(this.order.id))
      )
      .subscribe(
        (order) => {
          this.order = order;
          this.displayButton();
          this.toastr.success('Verify order successfully.', 'Success');
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  deliveringOrder() {
    this.dialogService
      .openConfirmDialog('Confirm', 'Are you sure you want to ship this order?', false)
      .pipe(
        concatMap((confirmResult) => {
          if (confirmResult.result)
            return this.orderService.shippingOrder(this.order.id);
        }),
        concatMap((_) => this.orderService.getManagerOrderDetail(this.order.id))
      )
      .subscribe(
        (order) => {
          this.order = order;
          this.displayButton();
          this.toastr.success('Move order to delivery successfully.', 'Success');
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  orderDelivered() {
    this.dialogService
      .openConfirmDialog('Confirm', 'Are you sure you want to confirm this order has been delivered?', false)
      .pipe(
        concatMap((confirmResult) => {
          if (confirmResult.result)
            return this.orderService.shippedOrder(this.order.id);
        }),
        concatMap((_) => this.orderService.getManagerOrderDetail(this.order.id))
      )
      .subscribe(
        (order) => {
          this.order = order;
          this.displayButton();
          this.toastr.success('Order delivery has completed.', 'Success');
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  acceptReturn() {
    this.dialogService
      .openConfirmDialog('Confirm', 'Are you sure you want to accept this order return request?', false)
      .pipe(
        concatMap((confirmResult) => {
          if (confirmResult.result)
            return this.orderService.acceptOrderReturnRequest(this.order.id);
        }),
        concatMap((_) => this.orderService.getManagerOrderDetail(this.order.id))
      )
      .subscribe(
        (order) => {
          this.order = order;
          this.displayButton();
          this.toastr.success('Order return request accepted.', 'Success');
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  acceptCancel() {
    this.dialogService
      .openConfirmDialog('Confirm', 'Are you sure you want to accept this order cancel request?', false)
      .pipe(
        concatMap((confirmResult) => {
          if (confirmResult.result)
            return this.orderService.acceptOrderCancelRequest(this.order.id);
        }),
        concatMap((_) => this.orderService.getManagerOrderDetail(this.order.id))
      )
      .subscribe(
        (order) => {
          this.order = order;
          this.displayButton();
          this.toastr.success('Order cancel request accepted.', 'Success');
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  cancelOrder() {
    let showNotification = true;
    this.dialogService
    .openConfirmDialog('Confirm', 'Are you sure you want to cancel this order? If you do, please provide the reason for this cancellation.', true)
    .pipe(
      concatMap((confirmResult) => {
        if (confirmResult.result)
          return this.orderService.cancelOrder(this.order.id, new CancelOrderRequest(confirmResult.reason));
        else
          showNotification = false;
      }),
      concatMap((_) => this.orderService.getManagerOrderDetail(this.order.id))
    )
    .subscribe(
      (order) => {
        this.order = order;
        this.displayButton();
        this.toastr.success('Cancel order successfully.', 'Success');
      },
      (error) => {
        if (showNotification)
          this.toastr.error('Can not cancel this order', 'Error');
      }
    );
  }

  getOrderStateStyle(status: number) {
    return fnGetOrderStateStyle(status);
  }

  getOrderStateString(status: number) {
    return fnGetOrderStateString(status);
  }

  displayButton() {
    this.disableAllButton();
    let status = OrderStatusList.find(x => x.id == this.order.currentStatus);    
    if (isAllowCancel(status.id))
      this.showCancelButton = true;
    if (isAllowVerify(status.id))
      this.showVerifyButton = true;
    if (isAllowShipping(status.id))
      this.showShippingButton = true;
    if (isAllowShipped(status.id))
      this.showShippedButton = true;
    if (isAllowCancelAccept(status.id))
      this.showAcceptCancelButton = true;
    if (isAllowReturnAccept(status.id))
      this.showAcceptReturnButton = true;
      
  }

  disableAllButton()
  {
    this.showCancelButton = false;
    this.showVerifyButton = false;
    this.showShippedButton = false;
    this.showShippingButton = false;
    this.showAcceptCancelButton = false;
    this.showAcceptReturnButton = false;
  }
}
