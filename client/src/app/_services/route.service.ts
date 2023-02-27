import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { fnHasValue } from '../_common/function/function';

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

  getReturnUrl(roles: string[]) {
    if (!fnHasValue<string[]>(roles))
      return '/';

    if (fnHasValue(roles.find((x) => x === 'AdministratorAccess'))) {
      if (fnHasValue(roles.find((x) => x === 'DashboardAccess'))) {
        return '/administrator/dashboard';
      }
      if (fnHasValue(roles.find((x) => x === 'ProductManagerAccess'))) {
        return '/administrator/product-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'OrderManagerAccess'))) {
        return '/administrator/order-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'CategoryManagerAccess'))) {
        return '/administrator/category-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'ProductOptionManagerAccess'))) {
        return '/administrator/option-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'SettingManagerAccess'))) {
        return '/administrator/setting-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'RoleManagerAccess'))) {
        return '/administrator/role-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'UserManagerAccess'))) {
        return '/administrator/user-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'ArticleManagerAccess'))) {
        return '/administrator/article-manager';
      }
      if (fnHasValue(roles.find((x) => x === 'CarouselManagerAccess'))) {
        return '/administrator/carousel-manager';
      }

    }
    return '/';
  }
}
