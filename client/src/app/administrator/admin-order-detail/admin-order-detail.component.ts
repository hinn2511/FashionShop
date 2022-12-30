import { ToastrService } from 'ngx-toastr';
import { concatMap } from 'rxjs/operators';
import {
  fnGetOrderStateString,
  fnGetOrderStateStyle,
} from 'src/app/_common/function/global';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from 'src/app/_services/order.service';
import { Component, OnInit } from '@angular/core';
import { CancelOrderRequest, ManagerOrder } from 'src/app/_models/order';
import { ConfirmService } from 'src/app/_services/confirm.service';

@Component({
  selector: 'app-admin-order-detail',
  templateUrl: './admin-order-detail.component.html',
  styleUrls: ['./admin-order-detail.component.css'],
})
export class AdminOrderDetailComponent implements OnInit {
  order: ManagerOrder;
  showCancelButton: boolean = false;
  showVerifyButton: boolean = false;

  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private confirmService: ConfirmService
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
    this.confirmService
      .confirm('Confirm', 'Are you sure you want to verify this order?', false)
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
          this.showVerifyButton = false;
          this.toastr.success('Verify order successfully.', 'Success');
        },
        (error) => {
          this.toastr.error('Can not verify this order', 'Error');
        }
      );
  }

  cancelOrder() {
    let showNotification = true;
    this.confirmService
    .confirm('Confirm', 'Are you sure you want to cancel this order? If you do, please provide the reason for this cancellation.', true)
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
    let statusListAllowCancel = [0, 1, 2, 3];
    if (statusListAllowCancel.indexOf(this.order.currentStatus) >= 0)
      this.showCancelButton = true;
    if (this.order.currentStatus == 1) this.showVerifyButton = true;
  }
}
