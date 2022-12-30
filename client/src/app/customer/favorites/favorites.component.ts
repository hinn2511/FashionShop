import { Product } from './../../_models/product';
import { Component, OnInit } from '@angular/core';
import { Pagination } from 'src/app/_models/pagination';
import { CustomerFilterOrder, ProductParams } from 'src/app/_models/productParams';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.css']
})
export class FavoritesComponent implements OnInit {
  products: Product[] = [];
  pagination: Pagination;
  productParams: ProductParams;
  selectedOrder: string;

  filterOrders: CustomerFilterOrder[] = [
    new CustomerFilterOrder(0, 'Name', 0, 'Name (A-Z)'),
    new CustomerFilterOrder(1, 'Name', 1, 'Name (Z-A)'),
    new CustomerFilterOrder(2, 'Price', 0, 'Price (low-high)'),
    new CustomerFilterOrder(3, 'Price', 1, 'Price (high-low)'),
    new CustomerFilterOrder(4, 'Sold', 0, 'Best seller'),
    new CustomerFilterOrder(5, 'Date', 1, 'Newest'),
    new CustomerFilterOrder(6, '', 0, 'Liked recently'),
  ];

  constructor(private accountService: AccountService) {
    this.productParams = this.accountService.getProductParams();
   }

  ngOnInit(): void {
    this.sort(6);
  }

  loadProducts() {
    this.accountService.setProductParams(this.productParams);

    this.accountService.getFavorites(this.productParams).subscribe(response => {
      this.products = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: any) {
    if (this.productParams.pageNumber !== event.page) {
      this.productParams.pageNumber = event.page;
      this.accountService.setProductParams(this.productParams);
      this.loadProducts();
    }
  }

  sort(type: number) {
    let filterOrder = this.filterOrders[type];
    this.selectedOrder = filterOrder.filterName;
    this.productParams.orderBy = filterOrder.orderBy;
    this.productParams.field = filterOrder.field;
    this.loadProducts();
  }

  filter(params: ProductParams) {
    this.productParams = params;
    this.loadProducts();
  }

  resetFilter() {
    this.productParams = this.accountService.resetProductParams();
    this.loadProducts();
  }

  reload(event: any)
  {
    this.accountService.clearFavoriteCache();
    this.loadProducts();
  }

}
