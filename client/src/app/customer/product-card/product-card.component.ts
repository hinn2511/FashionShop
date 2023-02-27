import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/user';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from 'src/app/_models/product';
import { AccountService } from 'src/app/_services/account.service';
import { ProductService } from 'src/app/_services/product.service';
import { fnCalculatePrice } from 'src/app/_common/function/function';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.css'],
})
export class ProductCardComponent implements OnInit {
  @Input() product: Product;
  @Output() update = new EventEmitter<boolean>();
  shadow: string = 'shadow-sm';
  user: User;

  hover($event) {
    this.shadow = $event.type == 'mouseover' ? 'shadow' : 'shadow-sm';
  }

  constructor(
    private router: Router,
    private accountService: AccountService,
    private productService: ProductService,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.user = this.authenticationService.userValue;
  }

  viewDetail() {
    this.router.navigate([`product/${this.product.slug}`], {
      queryParams: { id: this.product.id },
    });
  }

  likeProduct() {
    if (this.user == null || this.user == undefined) {
      this.router.navigateByUrl('/login');
    }
    this.accountService.addToFavorite(this.product.id).subscribe(
      (result) => {
        //add notification
        this.accountService.clearFavoriteCache();
        this.product.likedByUser = true;
        this.update.emit(true);
        this.productService.removeCache();
        this.toastr.success(
          'This product have been added to your favorites',
          'Success'
        );
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  unlikeProduct() {
    this.accountService.removeFromFavorite(this.product.id).subscribe(
      (result) => {
        //add notification
        this.accountService.clearFavoriteCache();
        this.product.likedByUser = false;
        this.update.emit(true);
        this.productService.removeCache();
        this.toastr.success(
          'This product have been removed from your favorites',
          'Success'
        );
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  calculatePrice(
    saleType: number,
    price: number,
    saleOffPercent: number,
    saleOffValue: number
  ) {
    return fnCalculatePrice(saleType, price, saleOffPercent, saleOffValue);
  }
}
