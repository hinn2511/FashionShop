import { Component, OnInit } from '@angular/core';
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import {
  ManagerOrder,
  ManagerOrderParams,
  ManagerOrderSummary,
  OrderStatus,
  OrderStatusList,
  PaymentMethod,
  PaymentMethodList,
  ShippingMethod,
  ShippingMethodList,
} from 'src/app/_models/order';
import { Pagination } from 'src/app/_models/pagination';
import { OrderService } from 'src/app/_services/order.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { fnGetOrderStateString, fnGetOrderStateStyle } from 'src/app/_common/function/global';

@Component({
  selector: 'app-admin-order',
  templateUrl: './admin-order.component.html',
  styleUrls: ['./admin-order.component.css'],
  animations: [
    trigger('rotatedState', [
      state('default', style({ transform: 'rotate(0)' })),
      state('rotated', style({ transform: 'rotate(-180deg)' })),
      transition('rotated => default', animate('500ms ease-out')),
      transition('default => rotated', animate('500ms ease-in')),
    ]),
  ],
})
export class AdminOrderComponent implements OnInit {
  orders: ManagerOrder[];
  pagination: Pagination;
  orderParams: ManagerOrderParams;
  selectAllOrder: boolean;
  state: string = 'default';
  showOrderStatusFilter: boolean = false;
  showPaymentMethodFilter: boolean = false;
  showShippingMethodFilter: boolean = false;
  selectedIds: number[] = [];
  OrderStatusList: OrderStatus[] = OrderStatusList;
  shippingMethods: ShippingMethod[] = ShippingMethodList;
  paymentMethods: PaymentMethod[] = PaymentMethodList;
  orderSummary: ManagerOrderSummary[] = [];
  orderFilterForm: FormGroup;
  fromMaxDate: Date = new Date();
  toMinDate: Date = new Date();
  viewAllStatus: boolean = false;

  constructor(
    private orderService: OrderService,
    private router: Router,
    private toastr: ToastrService,
    private fb: FormBuilder
  ) {
    this.orderParams = this.orderService.getManagerOrderParams();
  }

  ngOnInit(): void {
    this.orderParams.field = 'DateCreated';
    this.orderParams.orderBy = 1;
    this.orderParams.orderStatusFilter = [1];

    let now = new Date();
    let lastMonth = new Date();
    lastMonth.setMonth(lastMonth.getMonth() - 1);

    this.orderParams.to = now;
    this.orderParams.from = lastMonth;

    this.selectAllOrder = false;

    this.initializeForm();
    this.loadOrders();
    this.loadOrderSummary();
  }

  // load filter query and date range
  initializeForm() {
    this.orderFilterForm = this.fb.group({
      query: [''],
      from: [this.orderParams.from],
      to: [this.orderParams.to],
    });

    this.updateDateFilterValidation();

    this.orderFilterForm.valueChanges.pipe(debounceTime(500)).subscribe(() => {
      this.orderParams.from = new Date(
        this.orderFilterForm.controls.from.value
      );
      this.orderParams.to = new Date(this.orderFilterForm.controls.to.value);
      this.orderParams.query = this.orderFilterForm.controls.query.value;

      this.updateDateFilterValidation();

      this.loadOrders();
    });
  }

  private updateDateFilterValidation() {
    this.fromMaxDate = new Date(this.orderFilterForm.controls.to.value);
    this.toMinDate = new Date(this.orderFilterForm.controls.from.value);
  }

  // load orders

  loadOrders() {
    this.orderService.setManagerOrderParams(this.orderParams);

    this.orderService
      .getManagerOrders(this.orderParams)
      .subscribe((response) => {
        this.orders = response.result;
        this.pagination = response.pagination;
      });
  }

  loadOrderSummary() {
    this.orderService.getManagerOrderSummary().subscribe((response) => {
      let result = response;
      this.OrderStatusList.forEach((element) => {
        let statusCount = result.find((x) => x.orderStatus == element.id);
        if (statusCount != undefined) element.total = statusCount.total;
      });
      this.orderSummary = result;
    });
  }

  // apply filter and pagination
  pageChanged(event: any) {
    if (this.orderParams.pageNumber !== event.page) {
      this.orderParams.pageNumber = event.page;
      this.orderService.setManagerOrderParams(this.orderParams);
      this.loadOrders();
    }
  }

  orderBy(field: string) {
    switch (field) {
      case 'id':
        this.orderParams.field = 'Id';
        break;
      case 'externalId':
        this.orderParams.field = 'ExternalId';
        break;
      case 'firstName':
        this.orderParams.field = 'FirstName';
        break;
      case 'lastName':
        this.orderParams.field = 'LastName';
        break;
      case 'totalPrice':
        this.orderParams.field = 'TotalPrice';
        break;
      case 'shippingMethod':
        this.orderParams.field = 'ShippingMethod';
        break;
      case 'paymentMethod':
        this.orderParams.field = 'PaymentMethod';
        break;
      case 'totalQuantity':
        this.orderParams.field = 'TotalQuantity';
        break;
      case 'status':
        this.orderParams.field = 'Status';
        break;
      default:
        this.orderParams.field = 'DateCreated';
        break;
    }
    if (this.orderParams.orderBy == 0) this.orderParams.orderBy = 1;
    else this.orderParams.orderBy = 0;
    this.rotate();
    this.loadOrders();
  }

