import { Component, HostListener, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Product } from 'src/app/_models/product';

@Component({
  selector: 'app-product-carousel',
  templateUrl: './product-carousel.component.html',
  styleUrls: ['./product-carousel.component.css']
})

export class ProductCarouselComponent implements OnInit {
  @Input() singleSlide: boolean;

  product: Product = {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  }

  products: Product[] = [];

  itemsPerSlide: number;

  showItemQuantity(): void {
    const width = window.innerWidth;
    if (width < 480)
      this.itemsPerSlide = 2;
    if (width >= 480 && width <= 920)
      this.itemsPerSlide = 3;
    if (width > 920)
      this.itemsPerSlide = 4;
  }

  constructor() { }

  ngOnInit(): void {
    this.showItemQuantity();
    for (let i = 0; i < 9; i++) {
      let p = this.product;
      p.productName = i.toString();
      this.products.push(p);
    }
    
  }

}
