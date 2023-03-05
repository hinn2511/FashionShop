import { FadeInAndOut, SlideInOut, SlideTopToBottom } from './_common/animation/common.animation';
import { RouteService } from './_services/route.service';
import { DeviceService } from 'src/app/_services/device.service';
import { CartService } from 'src/app/_services/cart.service';
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { User } from './_models/user';
import { AuthenticationService } from './_services/authentication.service';
import { debounceTime, filter } from 'rxjs/operators';
import { CartItem } from './_models/cart';
import { Subscription, fromEvent } from 'rxjs';
import { fnIsNullOrEmpty, fnSwitchValue } from './_common/function/function';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  animations: [
    SlideInOut,
    FadeInAndOut
  ],
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Thyme';
  user: User;
  focus: boolean = false;
  sidebarState: string = 'in';
  cartItems: CartItem[] = [];
  windowResizeSubscription$: Subscription;
  routeSubscription$: Subscription;

  currentRouteSubscription$: Subscription;

  currentRoute: string = '';

  constructor(
    public authenticationService: AuthenticationService,
    private cartService: CartService,
    private routeService: RouteService,
    private router: Router,
    private deviceService: DeviceService
  ) {
    this.routeSubscribe();
  }

  ngOnInit(): void {
    this.updateCart();
    this.updateDeviceType();
    this.currentRouteSubscribe();
  }

  ngOnDestroy(): void {
    this.windowResizeSubscription$.unsubscribe();
    this.routeSubscription$.unsubscribe();
    this.currentRouteSubscription$.unsubscribe();
  }

  routeSubscribe() {
    this.routeSubscription$ = this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event) => {
        this.routeService.updateRoute(this.router.url);
      });
  }

  currentRouteSubscribe() {    this.routeSubscription$ = this.routeService.route$.subscribe((result) => {
      this.currentRoute = result;
    })
  }

  private updateDeviceType() {
    this.deviceService.setWidth(window.innerWidth);
    this.deviceService.setHeight(window.innerHeight);

    this.windowResizeSubscription$ = fromEvent(window, 'resize')
      .pipe(debounceTime(500))
      .subscribe((_) => {
        this.deviceService.setWidth(window.innerWidth);
        this.deviceService.setHeight(window.innerHeight);
      });
  }

  private updateCart() {
    if (!fnIsNullOrEmpty(localStorage.getItem('user'))) {
      this.authenticationService.setUser();
      let role = this.authenticationService.userValue.roles.find(
        (x) => x == 'ClientAccess'
      );
      if (!fnIsNullOrEmpty(role))
        this.cartService.getUserCartItems().subscribe((_) => {});
    } else {
      this.cartService.setCart(this.cartService.getLocalCartItems());
    }
  }

  logout() {
    this.authenticationService.logout();
  }



  toggleSidebar() {
    fnSwitchValue<string>(this.sidebarState, 'out', 'in');
  }

  setSidebar(value: string) {
    this.sidebarState = value;
  }

  setFocus(value: boolean) {
    this.focus = value;
  }
}
