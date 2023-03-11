import { Subscription } from 'rxjs';
import { SlideRightToLeft } from './../../_common/animation/common.animation';
import { SlideLeftToRight } from 'src/app/_common/animation/common.animation';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import {
  CustomerCatalogue,
  CustomerCategoryCatalogue,
} from 'src/app/_models/category';
import { User } from 'src/app/_models/user';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { CartService } from 'src/app/_services/cart.service';
import { CategoryService } from 'src/app/_services/category.service';
import { fnIsNullOrEmpty, fnHasValue } from 'src/app/_common/function/function';

@Component({
  selector: 'app-navigation-bar-mobile',
  templateUrl: './navigation-bar-mobile.component.html',
  styleUrls: ['./navigation-bar-mobile.component.css'],
  animations: [SlideLeftToRight, SlideRightToLeft],
})
export class NavigationBarMobileComponent implements OnInit, OnDestroy {
  collapseCategory: boolean = true;

  isLoggedIn: boolean = false;
  hideCategoryGroupDetail: boolean = true;
  collapseCheckoutWindow: boolean = true;
  categoryGroups: CustomerCatalogue[] = [];
  selectedCategoryGroup: CustomerCatalogue;
  selectedCategory: CustomerCategoryCatalogue;
  selectedSubCategory: CustomerCategoryCatalogue;

  initial = true;
  userSubscription$: Subscription;

  constructor(
    private authenticationService: AuthenticationService,
    private categoryService: CategoryService,
    private cartService: CartService,
    private router: Router
  ) {}
  ngOnDestroy(): void {
    this.userSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.loadCategoryGroup();
    this.userSubscribe();
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
      this.initial = false;
    });
  }

  userSubscribe() {
    this.userSubscription$ = this.authenticationService.currentUser$.subscribe(
      (result) => {
        if (fnHasValue<User>(result)) this.isLoggedIn = true;
        else this.isLoggedIn = false;
      }
    );
  }

  setCollapseCategory(value: boolean) {
    this.collapseCategory = value;
  }

  selectCategoryGroup(index: number) {
    if (index < 0) {
      this.selectedCategoryGroup = {
        gender: -1,
        genderTitle: '',
        categories: [],
      };
    } else this.selectedCategoryGroup = this.categoryGroups[index];
    if (!this.initial) this.hideCategoryGroupDetailToggle();
  }

  selectCategory(index: number) {
    this.selectedCategory = this.selectedCategoryGroup.categories[index];
  }

  viewCategory(category: CustomerCategoryCatalogue, gender: number) {
    this.categoryService.setCurrentCategory(category.categoryName, gender);
    if (
      this.selectedCategory != undefined &&
      this.selectedCategory.categoryName == category.categoryName
    )
      this.selectedCategory = undefined;
    else this.selectedCategory = category;

    if (category.subCategories.length > 0) return;

    this.navigateToCategory(category, gender);
  }

  private navigateToCategory(
    category: CustomerCategoryCatalogue,
    gender: number
  ) {
    this.router.navigate(['/product'], {
      queryParams: { category: category.id, gender: gender },
    });
  }

  viewAccount(tab: string) {
    this.router.navigate(['account/'], { queryParams: { tab: tab } });
  }

  hideCategoryGroupDetailToggle() {
    this.hideCategoryGroupDetail = !this.hideCategoryGroupDetail;
  }
}
