import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RouteService {
  private route = new BehaviorSubject<string>('');
  route$ = this.route.asObservable();

  public setRoute(value: string) {
    this.route.next(value);
  }
  public get currentRoute(): string {
    return this.route.getValue();
  }

  updateRoute(url: string) {
    if (url.includes('/administrator')) {
      this.updateAdminRoute(url);
    } else {
      if (url.includes('/login')) this.setRoute('Client Login');
      if (url.includes('/register')) this.setRoute('Client Register');
      if (url == '/')
        this.setRoute('Home');
    }

  }

  private updateAdminRoute(url: string) {
    if (url.includes('/login')) {
      this.setRoute('Administrator Login');
    }
    if (url.includes('/product')) {
      this.setRoute('Products');
    }
    if (url.includes('/order')) {
      this.setRoute('Orders');
    }
    if (url.includes('/category')) {
      this.setRoute('Categories');
    }
    if (url.includes('/option')) {
      this.setRoute('Product Options');
    }
    if (url.includes('/setting')) {
      this.setRoute('Settings');
    }
    if (url.includes('/role')) {
      this.setRoute('Roles');
    }
    if (url.includes('/user')) {
      this.setRoute('Users');
    }
    if (url.includes('/article')) {
      this.setRoute('Articles');
    }
    if (url.includes('/carousel')) {
      this.setRoute('Carousels');
    }
    if (url.includes('/dashboard')) {
      this.setRoute('Dashboard');
    }
  }
}
