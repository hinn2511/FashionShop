import { CategoryService } from 'src/app/_services/category.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { transition, trigger, useAnimation } from '@angular/animations';
import { Component, OnInit, OnDestroy } from '@angular/core';
import {
  fadeIn,
  fadeOut,
  scaleIn,
  scaleOut,
} from 'src/app/_common/animation/carousel.animations';
import { filter, map } from 'rxjs/operators';
import { BreadCrumb } from 'src/app/_models/breadcrum';
import { Pagination } from 'src/app/_models/pagination';
import { Category, Product } from 'src/app/_models/product';
import {
  CustomerFilterOrder,
  CustomerPriceRange,
  ProductParams,
} from 'src/app/_models/productParams';
import { ProductService } from 'src/app/_services/product.service';
import { combineLatest, Observable } from 'rxjs';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
})
export class ProductListComponent implements OnInit {
  products: Product[];
  categories: Category[];

  skeletonItems: number[];
  skeletonLoading: boolean = false;
  loadingCount: number = 0;
  pagination: Pagination;
  productParams: ProductParams;
  breadCrumb: BreadCrumb[];

  // filter selected value
  selectedPriceRange: CustomerPriceRange;
  showResetPriceRangeButton: boolean = false;
  selectedOrder: string;
  selectedCategory: string;
  selectedGender: number;

  //filter values
  priceRanges: CustomerPriceRange[] = [
    {
      id: 0,
      minPrice: 0,
      maxPrice: 50,
    },
    {
      id: 1,
      minPrice: 50,
      maxPrice: 100,
    },
    {
      id: 2,
      minPrice: 100,
      maxPrice: 300,
    },
    {
      id: 3,
      minPrice: 300,
      maxPrice: 0,
    },
  ];

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
  ];

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private route: ActivatedRoute
  ) {
    this.productParams = this.productService.getProductParams();
  }

  ngOnInit(): void {
    this.route.queryParamMap
      .pipe(
        filter(
          (queryParamMap) =>
            queryParamMap.has('category') || queryParamMap.has('gender')
        )
      )
      .subscribe((queryParamMap) => {
        this.selectedCategory = queryParamMap.get('category');
        this.selectedGender = +(queryParamMap.get('gender'));
        if (
          this.selectedCategory != this.productParams.category ||
          this.selectedGender != this.productParams.gender
        ) {
          this.productParams.category = this.selectedCategory;
          this.productParams.gender = this.selectedGender;
          this.updateBreadCrumb();
          this.loadProducts();
        }
      });

    this.skeletonLoading = true;
    this.skeletonItems = Array(this.productParams.pageSize).fill(1);
    this.productParams.field = 'Sold';
    this.productParams.orderBy = 1;
    this.selectedOrder = this.filterOrders[4].name;

    this.loadProducts();
  }

  private updateBreadCrumb() {
    this.breadCrumb = [];
    this.breadCrumb = [
      {
        name: 'Home',
        route: '/',
      },
    ];
    if (this.productParams.gender != null) {
      this.breadCrumb.push({
        name: this.categoryService.getCurrentGender(),
        route: '/' + this.categoryService.getCurrentGender().toLowerCase(),
      });
    }
    if (this.productParams.category != null) {
      this.breadCrumb.push({
        name: this.categoryService.getCurrentCategory(),
        route: '/' + this.categoryService.getCurrentCategory(),
      });
    }
  }

  loadProducts() {
    if (this.loadingCount == 0)
      this.skeletonLoading = true;
    this.productService.setProductParams(this.productParams);

    this.productService
      .getProducts(this.productParams)
      .subscribe((response) => {
        if (this.loadingCount == 0)
        {
          this.loadingCount++;
          setTimeout(() => {
            this.skeletonLoading = false;
          }, 500);
        }
        this.products = response.result;
        this.pagination = response.pagination;
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
    let filterOrder = this.filterOrders[type];
    this.productParams.orderBy = filterOrder.orderBy;
    this.productParams.field = filterOrder.field;
    this.loadProducts();
  }

  filter(params: ProductParams) {
    this.productParams = params;
    this.loadProducts();
  }

  resetFilter() {
    this.resetPriceRange();
    this.productParams = this.productService.resetProductParams();
    this.productParams.category = this.selectedCategory;
    this.productParams.gender = this.selectedGender;
    this.loadProducts();
  }

  setPriceRange(priceRangId: number) {
    var priceRange = this.priceRanges.find((x) => x.id == priceRangId);
    this.productParams.minPrice = priceRange?.minPrice;
    this.productParams.maxPrice = priceRange?.maxPrice;
    this.selectedPriceRange = priceRange;
    this.showResetPriceRangeButton = true;
    this.loadProducts();
  }

  isPriceRangeSelected() {
    return this.selectedPriceRange == null || this.selectedPriceRange == undefined;
  }

  resetPriceRange() {
    this.selectedPriceRange = undefined;
    this.showResetPriceRangeButton = false;
    this.productParams.minPrice = 0;
    this.productParams.maxPrice = 0;
    this.loadProducts();
  }
}
