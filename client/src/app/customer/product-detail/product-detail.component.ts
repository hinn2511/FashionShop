import { AccountService } from 'src/app/_services/account.service';
import { AuthenticationService } from './../../_services/authentication.service';
import { CartService } from 'src/app/_services/cart.service';
import { Component, HostListener, Input, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Params, Router } from '@angular/router';
import { mergeMap, switchMap, take } from 'rxjs/operators';
import { BreadCrumb } from 'src/app/_models/breadcrum';
import { Product } from 'src/app/_models/product';
import {
  CustomerOption,
  CustomerOptionColor,
  CustomerOptionSize,
} from 'src/app/_models/productOptions';
import { OptionService } from 'src/app/_services/option.service';
import { ProductService } from 'src/app/_services/product.service';
import { CartItem, NewCartItem } from 'src/app/_models/cart';
import { User } from 'src/app/_models/user';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css'],
})
export class ProductDetailComponent implements OnInit {
  product: Product;
  breadCrumb: BreadCrumb[];
  quantity: number;
  options: CustomerOption[] = [];
  sizes: CustomerOptionSize[] = [];
  selectedSize: CustomerOptionSize;
  selectedColor: CustomerOptionColor;
  user: User;

  constructor(
    private productService: ProductService,
    private optionService: OptionService,
    private cartService: CartService,
    private authenticationService: AuthenticationService,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
  }

  ngOnInit(): void {    
    this.user = this.authenticationService.userValue;
    this.loadProduct(this.route.snapshot.queryParams['id']);
    this.loadOptions(this.route.snapshot.queryParams['id']);
    this.quantity = 1;
  }

  loadProduct(id: number) {
    this.productService.getProduct(id).subscribe((response) => {
      this.product = response;
      this.setBreadcrumb();
    });
    this.loadOptions(id);
  }

  setBreadcrumb() {
    this.breadCrumb = [
      {
        name: 'Home',
        route: '/',
      },
      {
        name: this.product.categoryName,
        route: '',
      },
      {
        name: this.product.productName,
        route: '',
      },
    ];
    if (this.product.subCategoryName != undefined) {
      let subCategoryBreadcrum: BreadCrumb = {
        name: this.product.subCategoryName,
        route: '',
      };
      this.breadCrumb.splice(2, 0, subCategoryBreadcrum);
    }
  }

  chooseColor(color: CustomerOptionColor) {
    this.selectedColor = color;
    this.sizes = this.options.filter(
      (x) => x.color.colorCode == color.colorCode
    )[0].sizes;
    this.selectedSize = this.sizes[0];
  }

  onSizeChange(size: CustomerOptionSize) {
    this.selectedSize = size;
  }

  loadOptions(productId: number) {
    this.optionService
      .getCustomerProductOption(productId)
      .subscribe((result) => {
        this.options = result;
        this.chooseColor(this.options[0].color);
      });
  }

  increaseQuantity() {
    if (this.quantity < 99) this.quantity++;
  }

  decreaseQuantity() {
    if (this.quantity > 1) this.quantity--;
  }

  getColorStyle(color: CustomerOptionColor) {
    return 'background-color: ' + color.colorCode;
  }

  addToCart() {
    if (this.user == null || this.user == undefined) {
      let cartItem: CartItem = {
        sizeName: this.selectedSize.sizeName,
        colorCode: this.selectedColor.colorCode,
        colorName: this.selectedColor.colorName,
        optionId: this.selectedSize.optionId,
        price: this.product.price,
        additionalPrice: this.selectedSize.additionalPrice,
        productName: this.product.productName,
        slug: this.product.slug,
        id: 0,
        productId: this.product.id,
        quantity: this.quantity,
        totalItemPrice:
          (this.product.price + this.selectedSize.additionalPrice) *
          this.quantity,
        imageUrl: this.product.url,
      };
      this.cartService.updateLocalCart(cartItem);
      this.toastr.success('This item has been added to your cart!', 'Success');
      return;
    }
    
    let newCartItem: NewCartItem = {
      quantity: this.quantity,
      optionId: this.selectedSize.optionId,
    };
    this.cartService.addToCart(newCartItem).subscribe((result) => {
      this.toastr.success('This item has been added to your cart!', 'Success');
    },
    error => {
      this.toastr.error('Something wrong happen!', 'Error');
    });
  }

  likeProduct()
  {
    if(this.user == null || this.user == undefined)
    {
      this.router.navigateByUrl('/login');
    }
    this.accountService.addToFavorite(this.product.id).subscribe(result => {
        //add notification
        this.accountService.clearFavoriteCache();
        this.product.likedByUser = true;
        this.productService.removeCache();
        this.toastr.success("This product have been added to your favorites", "Success");
    },
    error => {
      this.toastr.error(error, 'Error');
    });
  }

  unlikeProduct()
  {
    this.accountService.removeFromFavorite(this.product.id).subscribe(result => {
        //add notification
        this.accountService.clearFavoriteCache();
        this.product.likedByUser = false;
        this.productService.removeCache();
        this.toastr.success("This product have been removed from your favorites", "Success");
    },
    error => {
      this.toastr.error(error, 'Error');
    });
  }
}
