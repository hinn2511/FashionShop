import { Component, OnInit } from '@angular/core';
import { Product } from 'src/app/_models/product';

@Component({
  selector: 'app-product-related',
  templateUrl: './product-related.component.html',
  styleUrls: ['./product-related.component.css']
})
export class ProductRelatedComponent implements OnInit {
  product: Product = {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1644470791/gagdlusutpljc28j5abj.jpg',
    productPrice: 450,
    productName: 'Chanel Shoe',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  }

  products: Product[] = [];

  itemsPerSlide = 4;

  constructor() { }

  ngOnInit(): void {
    for(let i = 0; i < 9; i++) {
      this.product.productName = this.product.productName + i;
      this.products.push(this.product);
    }
  }

}
