import { PaymentMethodList } from './../../_models/order';
import { CustomerOrder } from 'src/app/_models/order';
import { Component, OnInit } from '@angular/core';
import { OrderService } from 'src/app/_services/order.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css'],
})
export class OrderDetailComponent implements OnInit {
  order: CustomerOrder;
  paymentMethodString: string;

  constructor(private orderService: OrderService, private route: ActivatedRoute, private router: Router) {}

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
        
      });
  }
}
