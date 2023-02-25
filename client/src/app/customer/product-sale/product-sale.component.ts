import { ArticleService } from 'src/app/_services/article.service';
import {
  CustomerArticle,
  CustomerArticleParams,
} from 'src/app/_models/article';
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
import { Carousel } from 'src/app/_models/carousel';

@Component({
  selector: 'app-product-sale',
  templateUrl: './product-sale.component.html',
  styleUrls: ['./product-sale.component.css'],
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
    // this.querySubscribe$.unsubscribe();
    this.deviceSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    // this.querySubscribe$ = this.route.queryParamMap.subscribe((queryParamMap) => {
    //     this.productService.productParams = new ProductParams();
    //     this.productParams.gender = undefined;
    //     this.query = queryParamMap.get('q');
    //     this.productParams.query = this.query;
    //     this.loadBreadCrumb();
    //     this.loadColorFilters();
    //     this.loadProducts();
    //     this.loadCategoryGroup();
    //     // }
    //   });

    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        this.deviceType.next(this.deviceService.getDeviceType());
      }
    );
    this.productParams = this.productService.resetProductParams();
    this.skeletonItems = Array(this.productParams.pageSize).fill(1);
    this.productParams.field = 'Sold';
    this.productParams.orderBy = 1;
    this.productParams.isOnSale = true;
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
    this.showFilterMobile = !this.showFilterMobile;
  }

  loadCategoryGroup() {
    this.categoryService.getCatalogue().subscribe((result) => {
      this.categoryGroups = result;
      console.log(result);
    });
  }
}
