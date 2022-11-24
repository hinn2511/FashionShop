import { ProductService } from 'src/app/_services/product.service';
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
    {
      id: 0,
      field: 'Name',
      orderBy: 0,
      name: 'Name (A-Z)',
    },
    {
      id: 1,
      field: 'Name',
      orderBy: 1,
      name: 'Name (Z-A)',
    },
    {
      id: 2,
      field: 'Price',
      orderBy: 0,
      name: 'Price (low-high)',
    },
    {
      id: 3,
      field: 'Price',
      orderBy: 1,
      name: 'Price (high-low)',
    },
    {
      id: 4,
      field: 'Sold',
      orderBy: 1,
      name: 'Best Seller',
    },
    {
      id: 5,
      field: 'Date',
      orderBy: 1,
      name: 'Newest',
    },
    {
      id: 6,
      field: '',
      orderBy: 0,
      name: 'Liked recently',
    },
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
    this.selectedOrder = filterOrder.name;
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
