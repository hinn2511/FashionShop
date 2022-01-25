import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/_models/pagination';
import { Product } from 'src/app/_models/product';
import { ProductParams } from 'src/app/_models/productParams';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[];
  pagination: Pagination;
  productParams: ProductParams;
  sort: string = "Popular";

  constructor(private productService: ProductService) {
    this.productParams = this.productService.getProductParams();
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

  pageChanged(event: any) {
    if (this.productParams.pageNumber !== event.page) {
      this.productParams.pageNumber = event.page;
      this.productService.setProductParams(this.productParams);
      this.loadProducts();
    }
  }

  selectSortBy(type: string) {
    this.sort = type;
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
