import {
  OrderStatusFilter,
  CustomerStatusFilters,
} from 'src/app/_models/order';
import { FormGroup, FormBuilder } from '@angular/forms';
import { OrderService } from 'src/app/_services/order.service';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  QueryList,
  ViewChild,
  ViewChildren,
} from '@angular/core';
import { CustomerOrder, CustomerOrderParams } from 'src/app/_models/order';
import { Pagination } from 'src/app/_models/pagination';
import { debounceTime } from 'rxjs/operators';
import { CustomerFilterOrder } from 'src/app/_models/productParams';

@Component({
  selector: 'app-order-history',
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.css'],
})
export class OrderHistoryComponent implements OnInit, AfterViewInit {
  orders: CustomerOrder[] = [];
  @ViewChildren('statusBarItem') items: QueryList<ElementRef>;

  statusBarButtons: ElementRef[];

  orderFilterForm: FormGroup;
  pagination: Pagination;
  orderParams: CustomerOrderParams;
  selectedOrder: string;
  fromMaxDate: Date = new Date();
  toMinDate: Date = new Date();
  orderStatusList: OrderStatusFilter[] = CustomerStatusFilters;
  selectedOrderStatus: OrderStatusFilter;

  filterOrders: CustomerFilterOrder[] = [
    new CustomerFilterOrder(0, 'ExternalId', 0, 'Order ID (A-Z)'),
    new CustomerFilterOrder(1, 'ExternalId', 1, 'Order ID (Z-A)'),
    new CustomerFilterOrder(2, 'Total', 0, 'Total (low-high)'),
    new CustomerFilterOrder(3, 'Total', 1, 'Total (high-low)'),
    new CustomerFilterOrder(4, 'Date', 0, 'Oldest'),
    new CustomerFilterOrder(5, 'Date', 1, 'Newest'),
  ];

  constructor(private orderService: OrderService, private fb: FormBuilder) {
    this.orderParams = this.orderService.getOrderParams();
  }

  ngOnInit(): void {
    let now = new Date();
    let lastMonth = new Date();
    lastMonth.setMonth(lastMonth.getMonth() - 1);

    this.orderParams.to = now;
    this.orderParams.from = lastMonth;

    this.setOrderStatus(0);
    this.initializeForm();
    this.sort(5);
    this.loadOrders();
  }

  ngAfterViewInit() {
    this.statusBarButtons = this.items.toArray();
    
  }

  initializeForm() {
    this.orderFilterForm = this.fb.group({
      query: [''],
      from: [this.orderParams.from],
      to: [this.orderParams.to],
    });
    this.orderFilterForm.valueChanges.pipe(debounceTime(500)).subscribe(() => {
      this.fromMaxDate = new Date(this.orderFilterForm.controls.to.value);
      this.toMinDate = new Date(this.orderFilterForm.controls.from.value);
      this.orderParams.from = new Date(
        this.orderFilterForm.controls.from.value
      );
      this.orderParams.to = new Date(this.orderFilterForm.controls.to.value);
      this.orderParams.query = this.orderFilterForm.controls.query.value;
      this.loadOrders();
    });
  }

  loadOrders() {
    this.orderService.setOrderParams(this.orderParams);

    this.orderService
      .getCustomerOrders(this.orderParams)
      .subscribe((response) => {
        this.orders = response.result;
        this.pagination = response.pagination;
      });
  }

  pageChanged(event: any) {
    if (this.orderParams.pageNumber !== event.page) {
      this.orderParams.pageNumber = event.page;
      this.orderService.setOrderParams(this.orderParams);
      this.loadOrders();
    }
  }

  sort(type: number) {
    let filterOrder = this.filterOrders[type];
    this.orderParams.orderBy = filterOrder.orderBy;
    this.orderParams.field = filterOrder.field;
    this.selectedOrder = filterOrder.filterName;
    this.loadOrders();
  }

  filter(params: CustomerOrderParams) {
    this.orderParams = params;
    this.loadOrders();
  }

  resetFilter() {
    this.orderParams = this.orderService.resetOrderParams();
    this.loadOrders();
  }

  setOrderStatus(index: number) {
    setTimeout(() => {
      if (this.statusBarButtons.length != 0) {
        this.statusBarButtons[index].nativeElement.scrollIntoView({
          behavior: 'smooth',
          block: 'nearest',
          inline: 'center',
        });
      }
    }, 100);
    let status = this.orderStatusList[index];
    this.orderParams.orderStatusFilter = status.ids;
    this.selectedOrderStatus = status;
    this.loadOrders();
  }
}
