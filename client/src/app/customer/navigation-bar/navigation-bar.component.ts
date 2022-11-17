import { UpdateCartItem, CartItemList } from './../../_models/cart';
import { CartService } from 'src/app/_services/cart.service';
import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  HostListener,
  ElementRef,
  OnDestroy,
} from '@angular/core';
import { Router } from '@angular/router';
import { CartItem } from 'src/app/_models/cart';
import {
  Catalogue,
  CategoryCatalogue,
  SubCategoryCatalogue,
} from 'src/app/_models/category';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { CategoryService } from 'src/app/_services/category.service';
import { map, take } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css'],
})
export class NavigationBarComponent implements OnInit, OnDestroy {
  @Output() focus = new EventEmitter<boolean>();
  collapseNavbar: boolean = true;
  collapseSearchbar: boolean = true;
  collapseCategory: boolean = true;
  collapseCartWindow: boolean = true;
  categoryGroups: Catalogue[] = [];
  selectedCategoryGroup: Catalogue;
  categories: CategoryCatalogue[] = [];
  selectedCategory: CategoryCatalogue;
  selectedSubCategory: SubCategoryCatalogue;
  user: User;

  constructor(
    private authenticationService: AuthenticationService,
    private categoryService: CategoryService,
    public cartService: CartService,
    private router: Router,
    private eRef: ElementRef
  ) {
    this.authenticationService.currentUser$
      .pipe(take(1))
      .subscribe((result) => (this.user = result));
  }

  @HostListener('click', ['$event'])
  clickInside($event) {
    $event.stopPropagation();
  }

  @HostListener('document:click', ['$event'])
  clickOutside() {
    this.collapseAll();
    this.focus.emit(false);
  }

  ngOnInit(): void {
    this.collapseAll();
    this.loadCategoryGroup();
  }

  ngOnDestroy(): void {
    this.collapseAll();
    this.focus.emit(false);
  }

  navigationBarToggle() {
    let state = this.collapseNavbar;
    this.collapseAll();
    this.collapseNavbar = !state;
    this.focus.emit(state);
  }
  searchBarToggle() {
    let state = this.collapseSearchbar;
    this.collapseAll();
    this.collapseSearchbar = !state;
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

  collapseAll() {
    this.collapseCategory = true;
    this.collapseNavbar = true;
    this.collapseSearchbar = true;
    this.collapseCartWindow = true;
    this.focus.emit(false);
  }

  logout() {
    this.authenticationService.logout();
    this.cartService.clearCart();
    this.cartService.clearLocalCart();
  }

  isUserExist(): boolean {
    return (
      this.authenticationService.userValue !== null &&
      this.authenticationService.userValue !== undefined
    );
  }

  loadCategoryGroup() {
    this.categoryService.getCatalogue().subscribe((result) => {
      this.categoryGroups = result;
    });
  }

  setCollapseCategory(value: boolean) {
    this.collapseAll();
    this.collapseCategory = value;
    this.focus.emit(!value);
  }

  selectCategoryGroup(index: number) {
    this.selectedCategoryGroup = this.categoryGroups[index];
  }

  isCategoryGroupSelected(gender: number) {
    if (
      this.selectedCategoryGroup == undefined ||
      this.selectedCategoryGroup == null
    )
      return false;
    return this.selectedCategoryGroup.gender == gender;
  }

  selectCategory(index: number) {
    this.selectedCategory = this.selectedCategoryGroup.categories[index];
  }

  viewCategory(categoryName: string, categorySlug: string, gender: number) {
    this.categoryService.setCurrentCategory(categoryName, gender);
    this.router.navigate(['/products'], {
      queryParams: { category: categorySlug, gender: gender },
    });
  }

  increaseQuantity(cartItem: CartItem) {    
    if (cartItem.quantity < 99) {
      if (this.user == null || this.user == undefined) {
        ++cartItem.quantity;
        this.cartService.updateLocalCart(cartItem);
        this.collapseCartWindow = false;
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
    }
     else this.deleteCartItem(cartItem);
  }

  deleteCartItem(cartItem: CartItem) {
    if (this.user == null || this.user == undefined) 
      this.cartService.deleteLocalCartItem(cartItem.optionId);
    else
      this.cartService.deleteCart(cartItem.id).subscribe((_) => {});

  }

  viewCart() {
    this.collapseAll();
    this.router.navigateByUrl('/my-cart');
  }


}
