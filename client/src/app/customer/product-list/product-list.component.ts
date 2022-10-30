import { transition, trigger, useAnimation } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { fadeIn, fadeOut, scaleIn, scaleOut } from 'src/app/_common/animation/carousel.animations';
import { BreadCrumb } from 'src/app/_models/breadcrum';
import { Pagination } from 'src/app/_models/pagination';
import { Category, Product } from 'src/app/_models/product';
import { ProductParams } from 'src/app/_models/productParams';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})

export class ProductListComponent implements OnInit {
  products: Product[];
  categories: Category[];

  skeletonItems: number[];
  skeletonLoading: boolean = false;

  pagination: Pagination;
  productParams: ProductParams;

  breadCrumb: BreadCrumb[] = [
    {
      name: 'Home',
      route: '/'
    },
    {
      name: 'All products',
      route: ''
    },
  ];

  constructor(private productService: ProductService) {
    this.productParams = this.productService.getProductParams();
  }

  ngOnInit(): void {
    this.skeletonLoading = true;
    this.skeletonItems = Array(this.productParams.pageSize).fill(1);
    this.productParams.field = 'Sold';
    this.loadProducts();
    this.loadCategories('all');
  }

  loadProducts() {
    this.skeletonLoading = true;
    this.productService.setProductParams(this.productParams);

    this.productService.getProducts(this.productParams).subscribe(response => {
      setTimeout(() => {
        this.skeletonLoading = false;
      }, 100);
      this.products = response.result;
      this.pagination = response.pagination;
    })
  }

  loadCategories(gender: string) {
    this.productService.getCategories().subscribe(response => {
      this.categories = response;
    })
  }

  pageChanged(event: any) {
    if (this.productParams.pageNumber !== event.page) {
      this.productParams.pageNumber = event.page;
      this.productService.setProductParams(this.productParams);
      this.loadProducts();
    }
  }

  sort(type: number) {
    this.productParams.orderBy = type;
    this.loadProducts();
  }

  filter(params: ProductParams) {
    this.productParams = params;
    this.loadProducts();
  }

  resetFilter() {
    this.productParams = this.productService.resetProductParams();
    this.loadProducts();
  }
}
