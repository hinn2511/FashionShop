import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { SharedModule } from './_modules/shared.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { HasRoleDirective } from './_directives/has-role.directive';
import { DebounceClickDirective } from './_directives/debounce-click.directive';
import { LazyLoadingImageDirective } from './_directives/lazy-loading-image.directive';

import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DateInputComponent } from './_forms/date-input/date-input.component';
import { NumberInputComponent } from './_forms/number-input/number-input.component';
import { IconTextInputComponent } from './_forms/icon-text-input/icon-text-input.component';
import { TextAreaInputComponent } from './_forms/text-area-input/text-area-input.component';

import { appInitializer } from './_helpers/app.initializer';

import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { ErrorInterceptor } from './_interceptors/error.interceptor';

import { SafeUrlPipe } from './_pipes/safeUrl.pipe';

import { HomePageComponent } from './home-page/home-page.component';
import { AccountComponent } from './customer/account/account.component';
import { FavoritesComponent } from './customer/favorites/favorites.component';
import { AccountInformationComponent } from './customer/account-information/account-information.component';
import { ProductListComponent } from './customer/product-list/product-list.component';
import { ProductCardComponent } from './customer/product-card/product-card.component';
import { ProductDetailComponent } from './customer/product-detail/product-detail.component';
import { NavigationBarComponent } from './customer/navigation-bar/navigation-bar.component';
import { BreadcrumbComponent } from './customer/breadcrumb/breadcrumb.component';
import { ProductRatingComponent } from './customer/product-rating/product-rating.component';
import { ProductRecentViewComponent } from './customer/product-recent-view/product-recent-view.component';
import { HomeCarouselComponent } from './customer/home-carousel/home-carousel.component';
import { HomeCategoriesComponent } from './customer/home-categories/home-categories.component';
import { FooterComponent } from './customer/footer/footer.component';
import { ReactiveFormsModule,FormsModule } from '@angular/forms'
import { CartComponent } from './customer/cart/cart.component';
import { SearchBarComponent } from './customer/search-bar/search-bar.component';
import { ProductAboutComponent } from './customer/product-about/product-about.component';
import { LoginComponent } from './customer/login/login.component';
import { AuthenticationService } from './_services/authentication.service';
import { OrderHistoryComponent } from './customer/order-history/order-history.component';
import { AdminSidebarComponent } from './administrator/admin-sidebar/admin-sidebar.component';
import { AdminLoginComponent } from './administrator/admin-login/admin-login.component';
import { AdminProductComponent } from './administrator/admin-product/admin-product.component';
import { AdminProductAddComponent } from './administrator/admin-product-add/admin-product-add.component';
import { AdminProductDetailComponent } from './administrator/admin-product-detail/admin-product-detail.component';
import { AdminProductPhotoComponent } from './administrator/admin-product-photo/admin-product-photo.component';
import { ProductImageModalComponent } from './_modals/product-image-modal/product-image-modal.component';
import { ImageViewerModalComponent } from './_modals/image-viewer-modal/image-viewer-modal.component';
import { ProductImagesComponent } from './customer/product-images/product-images.component';
import { AdminCarouselComponent } from './administrator/admin-carousel/admin-carousel.component';
import { AdminCarouselAddComponent } from './administrator/admin-carousel-add/admin-carousel-add.component';
import { AdminProductOptionAddComponent } from './administrator/admin-product-option-add/admin-product-option-add.component';
import { AdminProductOptionComponent } from './administrator/admin-product-option/admin-product-option.component';
import { CheckOutComponent } from './customer/check-out/check-out.component';
import { OrderCardComponent } from './customer/order-card/order-card.component';
import { OrderDetailComponent } from './customer/order-detail/order-detail.component';
import { AdminOrderComponent } from './administrator/admin-order/admin-order.component';
import { AdminOrderDetailComponent } from './administrator/admin-order-detail/admin-order-detail.component';
import { ConfirmDialogComponent } from './_dialog/confirm-dialog/confirm-dialog.component';

