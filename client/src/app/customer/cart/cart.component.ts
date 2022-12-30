import { DeviceService } from 'src/app/_services/device.service';
import { AccountService } from 'src/app/_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  OnDestroy,
} from '@angular/core';
import { CartItem, UpdateCartItem } from 'src/app/_models/cart';
import { User } from 'src/app/_models/user';
import { CartService } from 'src/app/_services/cart.service';
import { ProductService } from 'src/app/_services/product.service';
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
  animations: [
    trigger('rotatedState', [
      state('true', style({ transform: 'rotate(0)' })),
      state('false', style({ transform: 'rotate(-180deg)' })),
      transition('false => true', animate('500ms ease-out')),
      transition('true => false', animate('500ms ease-in')),
    ]),
  ],
})
export class CartComponent implements OnInit, OnDestroy {
  @Output() hideCart = new EventEmitter<boolean>();
  @Output() goToCheckout = new EventEmitter<boolean>();
  user: User;
  discountAmount: number = 0;
  promoCode: string = '';
  promoCodeApplied: boolean = false;
  expandCartSummary: boolean = false;
  deviceType: string = '';
  deviceSubscription$: Subscription;

  constructor(
    private authenticationService: AuthenticationService,
    public cartService: CartService,
    private accountService: AccountService,
    private productService: ProductService,
    private toastr: ToastrService,
    private router: Router,
    private deviceService: DeviceService
  ) {}
  ngOnDestroy(): void {
    this.deviceSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        this.deviceType = this.deviceService.getDeviceType();
        if (this.deviceType == 'desktop') {
          this.expandCartSummary = true;
        }
      }
    );
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

  expandCartSummaryToggle() {
    this.expandCartSummary = !this.expandCartSummary;
  }

  increaseQuantity(cartItem: CartItem) {
    if (cartItem.quantity > 98) return;
    cartItem.quantity++;
  }

  decreaseQuantity(cartItem: CartItem) {
    if (cartItem.quantity == 1) {
      this.deleteCartItem(cartItem);
      return;
    }
    if (cartItem.quantity < 1) return;
    cartItem.quantity--;
  }

  deleteCartItem(cartItem: CartItem) {
    let success = true;
    if (this.userIsNotLoggedIn()) {
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

  updateCart(cartItem: CartItem) {
    if (cartItem.quantity < 0 && cartItem.quantity > 99) return;

    if (this.userIsNotLoggedIn()) {
      this.cartService.updateLocalCart(cartItem);
      return;
    }

    let updatedCart: UpdateCartItem = {
      cartId: cartItem.id,
      quantity: cartItem.quantity,
    };

    this.cartService.updateCart(updatedCart).subscribe((_) => {});
  }

  private userIsNotLoggedIn() {
    return this.user == null || this.user == undefined;
  }

  viewProduct(cartItem: CartItem) {
    this.router.navigate(['product/' + cartItem.slug], {
      queryParams: { id: cartItem.productId },
    });
    this.closeCart();
  }

  likeProduct(cartItem: CartItem) {
    if (this.userIsNotLoggedIn()) {
      this.closeCart();
      this.router.navigateByUrl('/login');
      return;
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

  goToCheckoutWindow()
  {
    if (this.userIsNotLoggedIn()) {
      this.router.navigateByUrl('login');
    };
    this.goToCheckout.emit(true);
  }
}
