import { Component, OnInit } from '@angular/core';
import { Product } from 'src/app/_models/product';

@Component({
  selector: 'app-product-related',
  templateUrl: './product-related.component.html',
  styleUrls: ['./product-related.component.css']
})
export class ProductRelatedComponent implements OnInit {
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
  }, {
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
  }, {
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
  }, {
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
  }, {
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
  }, {
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
  }, {
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
  }, {
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
  }, {
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

  showMoreButton: boolean;

  itemsPerClick: number = 4;

  itemsInitNumber: number = 4;
  
  currentProducts: Product[];

  constructor() { }

  ngOnInit(): void {
    this.currentProducts = this.products.slice(0, this.itemsInitNumber);
    this.showMoreButton = true;
  }

  getSize(): string {
    const width = window.innerWidth;
    if (width < 480)
      return "col-12 px-0 mx-0";
    if (width >= 480 && width <= 920)
      return "col-4";
    if (width > 920)
      return "col-3";
  }

  view(): void {
      let newLength: number = this.itemsInitNumber + this.itemsPerClick;
      this.currentProducts = this.products.slice(0, newLength);
      if(newLength == this.products.length -1)
        this.showMoreButton = false;
    
  }

}
