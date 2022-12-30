import { Component, Input, OnInit } from '@angular/core';
import { Product } from 'src/app/_models/product';

@Component({
  selector: 'app-product-carousel',
  templateUrl: './product-carousel.component.html',
  styleUrls: ['./product-carousel.component.css']
})

export class ProductCarouselComponent implements OnInit {
  @Input() singleSlide: boolean;

  products: Product[] = [{
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 1',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 2',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 3',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 4',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 5',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 6',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 7',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 8',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }, {
    url: 'https://res.cloudinary.com/dsqfbwwmq/image/upload/v1643121899/o17gkl0actlmbwehcsnv.jpg',
    price: 450,
    productName: 'Product 9',
    slug: '',
    id: 0,
    description: "",
    categoryId: 0,
    brandId: 1,
    productPhotos: null,
    categoryName: "XYZ",
    brandName: "XYZ",
    likedByUser: false
  }];

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

    
  }

}
