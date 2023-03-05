import { SlideRightToLeft, SlideTopToBottom, SlideLeftToRight, FadeInAndOut, SlideTopToBottomBoolean, Fade, ExpandTopToBottom } from './../../_common/animation/common.animation';
import { CartService } from 'src/app/_services/cart.service';
import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  HostListener,
  ElementRef,
  OnDestroy,
  ViewChild,
  AfterViewInit,
} from '@angular/core';
import { NavigationStart, Router } from '@angular/router';

import { AuthenticationService } from 'src/app/_services/authentication.service';
import { CategoryService } from 'src/app/_services/category.service';
import { User } from 'src/app/_models/user';
import { DeviceService } from 'src/app/_services/device.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { CustomerCatalogue, CustomerCategoryCatalogue } from 'src/app/_models/category';
import { RouteService } from 'src/app/_services/route.service';

export class NavSettings {
  navHeight: string;
  navMargin: string;
  deviceType: string;

  constructor(navHeight: string, navMargin: string, deviceType: string) {
    this.navHeight = navHeight;
    this.navMargin = navMargin;
    this.deviceType = deviceType;
  }

}

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css'],
  animations: [ SlideRightToLeft, SlideTopToBottom, SlideTopToBottomBoolean, SlideLeftToRight, FadeInAndOut, Fade, ExpandTopToBottom ]
})
export class NavigationBarComponent
  implements OnInit, OnDestroy, AfterViewInit
{

  @Output() focus = new EventEmitter<boolean>();
  @ViewChild('nav') navElement: ElementRef;
  searchQuery: string = '';
  icon: string;
  action: string;
  collapseSearchWindow: boolean = true;
  collapseSearchBar: boolean = true;
  collapseCategory: boolean = true;
  collapseCartWindow: boolean = true;
  collapseCheckoutWindow: boolean = true;
  hideCategoryGroupDetail: boolean = true;
  categoryGroups: CustomerCatalogue[] = [];
  selectedCategoryGroup:  CustomerCatalogue;
  categories:  CustomerCategoryCatalogue[] = [];
  selectedCategory:  CustomerCategoryCatalogue;
  selectedSubCategory:  CustomerCategoryCatalogue;
  user: User;
  settings: BehaviorSubject<NavSettings> = new BehaviorSubject(
    new NavSettings('', '', '')
  );  
  settingValue$ = this.settings.asObservable();
  deviceSubscription$: Subscription;
  
  @ViewChild("search") searchTextBox : ElementRef;

  @HostListener('click', ['$event'])
  clickInside($event) {
    $event.stopPropagation();
  }

  @HostListener('document:click', ['$event'])
  clickOutside() {
    this.collapseAll();
    this.collapseSearchBar = true;
    this.focus.emit(false);
  }

  constructor(
    private authenticationService: AuthenticationService,
    private categoryService: CategoryService,
    public cartService: CartService,
    private router: Router,
    public deviceService: DeviceService
  ) {
    router.events.forEach((event) => {
      if(event instanceof NavigationStart) {
        this.collapseAll();
      }
    });
  }

  ngOnInit(): void {
    this.user = this.authenticationService.userValue;
    this.collapseAll();
    this.loadCategoryGroup();
  }


  ngAfterViewInit(): void {
    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        switch (this.deviceService.getDeviceType()) {
          case 'mobile': {
            this.collapseSearchBar = true;
            this.updateSetting(
              '',
              `margin-top: ${this.navElement.nativeElement.offsetHeight}px`,
              'mobile'
            );
            break;
          }
          case 'tablet': {
            this.collapseSearchBar = true;
            this.updateSetting(
              '',
              `margin-top: ${this.navElement.nativeElement.offsetHeight}px`,
              'tablet'
            );
            break;
          }
          default: {
            this.updateSetting('height: 60px', 'margin-top: 60px', 'desktop');
            break;
          }
        }
        this.collapseAll();
        this.focus.emit(false);
      }
    );
  }

  ngOnDestroy() {
    this.deviceSubscription$.unsubscribe();
    this.collapseAll();
    this.focus.emit(false);
  }

  private updateSetting(
    navHeight: string,
    navMargin: string,
    deviceType: string
  ) {
    this.settings.next(new NavSettings(navHeight, navMargin, deviceType));
  }

  updateAction() {
    if (this.searchQuery != "") this.action = 'clear';
    else this.action = 'search';
  }

  clearSearchQuery() {
    this.searchQuery = "";
  }

  showSearchResult() {
    this.router.navigate(['/search'], {
      queryParams: { q: this.searchQuery },
    });
  }

  searchWindowToggle() {
    if(this.settings.getValue().deviceType == 'desktop')
    {
      this.collapseSearchBar = !this.collapseSearchBar;
      if (!this.collapseSearchBar)
      {
        setTimeout(() => {
          this.searchTextBox.nativeElement.focus();
        }, 200);
      }
      this.collapseAll();
      return;
    }
    let state = this.collapseSearchWindow;
    this.collapseAll();
    this.collapseSearchWindow = !state;
    this.focus.emit(state);
  }

  categoryWindowToggle() {
    let state = this.collapseCategory;
    this.collapseAll();
    this.collapseCategory = !state;
    this.focus.emit(state);
  }

  cartWindowToggle() {
    let state = this.collapseCartWindow;
    this.collapseAll();
    this.collapseCartWindow = !state;
    this.focus.emit(state);

  }

  checkOutWindowToggle() {
    let state = this.collapseCheckoutWindow;
    this.collapseAll();
    this.collapseCheckoutWindow = !state;
    this.focus.emit(state);
  }

  cartCheckoutToggle() {
    this.collapseCheckoutWindow = !this.collapseCheckoutWindow;
    this.collapseCartWindow = !this.collapseCartWindow;
  }

  hideCategoryGroupDetailToggle() {
    this.hideCategoryGroupDetail = !this.hideCategoryGroupDetail;
  }

  collapseAll() {
    this.collapseCategory = true;
    this.collapseSearchWindow = true;
    this.collapseCartWindow = true;
    this.collapseCheckoutWindow = true;
    this.focus.emit(false);
  }

  logout() {
    this.authenticationService.logout();
      this.cartService.clearCart();
      this.cartService.clearLocalCart();
  }

  loadCategoryGroup() {
    this.categoryService.getCatalogue().subscribe((result) => {
      this.categoryGroups = result;
      this.selectCategoryGroup(0);
    });
  }

  setCollapseCategory(value: boolean) {
    if (
      this.settings.getValue().deviceType == 'desktop' &&
      !this.collapseCheckoutWindow
    )
      return;
    this.collapseAll();
    this.collapseCategory = value;
    this.focus.emit(!value);
  }

  selectCategoryGroup(index: number) {
    if (index < 0) {
      this.selectedCategoryGroup = {
        gender: -1,
        genderTitle: '',
        categories: [],
      };
      if (
        this.settings.getValue().deviceType == 'desktop' &&
        !this.collapseCheckoutWindow
      )
        return;
      this.focus.emit(false);
    } else this.selectedCategoryGroup = this.categoryGroups[index];
  }

  selectCategory(index: number) {
    this.selectedCategory = this.selectedCategoryGroup.categories[index];
  }

  viewCategory(categoryName: string, categorySlug: string, gender: number) {
    this.categoryService.setCurrentCategory(categoryName, gender);
    this.collapseAll();
    this.router.navigate(['/product'], {
      queryParams: { category: categorySlug, gender: gender },
    });
  }

  viewAccount(tab: string) {
    this.collapseAll();
    this.router.navigate(['account/'], { queryParams: { tab: tab } });
  }

  
}
