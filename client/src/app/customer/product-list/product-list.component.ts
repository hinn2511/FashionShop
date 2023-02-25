import { SlideLeftToRight } from 'src/app/_common/animation/common.animation';
import { DeviceService } from 'src/app/_services/device.service';
import { ProductFilterComponent } from './../product-filter/product-filter.component';
import { CustomerSizeFilter } from './../../_models/productParams';
import { CategoryService } from 'src/app/_services/category.service';
import {
  ActivatedRoute,
} from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';

import { BreadCrumb } from 'src/app/_models/breadcrumb';
import { Pagination } from 'src/app/_models/pagination';
import { Product } from 'src/app/_models/product';
import {
  CustomerFilterOrder,
  CustomerPriceRange,
  CustomerColorFilter,
  ProductParams,
} from 'src/app/_models/productParams';
import { ProductService } from 'src/app/_services/product.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { fnGetGenderName, Category } from 'src/app/_models/category';
import { ProductFilter } from '../product-filter/product-filter.component';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
  animations: [ SlideLeftToRight ]
})
export class ProductListComponent implements OnInit, OnDestroy {
  @ViewChild('filterComponent') filterComponent: ProductFilterComponent;
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
  selectedOrder: string;
  selectedCategory: string;
  selectedGender: number;
  selectedGenderString: string;

  category: Category;

  querySubscribe$: Subscription;

  showFilterMobile: boolean = false;
  deviceSubscription$: Subscription;
  deviceType: BehaviorSubject<string> = new BehaviorSubject('');
  deviceTypeValue$ = this.deviceType.asObservable();

  count = 0;
  filterOrders: CustomerFilterOrder[] = [
    new CustomerFilterOrder(0, 'Name', 0, 'Name (A-Z)'),
    new CustomerFilterOrder(1, 'Name', 1, 'Name (Z-A)'),
    new CustomerFilterOrder(2, 'Price', 0, 'Price (low-high)'),
    new CustomerFilterOrder(3, 'Price', 1, 'Price (high-low)'),
    new CustomerFilterOrder(4, 'Sold', 0, 'Best seller'),
    new CustomerFilterOrder(5, 'Date', 1, 'Newest'),
  ];
  colorFilters: CustomerColorFilter[] = [];

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private deviceService: DeviceService
  ) {
    this.productParams = this.productService.getProductParams();
  }

  ngOnDestroy(): void {
    this.querySubscribe$.unsubscribe();
    this.deviceSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.querySubscribe$ = this.route.queryParamMap.subscribe((queryParamMap) => {
        this.selectedCategory = queryParamMap.get('category');
        this.selectedGender = +queryParamMap.get('gender');
        this.selectedGenderString = fnGetGenderName(this.selectedGender);
        this.productParams.category = this.selectedCategory;
        this.productParams.gender = this.selectedGender;
        this.loadCategory();
        this.loadColorFilters();
        this.loadProducts();
        // }
      });

    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        this.deviceType.next(this.deviceService.getDeviceType());
      }
    );

    this.skeletonItems = Array(this.productParams.pageSize).fill(1);
    this.productParams.field = 'Sold';
    this.productParams.orderBy = 1;
    this.selectedOrder = this.filterOrders[4].filterName;
  }

  private loadBreadCrumb() {
    this.breadCrumb = [];
    this.breadCrumb = [
      {
        name: 'Home',
        route: '/',
        active: true,
      },
    ];

    this.breadCrumb.push({
      name: `${this.selectedGenderString}`,
      route: ``,
      active: false,
    });

    if (this.category.parent != null) {
      this.breadCrumb.push({
        name: `${this.category.parent.categoryName}`,
        route: `/product?category=${this.category.parent.slug}&gender=${this.selectedGender}`,
        active: true,
      });
    }

    this.breadCrumb.push({
      name: `${this.category.categoryName}`,
      route: `/product?category=${this.category.slug}&gender=${this.selectedGender}`,
      active: true,
    });
  }

  loadProducts() {
    // if (this.loadingCount == 0) this.skeletonLoading = true;
    // this.skeletonLoading = true;
    this.productService.setProductParams(this.productParams);

    this.productService
      .getProducts(this.productParams)
      .subscribe((response) => {
        // if (this.loadingCount == 0) {
        //   this.loadingCount++;
        //   setTimeout(() => {
        //     this.skeletonLoading = false;
        //   }, 500);
        // this.skeletonLoading = false;
        // }
        this.products = response.result;
        this.pagination = response.pagination;
      });
  }

  loadColorFilters() {
    this.productService
      .getColorFilter(this.productParams)
      .subscribe((response) => {
        this.colorFilters = response;
      });
  }

  loadCategory() {
    this.categoryService
      .getCategoryDetail(this.productParams.category, this.productParams.gender)
      .subscribe((result) => {
        this.category = result;
        this.loadBreadCrumb();
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
    this.selectedOrder = filterOrder.filterName;
    this.productParams.orderBy = filterOrder.orderBy;
    this.productParams.field = filterOrder.field;
    this.loadProducts();
  }

  filter(productFilter: ProductFilter) {
    this.productParams = productFilter.productParams;
    this.selectedColor = productFilter.selectedColor;
    this.selectedPriceRange = productFilter.selectedPriceRange;
    this.selectedSize = productFilter.selectedSize;
    this.loadProducts();
  }

  resetColor() {
    console.log(this.filterComponent);
    
    this.filterComponent.resetColor();
  }

  resetPriceRange() {
    this.filterComponent.resetPriceRange();
  }

  resetSize() {
    this.filterComponent.resetSize();
  }

  mobileFilterToggle() {
    this.showFilterMobile = !this.showFilterMobile;
  }

  filterState()
  {
    if (this.showFilterMobile)
      return 'out';
    return 'in'
  }
}
