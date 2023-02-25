import { SettingService } from './_services/setting.service';
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
import { Router } from '@angular/router';
import { User } from './_models/user';
import { AuthenticationService } from './_services/authentication.service';
import { debounceTime } from 'rxjs/operators';
import { CartItem } from './_models/cart';
import { Subscription, fromEvent } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  animations: [
    trigger('slideInOut', [
      state(
        'in',
        style({
          transform: 'translate3d(-240px,0,0)',
        })
      ),
      state(
        'out',
        style({
          transform: 'translate3d(0, 0, 0)',
        })
      ),
      transition('in => out', animate('400ms ease-in-out')),
      transition('out => in', animate('400ms ease-in-out')),
    ]),
  ],
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Test';
  user: User;
  focus: boolean = false;
  menuState: string = 'in';
  cartItems: CartItem[] = [];
  windowResizeSubscription$: Subscription;

  constructor(
    public authenticationService: AuthenticationService,
    private cartService: CartService,
    private settingService: SettingService,
    private router: Router,
    private deviceService: DeviceService
  ) {}

  ngOnInit(): void {
    // this.updateSetting();
    this.updateCart();
    this.updateDeviceType();
  }
  // updateSetting() {
  //   this.settingService.getSettings().subscribe((result) => {
  //     this.settingService.setSetting(result);
  //     console.log( this.settingService.settingValue);
      
  //   });
  // }

  ngOnDestroy(): void {
    this.windowResizeSubscription$.unsubscribe();
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
    if (
      localStorage.getItem('user') != null &&
      localStorage.getItem('user') != undefined
    ) {
      this.authenticationService.setUser();

      if (
        this.authenticationService.userValue.roles.find(
          (x) => x == 'Customer'
        ) != undefined
      )
        this.cartService.getUserCartItems().subscribe((_) => {});
    } else {
      this.cartService.setCart(this.cartService.getLocalCartItems());
    }
  }

  logout() {
    this.authenticationService.logout();
  }

  hasRoute(route: string) {
    return this.router.url.includes(route);
  }

  toggleSidebar() {
    if (this.menuState == 'out') this.menuState = 'in';
    else this.menuState = 'out';
  }

  setSidebar(value: string)
  {
    this.menuState = value;
  }

  focusModeToggle(value: boolean) {
    this.focus = value;
  }
}
