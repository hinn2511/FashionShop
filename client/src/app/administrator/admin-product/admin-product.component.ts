import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BreadCrumb } from 'src/app/_models/breadcrum';
import { Pagination } from 'src/app/_models/pagination';
import { Product } from 'src/app/_models/product';
import { ProductParams } from 'src/app/_models/productParams';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-admin-product',
  templateUrl: './admin-product.component.html',
  styleUrls: ['./admin-product.component.css']
})
export class AdminProductComponent implements OnInit {

  products: Product[];
  pagination: Pagination;
  productParams: ProductParams;
  selectAll: boolean;

  constructor(private productService: ProductService, private router: Router) {
    this.productParams = this.productService.getProductParams();
   }

  ngOnInit(): void {
    this.productParams.field = 'Name';
    this.productParams.orderBy = 0;
    this.selectAll = false;
    this.loadProducts();
  }

  loadProducts() {
    this.productService.setProductParams(this.productParams);

    this.productService.getProducts(this.productParams).subscribe(response => {
      this.products = response.result;
      this.pagination = response.pagination;
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

  viewDetail(productId: number)
  {
    this.productService.setSelectedProductId(productId);
    this.router.navigateByUrl("/administrator/product-manager/detail/" + productId);
  }

}
