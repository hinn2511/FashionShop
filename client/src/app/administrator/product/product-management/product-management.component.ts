import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ProductService } from 'src/app/_services/product.service';
import { Router } from '@angular/router';
import { Product } from 'src/app/_models/product';
import { Pagination } from 'src/app/_models/pagination';
import { ProductParams } from 'src/app/_models/productParams';

@Component({
  selector: 'app-product-management',
  templateUrl: './product-management.component.html',
  styleUrls: ['./product-management.component.css']
})
export class ProductManagementComponent implements OnInit {
  products: Product[];
  pagination: Pagination;
  productParams: ProductParams;
  pageNumber = 1;
  pageSize = 4;

  constructor(private productService: ProductService) {
    this.productParams = this.productService.resetProductParams();
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.setProductParams(this.productParams);

    this.productService.getProducts(this.productParams).subscribe(response => {
      this.products = response.result;
      this.pagination = response.pagination;
    })
  }

  deleteProducts(id: number) {
    this.productService.deleteProduct(id).subscribe(response => {
      this.products = this.products.filter(p => p.id != id);
    });
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
