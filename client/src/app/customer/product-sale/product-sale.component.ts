import { SlideRightToLeft } from './../../_common/animation/common.animation';
import { SlideLeftToRight, SlideLeftToRightBoolean } from 'src/app/_common/animation/common.animation';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, BehaviorSubject } from 'rxjs';
import { BreadCrumb } from 'src/app/_models/breadcrumb';
import { CustomerCatalogue } from 'src/app/_models/category';
import { Pagination } from 'src/app/_models/pagination';
import { Product } from 'src/app/_models/product';
import {
  ProductParams,
  CustomerPriceRange,
  CustomerSizeFilter,
  CustomerColorFilter,
  CustomerFilterOrder,
} from 'src/app/_models/productParams';
import { CategoryService } from 'src/app/_services/category.service';
import { DeviceService } from 'src/app/_services/device.service';
import { ProductService } from 'src/app/_services/product.service';
import {
  ProductFilterComponent,
  ProductFilter,
} from '../product-filter/product-filter.component';

@Component({
  selector: 'app-product-sale',
  templateUrl: './product-sale.component.html',
  styleUrls: ['./product-sale.component.css'],
  animations: [SlideLeftToRightBoolean, SlideRightToLeft]
})
export class ProductSaleComponent implements OnInit, OnDestroy {
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
  // querySubscribe$: Subscription;
  categoryGroups: CustomerCatalogue[] = [];

  showFilter: boolean = true;
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
    private deviceService: DeviceService
  ) {
    this.productParams = this.productService.getProductParams();
  }
  ngOnDestroy(): void {
    this.deviceSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    if (this.deviceService.getDeviceType() != 'desktop')
      this.showFilter = false;

    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        this.deviceType.next(this.deviceService.getDeviceType());
        if (this.deviceType.value == 'desktop')
          this.showFilter = true;
      }
    );
    this.productParams = this.productService.resetProductParams();
    this.skeletonItems = Array(this.productParams.pageSize).fill(1);
    this.productParams.field = 'Sold';
    this.productParams.orderBy = 1;
    this.productParams.isOnSale = true;
    this.productParams.categoryId = -1;
    this.productParams.gender = null;
    this.selectedOrder = this.filterOrders[0].filterName;
    this.loadBreadCrumb();
    this.loadColorFilters();
    this.loadProducts();
    this.loadCategoryGroup();
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
      name: `Sale off`,
      route: ``,
      active: false,
    });
  }

  loadProducts() {
    this.productService.setProductParams(this.productParams);

    this.productService
      .getProducts(this.productParams)
      .subscribe((response) => {
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
    if (this.selectedCategory != null) this.filterApplyCount++;
    if (this.selectedPriceRange != null) this.filterApplyCount++;
  }
}
