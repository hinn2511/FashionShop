import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from 'src/app/_models/product';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.css']
})
export class ProductCardComponent implements OnInit {
  @Input() product: Product;
  shadow:string = 'shadow-sm';

  hover($event){
    this.shadow = $event.type == 'mouseover' ? 'shadow' : 'shadow-sm';
  }

  constructor(private router: Router) { }

  ngOnInit(): void {
    
  }

  viewDetail() {
    this.router.navigate(
      ['product/' + this.product.slug],
      { queryParams: { id: this.product.id } }
    );
  }
}
