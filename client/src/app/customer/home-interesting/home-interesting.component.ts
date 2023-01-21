import { ProductService } from './../../_services/product.service';
import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
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

  @ViewChildren('card') items: QueryList<ElementRef>;
  productCards: ElementRef[] = [];

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
        switch (this.deviceService.getDeviceType()) {
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
    let productParams: ProductParams = {
      category: 'shoe',
      colorCode: '',
      size: '',
      gender: 0,
      brand: '',
      pageNumber: 1,
      pageSize: 8,
      orderBy: 1,
      field: 'Sold',
      query: '',
      minPrice: 0,
      maxPrice: 0,
    };
    this.productService.getProducts(productParams).subscribe((response) => {
      this.products = response.result;

      // this.maxValue = this.cardWidth * this.products.length;
    });
  }

  scrollLeft() {
    if (this.currentStep == 0) return;
    setTimeout(() => {
      if (this.productCards.length != 0) {
        this.currentStep -= this.step;
        if (this.currentStep < 0)
          this.currentStep = 0;
        this.productCards[this.currentStep].nativeElement.scrollIntoView({
          behavior: 'smooth',
          block: 'nearest',
          inline: 'center',
        });
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
        this.productCards[this.currentStep].nativeElement.scrollIntoView({
          behavior: 'smooth',
          block: 'nearest',
          inline: 'center',
        });
      }
    }, 100);
  }
}
