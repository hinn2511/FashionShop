import { CustomerOrder } from 'src/app/_models/order';
import { Component, Input, OnInit} from '@angular/core';
import { OrderService } from 'src/app/_services/order.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-card',
  templateUrl: './order-card.component.html',
  styleUrls: ['./order-card.component.css']
})
export class OrderCardComponent implements OnInit {
  @Input() order: CustomerOrder;
  
  constructor(private orderService: OrderService, private router: Router) { }

  ngOnInit(): void {
  }

  viewDetail()
  {
    this.orderService.selectedOrderId = this.order.externalId;    
      this.router.navigate(
        ['order' ],
        { queryParams: { id: this.order.externalId } }
      );
    
  }

}
