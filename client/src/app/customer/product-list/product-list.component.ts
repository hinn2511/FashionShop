import { CustomerSizeFilter } from './../../_models/productParams';
import { CategoryService } from 'src/app/_services/category.service';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';

import { filter } from 'rxjs/operators';
import { BreadCrumb } from 'src/app/_models/breadcrumb';
import { Pagination } from 'src/app/_models/pagination';
import { Category, Product } from 'src/app/_models/product';
import {
  CustomerFilterOrder,
  CustomerPriceRange,
  CustomerColorFilter,
  ProductParams,
} from 'src/app/_models/productParams';
import { ProductService } from 'src/app/_services/product.service';
import { Subscription } from 'rxjs';
import { Gender } from 'src/app/_models/category';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
})
export class ProductListComponent implements OnInit, OnDestroy {
  products: Product[];
  skeletonItems: number[];
  skeletonLoading: boolean = false;
  loadingCount: number = 0;
  pagination: Pagination;
  productParams: ProductParams;
  breadCrumb: BreadCrumb[];

  // filter selected value
  selectedPriceRange: CustomerPriceRange;
  selectedSize: CustomerSizeFilter;
  selectedColor: CustomerColorFilter;
  showResetPriceRangeButton: boolean = false;
  showResetSizeFilterButton: boolean = false;
  showResetColorFilterButton: boolean = false;
  selectedOrder: string;
  selectedCategory: string;
  selectedGender: number;
  selectedGenderString: string;

  querySubscribe$: Subscription;

  //filter values
  priceRanges: CustomerPriceRange[] = [
    new CustomerPriceRange(0, -1, 50),
    new CustomerPriceRange(1, 50, 100),
    new CustomerPriceRange(2, 100, 300),
    new CustomerPriceRange(3, 300, -1),
  ];

  sizeFilters: CustomerSizeFilter[] = [
    new CustomerSizeFilter(0, 'XS'),
    new CustomerSizeFilter(1, 'S'),
    new CustomerSizeFilter(2, 'M'),
    new CustomerSizeFilter(3, 'L'),
    new CustomerSizeFilter(4, 'XL'),
    new CustomerSizeFilter(5, 'XXL'),
  ];

  colorFilters: CustomerColorFilter[] = [];

  filterOrders: CustomerFilterOrder[] = [
    new CustomerFilterOrder(0, 'Name', 0, 'Name (A-Z)'),
    new CustomerFilterOrder(1, 'Name', 1, 'Name (Z-A)'),
    new CustomerFilterOrder(2, 'Price', 0, 'Price (low-high)'),
    new CustomerFilterOrder(3, 'Price', 1, 'Price (high-low)'),
    new CustomerFilterOrder(4, 'Sold', 0, 'Best seller'),
    new CustomerFilterOrder(5, 'Date', 1, 'Newest'),
  ];
  

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private route: ActivatedRoute
  ) {
    this.productParams = this.productService.getProductParams();
  }
  ngOnDestroy(): void {
    this.querySubscribe$.unsubscribe();
  }

  ngOnInit(): void {
    this.querySubscribe$ = this.route.queryParamMap
      .pipe(
        filter(
          (queryParamMap) =>
            queryParamMap.has('category') || queryParamMap.has('gender')
        )
      )
      .subscribe((queryParamMap) => {
        this.selectedCategory = queryParamMap.get('category');
        this.selectedGender = +queryParamMap.get('gender');
        this.selectedGenderString = Gender[this.selectedGender];
        if (
          this.selectedCategory != this.productParams.category ||
          this.selectedGender != this.productParams.gender
          ) {
            this.productParams.category = this.selectedCategory;
            this.productParams.gender = this.selectedGender;
            this.updateBreadCrumb();
            this.resetFilter();
            this.loadColorFilters();
            this.loadProducts();
        }
      });

    this.skeletonLoading = true;
    this.skeletonItems = Array(this.productParams.pageSize).fill(1);
    this.productParams.field = 'Sold';
    this.productParams.orderBy = 1;    
    this.selectedOrder = this.filterOrders[4].filterName;
    this.loadProducts();
    this.loadColorFilters();
  }

  private updateBreadCrumb() {
    this.breadCrumb = [];
    this.breadCrumb = [
      {
        name: 'Home',
        route: '/',
        active: false
      },
    ];
    if (this.productParams.gender != null) {
      this.breadCrumb.push({
        name: this.selectedGenderString,
        route: '',
        active: false,
      });
    }
    if (this.productParams.category != null) {
      this.breadCrumb.push({
        name: this.categoryService.getCurrentCategory(),
        route: '',
        active: false
      });
    }
  }

  loadProducts() {
    if (this.loadingCount == 0) this.skeletonLoading = true;
    // this.skeletonLoading = true;
    this.productService.setProductParams(this.productParams);

    this.productService
      .getProducts(this.productParams)
      .subscribe((response) => {
        if (this.loadingCount == 0) {
          this.loadingCount++;
          setTimeout(() => {
            this.skeletonLoading = false;
          }, 500);
          // this.skeletonLoading = false;
        }
        this.products = response.result;
        this.pagination = response.pagination;
      });
  }

  loadColorFilters()
  {
    this.productService
      .getColorFilter(this.productParams)
      .subscribe((response) => {
        this.colorFilters = response;
      });
  };


  pageChanged(event: any) {
    if (this.productParams.pageNumber !== event.page) {
      this.productParams.pageNumber = event.page;
      this.productService.setProductParams(this.productParams);
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
    this.resetPriceRange();
    this.resetSize();
    this.resetColor();
    this.productParams = this.productService.resetProductParams();
    this.productParams.category = this.selectedCategory;
    this.productParams.gender = this.selectedGender;
    this.loadProducts();
  }

  setPriceRange(priceRangeId: number) {
    if (this.selectedPriceRange != null) {
      this.selectedPriceRange = null;
      this.resetPriceRange();
      return;
    }
    var priceRange = this.priceRanges.find((x) => x.id == priceRangeId);
    this.productParams.minPrice = priceRange?.minPrice;
    this.productParams.maxPrice = priceRange?.maxPrice;
    this.selectedPriceRange = priceRange;
    this.showResetPriceRangeButton = true;
    this.loadProducts();
  }

  setSizeFilter(sizeFilterId: number) {
    if (this.selectedSize != null) {
      this.selectedSize = null;
      this.resetSize();
      return;
    }
    var size = this.sizeFilters.find((x) => x.id == sizeFilterId);
    this.selectedSize = size;
    this.productParams.size = size.sizeName;
    this.showResetSizeFilterButton = true;
    this.loadProducts();
  }

  setColorFilter(colorFilterId: number) {
    if (this.selectedColor != null) {
      this.selectedColor = null;
      this.resetColor();
      return;
    }
    var color = this.colorFilters.find((x) => x.id == colorFilterId);
    this.selectedColor = color;
    this.productParams.colorCode = color.colorCode;
    this.showResetColorFilterButton = true;
    this.loadProducts();
  }

  resetColor() {
    this.selectedColor = null;
    this.showResetColorFilterButton = false;
    this.productParams.colorCode = '';
    this.loadProducts();
  }

  resetPriceRange() {
    this.selectedPriceRange = null;
    this.showResetPriceRangeButton = false;
    this.productParams.minPrice = -1;
    this.productParams.maxPrice = -1;
    this.loadProducts();
  }

  resetSize() {
    this.selectedSize = null;
    this.showResetSizeFilterButton = false;
    this.productParams.size = '';
    this.loadProducts();
  }
}