  filter(params: ManagerOrderParams) {
    this.orderParams = params;
    this.loadOrders();
  }

  resetFilter() {
    this.orderParams = this.orderService.resetManagerOrderParams();
    this.loadOrders();
  }

  // view detail
  viewDetail(orderId: number) {
    this.router.navigateByUrl('/administrator/order-manager/detail/' + orderId);
  }

  // order table sort indicator
  rotate() {
    this.state = this.state === 'default' ? 'rotated' : 'default';
  }

  // select order
  selectAllOrders() {
    if (this.selectAllOrder) {
      this.selectedIds = [];
    } else {
      this.selectedIds = this.orders.map(({ id }) => id);
    }
    this.selectAllOrder = !this.selectAllOrder;
  }

  selectOrder(id: number) {
    if (this.selectedIds.includes(id)) {
      this.selectedIds.splice(this.selectedIds.indexOf(id), 1);
    } else {
      this.selectedIds.push(id);
    }
  }

  isOrderSelected(id: number) {
    if (this.selectedIds.indexOf(id) >= 0) return true;
    return false;
  }

  isSingleSelected() {
    return this.selectedIds.length == 1;
  }

  isMultipleSelected() {
    return this.selectedIds.length >= 1;
  }

  // order status filter
  statusFilterToggle() {
    this.showOrderStatusFilter = !this.showOrderStatusFilter;
  }

  isStatusIncluded(status: number) {
    return this.orderParams.orderStatusFilter.indexOf(status) > -1;
  }

  showStatus(status: number, hideWhenViewAllStatus: boolean) {
    if (this.viewAllStatus && hideWhenViewAllStatus) return false;
    return this.orderParams.orderStatusFilter.indexOf(status) > -1;
  }

  selectAllOrderStatus() {
    if (this.orderParams.orderStatusFilter.length == this.OrderStatusList.length)
      this.orderParams.orderStatusFilter = [];
    else
      this.orderParams.orderStatusFilter = this.OrderStatusList.map(
        ({ id }) => id
      );
    this.loadOrders();
  }

  selectStatus(status: number, pendingToCurrentStatus: boolean) {
    if (pendingToCurrentStatus) {
      if (this.isStatusIncluded(status)) {
        this.orderParams.orderStatusFilter =
          this.orderParams.orderStatusFilter.filter((x) => x != status);
        console.log(this.orderParams.orderStatusFilter);
      } else this.orderParams.orderStatusFilter.push(status);
      this.orderParams.orderStatusFilter = [
        ...this.orderParams.orderStatusFilter
      ].sort((a, b) => a - b);
    } else {
      this.viewAllStatus = false;
      this.orderParams.orderStatusFilter = [status];
    }
    this.loadOrders();
  }

  // order payment method filter
  paymentMethodFilterToggle() {
    this.showPaymentMethodFilter = !this.showPaymentMethodFilter;
  }

  isPaymentMethodIncluded(paymentMethod: number) {
    return this.orderParams.paymentMethodFilter.indexOf(paymentMethod) > -1;
  }

  selectAllPaymentMethod() {
    if (
      this.orderParams.paymentMethodFilter.length == this.paymentMethods.length
    )
      this.orderParams.paymentMethodFilter = [];
    else
      this.orderParams.paymentMethodFilter = this.paymentMethods.map(
        ({ id }) => id
      );
    this.loadOrders();
  }

  selectPaymentMethod(paymentMethod: number) {
    if (this.isPaymentMethodIncluded(paymentMethod))
      this.orderParams.paymentMethodFilter =
        this.orderParams.paymentMethodFilter.filter((x) => x !== paymentMethod);
    else this.orderParams.paymentMethodFilter.push(paymentMethod);
    this.orderParams.paymentMethodFilter =  [
      ...this.orderParams.paymentMethodFilter
    ].sort((a, b) => a - b);
    this.loadOrders();
  }

  // order shipping method filter
  shippingMethodFilterToggle() {
    this.showShippingMethodFilter = !this.showShippingMethodFilter;
  }

  isShippingMethodIncluded(shippingMethod: string) {
    return this.orderParams.shippingMethodFilter.indexOf(shippingMethod) > -1;
  }

  selectAllShippingMethod() {
    if (
      this.orderParams.shippingMethodFilter.length ==
      this.shippingMethods.length
    )
      this.orderParams.shippingMethodFilter = [];
    else
      this.orderParams.shippingMethodFilter = this.shippingMethods.map(
        ({ name }) => name
      );
    this.loadOrders();
  }

  selectShippingMethod(shippingMethod: string) {
    if (this.isShippingMethodIncluded(shippingMethod))
      this.orderParams.shippingMethodFilter =
        this.orderParams.shippingMethodFilter.filter(
          (x) => x !== shippingMethod
        );
    else this.orderParams.shippingMethodFilter.push(shippingMethod);
    this.orderParams.shippingMethodFilter = 
      [...this.orderParams.shippingMethodFilter].sort();
    this.loadOrders();
  }

  getOrderStateStyle(status: number) {
    return fnGetOrderStateStyle(status);
  }

  getOrderStateString(status: number) {
    return fnGetOrderStateString(status);
  }

  viewAllStatusFilter() {
    if (!this.viewAllStatus) {
      this.viewAllStatus = true;
      this.selectAllOrderStatus();
    }
  }
}
