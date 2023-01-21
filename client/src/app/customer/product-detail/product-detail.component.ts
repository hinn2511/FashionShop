import { fnGetGenderName, Category } from 'src/app/_models/category';
import { CategoryService } from 'src/app/_services/category.service';
import { Carousel } from 'src/app/_models/carousel';
import { AccountService } from 'src/app/_services/account.service';
import { AuthenticationService } from './../../_services/authentication.service';
import { CartService } from 'src/app/_services/cart.service';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BreadCrumb } from 'src/app/_models/breadcrumb';
import { Product } from 'src/app/_models/product';
import {
  CustomerOption,
  CustomerOptionColor,
  CustomerOptionSize,
} from 'src/app/_models/productOptions';
import { OptionService } from 'src/app/_services/option.service';
import { ProductService } from 'src/app/_services/product.service';
import { CartItem, NewCartItem } from 'src/app/_models/cart';
import { User } from 'src/app/_models/user';
import { ToastrService } from 'ngx-toastr';
import { GrowAnimation } from 'src/app/_common/animation/common.animation';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css'],
  animations: [GrowAnimation]
})
export class ProductDetailComponent implements OnInit {
  product: Product;
  breadCrumb: BreadCrumb[];
  quantity: number;
  carousels: Carousel[] = [];
  options: CustomerOption[] = [];
  sizes: CustomerOptionSize[] = [];
  selectedSize: CustomerOptionSize;
  selectedColor: CustomerOptionColor;
  user: User;
  expandDescription: string = 'in';

  @ViewChild('descriptionTitle') descriptionTitleRef: ElementRef;

  constructor(
    private productService: ProductService,
    private optionService: OptionService,
    private cartService: CartService,
    private authenticationService: AuthenticationService,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService,
  ) {}

  ngOnInit(): void {
    this.user = this.authenticationService.userValue;
    
    this.loadProduct(this.route.snapshot.queryParams['id']);
    this.loadOptions(this.route.snapshot.queryParams['id']);
    this.quantity = 1;
  }

  loadProduct(id: number) {
    this.productService.getProduct(id).subscribe((response) => {
      this.product = response;
      this.productService.addToRecent(this.product);
      this.loadProductImageCarousel();
      this.loadBreadCrumb();
    });
    this.loadOptions(id);
  }

  loadProductImageCarousel() {
    this.product.productPhotos.forEach((element) => {
      this.carousels.push(new Carousel('', '', '', element.url));
    });
  }

  private loadBreadCrumb() {
    this.breadCrumb = [];
    this.breadCrumb = [
      {
        name: 'Home',
        route: '/',
        active: true,
      },
    ];

    this.breadCrumb.push({
      name: `${fnGetGenderName(this.product.gender)}`,
      route: ``,
      active: false,
    });

    if (this.product.parentCategory != null) {
      this.breadCrumb.push({
        name: `${this.product.parentCategory}`,
        route: `/product?category=${this.product.parentCategorySlug}&gender=${this.product.gender}`,
        active: true,
      });
    } 
   
    this.breadCrumb.push({
      name: `${this.product.category}`,
      route: `/product?category=${this.product.categorySlug}&gender=${this.product.gender}`,
      active: true,
    });  

    
    this.breadCrumb.push({
      name: `${this.product.productName}`,
      route: ``,
      active: false,
    });  
    
  }

  chooseColor(color: CustomerOptionColor) {
    this.selectedColor = color;
    this.sizes = this.options.filter(
      (x) => x.color.colorCode == color.colorCode
    )[0].sizes;
    this.selectedSize = this.sizes[0];
  }

  onSizeChange(size: CustomerOptionSize) {
    this.selectedSize = size;
  }

  loadOptions(productId: number) {
    this.optionService
      .getCustomerProductOption(productId)
      .subscribe((result) => {
        this.options = result;
        this.chooseColor(this.options[0].color);
      });
  }

  increaseQuantity() {
    if (this.quantity < 99) this.quantity++;
  }

  decreaseQuantity() {
    if (this.quantity > 1) this.quantity--;
  }

  getColorStyle(color: CustomerOptionColor) {
    return 'background-color: ' + color.colorCode;
  }

  addToCart() {
    if (this.user == null || this.user == undefined) {
      let cartItem: CartItem = {
        sizeName: this.selectedSize.sizeName,
        colorCode: this.selectedColor.colorCode,
        colorName: this.selectedColor.colorName,
        optionId: this.selectedSize.optionId,
        price: this.product.price,
        additionalPrice: this.selectedSize.additionalPrice,
        productName: this.product.productName,
        slug: this.product.slug,
        id: 0,
        productId: this.product.id,
        quantity: this.quantity,
        totalItemPrice:
          (this.product.price + this.selectedSize.additionalPrice) *
          this.quantity,
        imageUrl: this.product.url,
      };
      this.cartService.updateLocalCart(cartItem);
      this.toastr.success('This item has been added to your cart!', 'Success');
      return;
    }

    let newCartItem: NewCartItem = {
      quantity: this.quantity,
      optionId: this.selectedSize.optionId,
    };
    this.cartService.addToCart(newCartItem).subscribe(
      (result) => {
        this.toastr.success(
          'This item has been added to your cart!',
          'Success'
        );
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  likeProduct() {
    if (this.user == null || this.user == undefined) {
      this.router.navigateByUrl('/login');
    }
    this.accountService.addToFavorite(this.product.id).subscribe(
      (result) => {
        this.accountService.clearFavoriteCache();
        this.product.likedByUser = true;
        this.productService.removeCache();
        this.toastr.success(
          'This product have been added to your favorites',
          'Success'
        );
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  unlikeProduct() {
    this.accountService.removeFromFavorite(this.product.id).subscribe(
      (result) => {
        this.accountService.clearFavoriteCache();
        this.product.likedByUser = false;
        this.productService.removeCache();
        this.toastr.success(
          'This product have been removed from your favorites',
          'Success'
        );
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  expandDescriptionToggle()
  {
    this.expandDescription = this.expandDescription === 'in' ? 'out' : 'in';
    if(this.expandDescription === 'in')
      this.descriptionTitleRef.nativeElement.scrollIntoView({ behavior: 'smooth'});
    
  }
}
