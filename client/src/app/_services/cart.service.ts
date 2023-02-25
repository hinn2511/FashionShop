import {
  CartItem,
  CartItemList,
  UpdateCartItem,
  UpdateCartItemAfterLogin,
} from 'src/app/_models/cart';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { NewCartItem } from 'src/app/_models/cart';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baseUrl = environment.apiUrl;
  private cartSubject = new BehaviorSubject<CartItemList>({
    cartItems: [],
    totalItem: 0,
    totalPrice: 0,
  });
  currentCarts$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {}

  public setCart(cart: CartItemList) {
    this.cartSubject.next(cart);
  }

  public get cartValue(): CartItemList {
    return this.cartSubject.getValue();
  }

  public getSelectedCartId(): number {
    return +localStorage.getItem('selectedCartId');
  }

  public setSelectedCartId(value: number) {
    localStorage.setItem('selectedCartId', value.toString());
  }

  public removeSelectedCartId() {
    localStorage.removeItem('selectedCartId');
  }

  getUserCartItems() {
      let localCart = this.getLocalCartItems().cartItems;
      if (localCart.length > 0) {
        let items: NewCartItem[] = [];
        localCart.forEach((item) => {
          items.push({ optionId: item.optionId, quantity: item.quantity });
        });
        let body: UpdateCartItemAfterLogin = {
          newCartItems: items,
        };
        return this.http.put(this.baseUrl + 'user/cart-after-login', body).pipe(
          map((response: CartItemList) => {
            if (response) {
              this.clearCart();
              this.clearLocalCart();
              this.cartSubject.next(response);
              return response;
            }
          })
        );
    }
    return this.http.get<CartItemList>(this.baseUrl + 'user/cart').pipe(
      map((response: CartItemList) => {
        if (response) {
          this.clearCart();
          this.clearLocalCart();
          this.cartSubject.next(response);
          return response;
        }
      })
    );
  }

  addToCart(cartItem: NewCartItem) {
    return this.http.post(this.baseUrl + 'user/cart', cartItem).pipe(
      map((response: CartItemList) => {
        if (response) {
          this.cartSubject.next(response);
          return response;
        }
      })
    );
  }

  updateCart(cartItem: UpdateCartItem) {
    return this.http.put(this.baseUrl + 'user/cart', cartItem).pipe(
      map((response: CartItemList) => {
        if (response) {
          this.cartSubject.next(response);
          return response;
        }
      })
    );
  }

  deleteCart(cartId: number) {
    return this.http.delete(this.baseUrl + 'user/cart/' + cartId).pipe(
      map((response: CartItemList) => {
        if (response) {
          this.cartSubject.next(response);
          return response;
        }
      })
    );
  }

  clearCart() {
    this.cartSubject.next({ cartItems: [], totalItem: 0, totalPrice: 0 });
  }

  getLocalCartItems() {
    let cartItemList: CartItemList = {totalItem: 0 , totalPrice: 0, cartItems: []};
    let cart = localStorage.getItem('userCart');
    if (cart != null || cart != undefined) {
      cartItemList = JSON.parse(cart);
    }
    return cartItemList;
  }

  updateLocalCart(cartItem: CartItem) {
    let cart = localStorage.getItem('userCart');
    let cartItemList: CartItemList = {
      cartItems: [],
      totalItem: 0,
      totalPrice: 0,
    };
    // check if cart exist
    if (cart != null || cart != undefined) {
      cartItemList = JSON.parse(cart);

      // check if item exist in cart
      let cartItemIndex = cartItemList.cartItems.findIndex(
        (x) => x.optionId == cartItem.optionId
      );
      // not exist
      if (cartItemIndex < 0) cartItemList.cartItems.push(cartItem);
      // exist
      else cartItemList.cartItems[cartItemIndex].quantity = cartItem.quantity;
    }
    // force push if cart empty
    else {
      cartItemList.cartItems.push(cartItem);
    }
    // calculate total item and price
    cartItemList.totalItem = cartItemList.cartItems.reduce(
      (a, b) => a + b.quantity,
      0
    );
    cartItemList.totalPrice = cartItemList.cartItems.reduce(
      (a, b) => a + (b.price + b.additionalPrice) * b.quantity,
      0
    );

    // update cart
    this.cartSubject.next(cartItemList);
    console.log(cartItemList);    
    localStorage.setItem('userCart', JSON.stringify(cartItemList));

    return true;
  }

  deleteLocalCartItem(optionId: number) {
    let cart = localStorage.getItem('userCart');
    let cartItemList: CartItemList = {
      cartItems: [],
      totalItem: 0,
      totalPrice: 0,
    };
    if (cart != null || cart != undefined) {
      cartItemList = JSON.parse(cart);
      let cartItemIndex = cartItemList.cartItems.findIndex(
        (x) => x.optionId == optionId
      );
      if (cartItemIndex >= 0) {
        cartItemList.cartItems.splice(cartItemIndex, 1);
      }
      if (cartItemList.cartItems.length == 0) {
        localStorage.removeItem('userCart');
      }
    }

    cartItemList.totalItem = cartItemList.cartItems.reduce(
      (a, b) => a + b.quantity,
      0
    );
    cartItemList.totalPrice = cartItemList.cartItems.reduce(
      (a, b) => a + (b.price + b.additionalPrice) * b.quantity,
      0
    );
    this.cartSubject.next(cartItemList);
    localStorage.setItem('userCart', JSON.stringify(cartItemList));
  }

  clearLocalCart() {
    localStorage.removeItem('userCart');
  }
}
