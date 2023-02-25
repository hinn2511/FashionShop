import { ProductService } from 'src/app/_services/product.service';
import {
  Component,
  OnInit,
  OnDestroy,
  ElementRef,
  AfterViewInit,
  ViewChildren,
  QueryList,
} from '@angular/core';
import { Product } from 'src/app/_models/product';
import { ProductParams } from 'src/app/_models/productParams';
import { Subscription } from 'rxjs';
import { DeviceService } from 'src/app/_services/device.service';

@Component({
  selector: 'app-home-interesting',
  templateUrl: './home-interesting.component.html',
  styleUrls: ['./home-interesting.component.css'],
})
export class HomeInterestingComponent
  implements OnInit, OnDestroy, AfterViewInit
{
  products: Product[];
  currentValue = 0;
  maxValue = 110;
  step: number = 1;
  currentStep: number = 0;
  slideStyle: string = 'margin-left: 20;';
  deviceSubscription$: Subscription;
  scrollSubscription$: Subscription;

  deviceType: string = 'desktop';

  productParams: ProductParams = {
    category: '',
    colorCode: '',
    size: '',
    gender: 0,
    brand: '',
    pageNumber: 1,
    pageSize: 8,
    orderBy: 1,
    field: '',
    query: '',
    minPrice: 0,
    maxPrice: 0,
    isOnSale: false,
    isFeatured: true,
    isMostInteresting: false,
  };

  @ViewChildren('card') items: QueryList<ElementRef>;
  productCards: ElementRef[] = [];
  filter: string = 'featured';

  constructor(
    private productService: ProductService,
    private deviceService: DeviceService
  ) {}
  ngOnDestroy(): void {
    this.deviceSubscription$.unsubscribe();
    this.scrollSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.loadProducts();
    this.deviceSubscribe();
  }

  ngAfterViewInit() {
    this.scrollSubscription$ = this.items.changes.subscribe(() => {
      this.productCards = this.items.toArray();
    });
  }

  private deviceSubscribe() {
    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        this.deviceType = this.deviceService.getDeviceType();
        switch (this.deviceType) {
          case 'mobile': {
            this.step = 1;
            break;
          }
          case 'tablet': {
            this.step = 3;
            break;
          }
          default: {
            this.step = 4;
            break;
          }
        }
      }
    );
  }

  loadProducts() {
    this.productService
      .getProducts(this.productParams)
      .subscribe((response) => {
        this.products = response.result;
      });
  };

  scrollLeft() {
    if (this.currentStep == 0) return;
    setTimeout(() => {
      if (this.productCards.length != 0) {
        this.currentStep -= this.step;
        if (this.currentStep < 0) this.currentStep = 0;
        this.scrollToView();
      }
    }, 100);
  }

 

  scrollRight() {
    if (this.currentStep >= this.products.length - 1) return;
    setTimeout(() => {
      if (this.productCards.length != 0) {
        this.currentStep += this.step;
        if (this.currentStep > this.products.length - 1)
          this.currentStep = this.products.length - 1;
        this.scrollToView();
      }
    }, 100);
  }

  private scrollToView() {
    this.productCards[this.currentStep].nativeElement.scrollIntoView({
      behavior: 'smooth',
      block: 'nearest',
      inline: 'center',
    });
  }

  applyFilter(filter: string) {
    this.filter = filter;
    this.productParams.isFeatured = false;
    this.productParams.isMostInteresting = false;
    this.productParams.isOnSale = false;
    switch (filter) {
      case 'featured':
        this.productParams.isFeatured = true;
        break;
      case 'onSale':
        this.productParams.isOnSale = true;
        break;
      default:
        this.productParams.isMostInteresting = true;
        break;
    }
    this.loadProducts();
  }
}
