import { ProductService } from './../../_services/product.service';
import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  AfterViewInit,
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
export class HomeInterestingComponent implements OnInit, OnDestroy {
  products: Product[] = [];
  currentValue = 0;
  maxValue = 110;
  step: number = 20;
  slideStyle: string = 'margin-left: 20;';
  deviceSubscription$: Subscription;

  @ViewChild('card', { static: false })
  cards: ElementRef;

  cardWidth: number = 0;

  constructor(
    private productService: ProductService,
    private deviceService: DeviceService
  ) {}
  ngOnDestroy(): void {
    this.deviceSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  private deviceSubscribe() {
    this.cardWidth = this.cards.nativeElement.offsetWidth;
    this.maxValue = this.cardWidth * 8;

    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        switch (this.deviceService.getDeviceType()) {
          case 'mobile': {
            this.maxValue = this.cardWidth * 6;
            this.step = this.cardWidth * 1.1;
            break;
          }
          case 'tablet': {
            this.step = this.cardWidth * 2;
            break;
          }
          default: {
            this.step = this.cardWidth * 4;
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
      setTimeout(() => {
        this.deviceSubscribe();
      }, 500);

      this.maxValue = this.cardWidth * this.products.length;
    });
  }

  // scrollLeft() {
  //   if (this.currentValue == 0) return;
  //   this.currentValue += this.step;
  //   this.slideStyle = `margin-left: ${this.currentValue}%;`;
  //   console.log(this.currentValue);
  // }

  // scrollRight() {
  //   let newValue = this.currentValue - this.step;
  //   if (newValue < -this.maxValue) return;
  //   this.currentValue -= this.step;
  //   this.slideStyle = `margin-left: ${this.currentValue}%;`;
  //   console.log(this.currentValue);
  // }

  scrollLeft() {
    if (this.currentValue == 0) return;
    this.currentValue += this.step;
    this.slideStyle = `margin-left: ${this.currentValue}px;`;
  }

  scrollRight() {
    let newValue = this.currentValue - this.step;
    if (newValue < -this.maxValue) return;
    this.currentValue -= this.step;
    this.slideStyle = `margin-left: ${this.currentValue}px;`;
  }
}