import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
import { EditorComponent } from './_forms/editor/editor.component';
import { AdminProductEditComponent } from './administrator/admin-product-edit/admin-product-edit.component';
import { AdminArticleComponent } from './administrator/admin-article/admin-article.component';
import { AdminArticleAddComponent } from './administrator/admin-article-add/admin-article-add.component';
import { AdminArticleDetailComponent } from './administrator/admin-article-detail/admin-article-detail.component';
import { NewsComponent } from './customer/news/news.component';
import { NewsCardComponent } from './customer/news-card/news-card.component';
import { NewsDetailComponent } from './customer/news-detail/news-detail.component';
import { NewsCarouselComponent } from './customer/news-carousel/news-carousel.component';
import { ColorOptionComponent } from './_common/color-option/color-option.component';
import { ImageCarouselComponent } from './_common/image-carousel/image-carousel.component';
import { ProductFilterComponent } from './customer/product-filter/product-filter.component';
import { AdminCategoryComponent } from './administrator/admin-category/admin-category.component';
import { AdminCategoryAddComponent } from './administrator/admin-category-add/admin-category-add.component';
import { CategoriesListDialogComponent } from './_dialog/categories-list-dialog/categories-list-dialog.component';
import { AdminCategoryEditComponent } from './administrator/admin-category-edit/admin-category-edit.component';
import { HomeInterestingComponent } from './customer/home-interesting/home-interesting.component';
import { HomeNewsComponent } from './customer/home-news/home-news.component';
import { SearchResultComponent } from './customer/search-result/search-result.component';
import { RegisterComponent } from './customer/register/register.component';

@NgModule({
  declarations: [
    AppComponent,
    HomePageComponent,
    ProductListComponent,
    ProductCardComponent,
    ProductDetailComponent,
    NavigationBarComponent,
    BreadcrumbComponent,
    ProductRatingComponent,
    ProductRecentViewComponent,
    HomeCarouselComponent,
    HomeCategoriesComponent,
    FooterComponent,
    TextInputComponent,
    DateInputComponent,
    NumberInputComponent,
    CartComponent,
    SearchBarComponent,
    ProductAboutComponent,
    LoginComponent,
    HasRoleDirective,
    AdminSidebarComponent,
    AdminLoginComponent,
    AdminProductComponent,
    AdminProductAddComponent,
    SafeUrlPipe,
    AdminProductDetailComponent,
    AdminProductPhotoComponent,
    TextAreaInputComponent,
    ProductImageModalComponent,
    ImageViewerModalComponent,
    ProductImagesComponent,
    AdminCarouselAddComponent,
    AdminCarouselComponent,
    AdminProductOptionComponent,
    AdminProductOptionAddComponent,
    AccountComponent,
    AccountInformationComponent, 
    FavoritesComponent, 
    DebounceClickDirective, 
    CheckOutComponent, 
    OrderHistoryComponent,
    OrderCardComponent,
    OrderDetailComponent,
    LazyLoadingImageDirective,
    AdminOrderComponent,
    IconTextInputComponent,
    AdminOrderDetailComponent,
    ConfirmDialogComponent,
    EditorComponent,
    AdminProductEditComponent,
    AdminArticleComponent,
    AdminArticleAddComponent,
    AdminArticleDetailComponent,
    NewsComponent,
    NewsCardComponent,
    NewsDetailComponent,
    NewsCarouselComponent,
    ColorOptionComponent,
    ImageCarouselComponent,
    ProductFilterComponent,
    AdminCategoryComponent,
    AdminCategoryAddComponent,
    CategoriesListDialogComponent,
    AdminCategoryEditComponent,
    HomeInterestingComponent,
    HomeNewsComponent,
    SearchResultComponent,
    RegisterComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,   
    CKEditorModule,
    SharedModule
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: appInitializer, multi: true, deps: [AuthenticationService] },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
],
  bootstrap: [AppComponent]
})
export class AppModule { }
