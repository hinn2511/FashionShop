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
  ViewChild,
  AfterViewInit,
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
import { User } from 'src/app/_models/user';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css'],
})
export class NavigationBarComponent implements OnInit, OnDestroy, AfterViewInit {
  @Output() focus = new EventEmitter<boolean>();
  @ViewChild('nav') navElement: ElementRef;
  collapseNavbar: boolean = true;
  collapseSearchbar: boolean = true;
  collapseCategory: boolean = true;
  collapseCartWindow: boolean = true;
  hideCategoryGroupDetail: boolean = true;
  categoryGroups: Catalogue[] = [];
  selectedCategoryGroup: Catalogue;
  categories: CategoryCatalogue[] = [];
  selectedCategory: CategoryCatalogue;
  selectedSubCategory: SubCategoryCatalogue;
  user: User;
  navHeight: string;
  navMargin: string;
  cartMargin: string;
  isMobile: boolean;


  constructor(
    private authenticationService: AuthenticationService,
    private categoryService: CategoryService,
    public cartService: CartService,
    private router: Router,
    private toastr: ToastrService
  ) {
    
  }

  ngAfterViewInit(): void {
    if(window.innerWidth < 768)
    {
      
      this.navHeight = '';
      this.navMargin = 'margin-top: ' + this.navElement.nativeElement.offsetHeight + 'px';
      this.cartMargin = this.navMargin;
      this.isMobile = true;
      
    }
    if(window.innerWidth >= 768 && window.innerWidth <= 1280)
    {
      this.navHeight = 'height: 7vh'
      this.navMargin = 'margin-top: 7vh';
      this.cartMargin = 'margin-top: 8vh; margin-right: 1vh; border-radius: 3px;'
      this.isMobile = false;
    }
    if(window.innerWidth > 1280)
    {
      this.navHeight = 'height: 8vh'
      this.navMargin = 'margin-top: 8vh';
      this.cartMargin = 'margin-top: 9vh; margin-right: 1vh; border-radius: 3px;'
      this.isMobile = false;
    }
  }
  
  ngOnDestroy() {
    this.collapseAll();
    this.focus.emit(false);
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
  
    this.user = this.authenticationService.userValue;
    this.collapseAll();
    this.loadCategoryGroup();
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

  hideCategoryGroupDetailToggle() {
    this.hideCategoryGroupDetail = !this.hideCategoryGroupDetail;
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
      this.selectCategoryGroup(0);
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
    this.collapseAll();
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
    let success = true;
    if (this.user == null || this.user == undefined) 
    {
      this.cartService.deleteLocalCartItem(cartItem.optionId);
      success = true;
    }
    else
    this.cartService.deleteCart(cartItem.id).subscribe((_) => {},
    error => {
      success = false;
    });
    if(success)
      this.toastr.success('This item has been removed from your cart!', 'Success');
    else
      this.toastr.error('Something wrong happen!', 'Error');
  }

  viewCart() {
    this.collapseAll();
    this.router.navigateByUrl('/my-cart');
  }

  viewProduct(cartItem: CartItem) {
    this.collapseAll();
    this.router.navigate(
      ['product/' + cartItem.slug],
      { queryParams: { id: cartItem.productId } }
    );
  }

  viewAccount(tab: string) {
    this.collapseAll();
    this.router.navigate(
      ['account/'],
      { queryParams: { tab: tab } }
    );
  }

}
