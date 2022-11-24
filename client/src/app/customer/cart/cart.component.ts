import { AccountService } from 'src/app/_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CartItem, UpdateCartItem } from 'src/app/_models/cart';
import { User } from 'src/app/_models/user';
import { CartService } from 'src/app/_services/cart.service';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
})
export class CartComponent implements OnInit {
  @Output() hideCart = new EventEmitter<boolean>();
  user: User;
  discountAmount: number = 0;
  promoCode: string = '';
  promoCodeApplied: boolean = false;

  constructor(
    private authenticationService: AuthenticationService,
    public cartService: CartService,
    private accountService: AccountService,
    private productService: ProductService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.user = this.authenticationService.userValue;
  }

  applyDiscountCode() {
    this.toastr.success('Discount code has been applied', 'Success', {
      positionClass: 'toast-top-right',
    });
    this.promoCodeApplied = true;
  }

  removeDiscountCode() {
    this.promoCodeApplied = false;
    this.promoCode = '';
  }

  increaseQuantity(cartItem: CartItem) {
    if (cartItem.quantity < 99) {
      if (this.user == null || this.user == undefined) {
        ++cartItem.quantity;
        this.cartService.updateLocalCart(cartItem);
        return;
      }
      let updatedCart: UpdateCartItem = {
        cartId: cartItem.id,
        quantity: ++cartItem.quantity,
      };
      this.cartService.updateCart(updatedCart).subscribe((_) => {});
    }
  }

  decreaseQuantity(cartItem: CartItem) {
    if (cartItem.quantity > 1) {
      if (this.user == null || this.user == undefined) {
        --cartItem.quantity;
        this.cartService.updateLocalCart(cartItem);
        return;
      }
      let updatedCart: UpdateCartItem = {
        cartId: cartItem.id,
        quantity: --cartItem.quantity,
      };
      this.cartService.updateCart(updatedCart).subscribe((_) => {});
    } else this.deleteCartItem(cartItem);
  }

  deleteCartItem(cartItem: CartItem) {
    let success = true;
    if (this.user == null || this.user == undefined) {
      this.cartService.deleteLocalCartItem(cartItem.optionId);
      success = true;
    } else
      this.cartService.deleteCart(cartItem.id).subscribe(
        (_) => {},
        (error) => {
          success = false;
        }
      );
    if (success)
      this.toastr.success(
        'This item has been removed from your cart!',
        'Success'
      );
    else this.toastr.error('Something wrong happen!', 'Error');
  }

  viewProduct(cartItem: CartItem) {
    this.closeCart();
    this.router.navigate(['product/' + cartItem.slug], {
      queryParams: { id: cartItem.productId },
    });
  }

  likeProduct(cartItem: CartItem) {
    if (this.user == null || this.user == undefined) {
      this.router.navigateByUrl('/login');
    }
    this.accountService.addToFavorite(cartItem.productId).subscribe(
      (result) => {
        this.accountService.clearFavoriteCache();
        this.productService.removeCache();
        this.toastr.success(
          'This product have been added to your favorites',
          'Success'
        );
      },
      (error) => {
        this.toastr.error('This product have been liked by you', 'Error');
      }
    );
  }

  closeCart() {
    this.hideCart.emit(true);
  }
}
