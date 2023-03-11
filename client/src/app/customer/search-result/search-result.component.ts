import { SlideRightToLeft, FadeInAndOut } from './../../_common/animation/common.animation';
import { SlideLeftToRight, SlideLeftToRightBoolean } from 'src/app/_common/animation/common.animation';
import { DeviceService } from 'src/app/_services/device.service';
import { ProductFilterComponent, ProductFilter } from 'src/app/customer/product-filter/product-filter.component';
import { CustomerSizeFilter } from 'src/app/_models/productParams';
import { CategoryService } from 'src/app/_services/category.service';
import {
  ActivatedRoute,
} from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';

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
import { CustomerCatalogue } from 'src/app/_models/category';


@Component({
  selector: 'app-search-result',
  templateUrl: './search-result.component.html',
  styleUrls: ['./search-result.component.css'],
  animations: [SlideLeftToRight, SlideLeftToRightBoolean, SlideRightToLeft, SlideRightToLeft, FadeInAndOut ]
})
export class SearchResultComponent implements OnInit, OnDestroy {
  @ViewChild('filterComponent') filterComponent: ProductFilterComponent;
  products: Product[];
  skeletonItems: number[];
  skeletonLoading: boolean = false;
  loadingCount: number = 0;
  pagination: Pagination;
  productParams: ProductParams;
  breadCrumb: BreadCrumb[];

  // filter selected value
  query: string;
  selectedPriceRange: CustomerPriceRange;
  selectedSize: CustomerSizeFilter;
  selectedColor: CustomerColorFilter;
  selectedOrder: string;
  selectedCategory: string;
  selectedGender: number;
  selectedGenderString: string;
  querySubscribe$: Subscription;
  categoryGroups: CustomerCatalogue[] = [];

  showFilter: boolean = false;
  deviceSubscription$: Subscription;
  deviceType: BehaviorSubject<string> = new BehaviorSubject('');
  deviceTypeValue$ = this.deviceType.asObservable();

  filterApplyCount = 0;
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

    if (this.deviceService.getDeviceType() != 'desktop')
      this.showFilter = false;
    this.querySubscribe$ = this.route.queryParamMap.subscribe((queryParamMap) => {
        this.productService.productParams = new ProductParams();
        this.productParams = this.productService.resetProductParams();
        this.productParams.gender = undefined;
        this.query = queryParamMap.get('q');
        this.productParams.query = this.query;
        this.loadBreadCrumb();
        this.loadColorFilters();
        this.loadProducts();
        this.loadCategoryGroup();
      });

    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        this.deviceType.next(this.deviceService.getDeviceType());
        if (this.deviceType.value == 'desktop')
          this.showFilter = true;
      }
    );

    this.skeletonItems = Array(this.productParams.pageSize).fill(1);
    this.productParams.field = 'Sold';
    this.productParams.orderBy = 1;
    this.selectedOrder = this.filterOrders[0].filterName;
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
      name: `Search result: "${this.query}"`,
      route: ``,
      active: false,
    });


  }

  loadProducts() {
    this.skeletonLoading = true;
    this.productService.setProductParams(this.productParams);

    this.productService
      .getProducts(this.productParams)
      .subscribe((response) => {
        this.skeletonLoading = false;
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
    if(this.deviceType.value == 'desktop')
      this.showFilter = true;
    else
      this.showFilter = false;
    this.filterApplyCounting();
    this.loadProducts();
  }

  resetColor() {
    this.filterComponent.resetColor();
  }

  resetPriceRange() {
    this.filterComponent.resetPriceRange();
  }

  resetSize() {
    this.filterComponent.resetSize();
  }

  mobileFilterToggle() {
    this.showFilter = !this.showFilter;
  }

  loadCategoryGroup() {
    this.categoryService.getCatalogue().subscribe((result) => {
      this.categoryGroups = result;
    });
  }

  filterApplyCounting() {
    this.filterApplyCount = 0;
    if (this.selectedColor != null) this.filterApplyCount++;
    if (this.selectedSize != null) this.filterApplyCount++;
    if (this.selectedPriceRange != null) this.filterApplyCount++;
  }

}
