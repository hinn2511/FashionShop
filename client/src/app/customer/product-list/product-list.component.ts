import { Component, OnInit } from '@angular/core';
import { BreadCrumb } from 'src/app/_models/breadcrum';
import { Category } from 'src/app/_models/category';
import { Pagination } from 'src/app/_models/pagination';
import { Product } from 'src/app/_models/product';
import { ProductParams } from 'src/app/_models/productParams';
import { CategoryService } from 'src/app/_services/category.service';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[];
  categories: Category[];
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

  constructor(private productService: ProductService, private categoryService: CategoryService) {
    this.productParams = this.productService.getProductParams();
  }

  ngOnInit(): void {
    this.productParams.field = 'Sold';
    this.loadProducts();
    this.loadCategories('all');
  }

  loadProducts() {
    this.productService.setProductParams(this.productParams);

    this.productService.getProducts(this.productParams).subscribe(response => {
      this.products = response.result;
      this.pagination = response.pagination;
    })
  }

  loadCategories(gender: string) {
    this.categoryService.getCategories(gender).subscribe(response => {
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
