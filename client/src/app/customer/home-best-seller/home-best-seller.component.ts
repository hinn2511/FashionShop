import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Product } from 'src/app/_models/product';

@Component({
  selector: 'app-home-best-seller',
  templateUrl: './home-best-seller.component.html',
  styleUrls: ['./home-best-seller.component.css'],
})
export class HomeBestSellerComponent implements OnInit {
  products: Product[] = [{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 1',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 2',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 3',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 4',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 5',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 6',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 7',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 8',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  },{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    productPrice: 450,
    productName: 'Product 9',
    gender: '',
    slug: '',
    id: 0,
    category: '',
    sold: 0,
    stocks: null,
    productPhotos: null
  }];

  itemsPerSlide: number;

  showItemQuantity(): void {
    const width = window.innerWidth;
    if (width < 480)
      this.itemsPerSlide = 1;
    if (width >= 480 && width <= 920)
      this.itemsPerSlide = 2;
    if (width > 920)
      this.itemsPerSlide = 4;
  }

  constructor() { }

  ngOnInit(): void {
    this.showItemQuantity();
  }
}
