import { CartItem, CartItemList, UpdateCartItem, UpdateCartItemAfterLogin } from 'src/app/_models/cart';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IdArray } from 'src/app/_models/adminRequest';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { NewCartItem } from 'src/app/_models/cart';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baseUrl = environment.apiUrl;
  private cartSubject = new BehaviorSubject<CartItemList>({cartItems: [], totalItem: 0, totalPrice: 0});
  currentCarts$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {
  }

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


  getAuthenticatedUserCartItems() {
    let cart = localStorage.getItem("userCart");
    if(cart != null || cart != undefined)
    {
      let items: NewCartItem[] = [];
      this.getLocalCartItems().cartItems.forEach(x => {
        items.push({optionId: x.optionId, quantity: x.quantity});
      });
      let body: UpdateCartItemAfterLogin = {
        newCartItems: items
      };
      return this.http.put(this.baseUrl + 'user/cart-after-login', body).pipe(
        map((response: CartItemList) => {       
          if (response){
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
        if (response){
          this.clearCart();
          this.clearLocalCart();
          this.cartSubject.next(response);              
          return response;
        }
      })
    )
  }

  addToCart(cartItem: NewCartItem) {
    return this.http.post(this.baseUrl + 'user/cart', cartItem).pipe(
      map((response: CartItemList) => {      
        if (response){
          this.cartSubject.next(response);   
          return response;
        }
      })
    );
  } 

  updateCart(cartItem: UpdateCartItem) {
    return this.http.put(this.baseUrl + 'user/cart', cartItem).pipe(
      map((response: CartItemList) => {     
        if (response){
          this.cartSubject.next(response);   
          return response;
        }
      })
    );
  }

  deleteCart(cartId: number) {
    return this.http.delete(this.baseUrl + 'user/cart/' + cartId).pipe(
      map((response: CartItemList) => {       
        if (response){
          this.cartSubject.next(response);   
          return response;
        }
      })
    );
  }

  clearCart()
  {
    this.cartSubject.next({cartItems: [], totalItem: 0, totalPrice: 0});
  }

  getLocalCartItems() {
    let cart = localStorage.getItem("userCart");
    if(cart != null || cart != undefined)
    {
      let cartItemList: CartItemList =  JSON.parse(cart);
      this.cartSubject.next(cartItemList);    
      return cartItemList;
    }
  }

  updateLocalCart(cartItem: CartItem) {
    let cart = localStorage.getItem("userCart");
    let cartItemList: CartItemList = {
      cartItems: [],
      totalItem: 0,
      totalPrice: 0
    };
    // check if cart exist
    if(cart != null || cart != undefined)
    {
      cartItemList = JSON.parse(cart);
      
      // check if item exist in cart
      let cartItemIndex = cartItemList.cartItems.findIndex(x => x.optionId == cartItem.optionId);
      // not exist
      if (cartItemIndex < 0)
        cartItemList.cartItems.push(cartItem);
      // exist
      else
        cartItemList.cartItems[cartItemIndex].quantity = cartItem.quantity;
    }
    // force push if cart empty
    else
    {
      cartItemList.cartItems.push(cartItem);
    }
    // calculate total item and price
    cartItemList.totalItem = cartItemList.cartItems.reduce((a, b) => a + b.quantity, 0);   
    cartItemList.totalPrice = cartItemList.cartItems.reduce((a, b) => a + (b.price + b.additionalPrice) * b.quantity, 0);

    // update cart
    this.cartSubject.next(cartItemList);  
    localStorage.setItem("userCart", JSON.stringify(cartItemList));

    return true;
  }

  deleteLocalCartItem(optionId: number) {
    let cart = localStorage.getItem("userCart");
    let cartItemList: CartItemList = {
      cartItems: [],
      totalItem: 0,
      totalPrice: 0
    };
    if(cart != null || cart != undefined)
    {
      cartItemList =  JSON.parse(cart);
      let cartItemIndex = cartItemList.cartItems.findIndex(x => x.optionId == optionId);
      if (cartItemIndex >= 0)
      {
        cartItemList.cartItems.splice(cartItemIndex, 1);
      }
      if (cartItemList.cartItems.length == 0)
      {
        localStorage.removeItem("userCart");
      }
    }

    cartItemList.totalItem = cartItemList.cartItems.reduce((a, b) => a + b.quantity, 0);   
    cartItemList.totalPrice = cartItemList.cartItems.reduce((a, b) => a + (b.price + b.additionalPrice) * b.quantity, 0);
    this.cartSubject.next(cartItemList);  
    localStorage.setItem("userCart", JSON.stringify(cartItemList));
  }

  clearLocalCart()
  {
    localStorage.removeItem("userCart");
    this.clearCart();
  }

  // getCartParams() {
  //   return this.managerCartParams;
  // }

  // setCartParams(params: ManagerCartParams) {
  //   this.managerCartParams = params;
  // }

  // resetCartParams() {
  //   this.managerCartParams = new ManagerCartParams();
  //   return this.managerCartParams;
  // }

  // getManagerCarts(optionParams: ManagerCartParams) {
  //   let params = getPaginationHeaders(optionParams.pageNumber, optionParams.pageSize);
  //   params = params.append('orderBy', optionParams.orderBy);
  //   params = params.append('field', optionParams.field);
  //   params = params.append('query', optionParams.query);
  //   optionParams.productCartStatus.forEach(element => {
  //     params = params.append('productCartStatus', element);
  //   });

  //   return getPaginatedResult<ManagerCart[]>(this.baseUrl + 'productCart/all', params, this.http);
  // }

  // getManagerCart(id: number) {
  //   return this.http.get<ManagerCart>(this.baseUrl + 'productCart/' + id + '/detail');
  // }

  // addCart(option: CreateCart) {
  //   return this.http.post<CreateCart>(this.baseUrl + 'productCart/create', option);
  // }

  

  // hideCarts(ids: IdArray) {
  //   return this.http.put(this.baseUrl + 'productCart/hide-or-unhide', ids);
  // }

  // deleteCart(ids: number[]) {
  //   const options = {
  //     headers: new HttpHeaders({
  //       'Content-Type': 'application/json',
  //     }),
  //     body: {
  //       ids
  //     },
  //   };
  //   return this.http.delete(this.baseUrl + 'productCart/soft-delete', options);
  // }

  // getManagerColorCart() {
  //   return this.http.get<ManagerCartColor[]>(this.baseUrl + 'color/all');
  // }

  // getManagerSizeCart() {
  //   return this.http.get<ManagerCartSize[]>(this.baseUrl + 'size/all');
  // }

  // getCustomerProductCart(productId: number) {
  //   return this.http.get<CustomerCart[]>(this.baseUrl + 'productCart/' + productId);
  // }
}
