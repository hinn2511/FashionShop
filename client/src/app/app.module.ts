import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from './_modules/shared.module';
import { HomePageComponent } from './home-page/home-page.component';
import { ProductListComponent } from './customer/product-list/product-list.component';
import { ProductCardComponent } from './customer/product-card/product-card.component';
import { ProductDetailComponent } from './customer/product-detail/product-detail.component';
import { NavigationBarComponent } from './customer/navigation-bar/navigation-bar.component';
import { BreadcrumbComponent } from './customer/breadcrumb/breadcrumb.component';
import { ImageGalleryComponent } from './customer/image-gallery/image-gallery.component';
import { ImagePreviewClickDirective } from './_directives/image-preview-click.directive';
import { ButtonImageClickDirective } from './_directives/button-image-click.directive';
import { ProductRatingComponent } from './customer/product-rating/product-rating.component';
import { ProductCarouselComponent } from './customer/product-carousel/product-carousel.component';
import { ProductRecentViewComponent } from './customer/product-recent-view/product-recent-view.component';
import { HomeCarouselComponent } from './customer/home-carousel/home-carousel.component';
import { HomeCategoriesComponent } from './customer/home-categories/home-categories.component';
import { HomeBestSellerComponent } from './customer/home-best-seller/home-best-seller.component';
import { HomeColletionComponent } from './customer/home-colletion/home-colletion.component';
import { FooterComponent } from './customer/footer/footer.component';
import { HomeNewsletterComponent } from './customer/home-newsletter/home-newsletter.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DateInputComponent } from './_forms/date-input/date-input.component';
import { NumberInputComponent } from './_forms/number-input/number-input.component';
import { ReactiveFormsModule,FormsModule } from '@angular/forms'
import { CartComponent } from './customer/cart/cart.component';
import { ProductRelatedComponent } from './customer/product-related/product-related.component';
import { SearchBarComponent } from './customer/search-bar/search-bar.component';
import { ProductAboutComponent } from './customer/product-about/product-about.component';
import { LoginComponent } from './customer/login/login.component';
import { appInitializer } from './_helpers/app.initializer';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { AuthenticationService } from './_services/authentication.service';
import { HasRoleDirective } from './_directives/has-role.directive';
import { AdminSidebarComponent } from './administrator/admin-sidebar/admin-sidebar.component';
import { AdminLoginComponent } from './administrator/admin-login/admin-login.component';
import { AdminProductComponent } from './administrator/admin-product/admin-product.component';
import { AdminProductAddComponent } from './administrator/admin-product-add/admin-product-add.component';
import { SafeurlPipe } from './_pipes/safeurl.pipe';
import { AdminProductDetailComponent } from './administrator/admin-product-detail/admin-product-detail.component';
import { AdminProductPhotoComponent } from './administrator/admin-product-photo/admin-product-photo.component';
import { TextAreaInputComponent } from './_forms/text-area-input/text-area-input.component';
import { ProductImageModalComponent } from './_modals/product-image-modal/product-image-modal.component';
import { CarouselComponent } from './_common/carousel/carousel.component';
import { ImageViewerModalComponent } from './_modals/image-viewer-modal/image-viewer-modal.component';
import { ProductImagesComponent } from './customer/product-images/product-images.component';
import { AdminCarouselComponent } from './administrator/admin-carousel/admin-carousel.component';
import { CarouselPreviewComponent } from './_common/carousel-preview/carousel-preview.component';
import { AdminCarouselAddComponent } from './administrator/admin-carousel-add/admin-carousel-add.component';


@NgModule({
  declarations: [
    AppComponent,
    HomePageComponent,
    ProductListComponent,
    ProductCardComponent,
    ProductDetailComponent,
    NavigationBarComponent,
    BreadcrumbComponent,
    ImageGalleryComponent,
    ImagePreviewClickDirective,
    ButtonImageClickDirective,
    ProductRatingComponent,
    ProductCarouselComponent,
    ProductRecentViewComponent,
    HomeCarouselComponent,
    HomeCategoriesComponent,
    HomeBestSellerComponent,
    HomeColletionComponent,
    FooterComponent,
    HomeNewsletterComponent,
    TextInputComponent,
    DateInputComponent,
    NumberInputComponent,
    CartComponent,
    ProductRelatedComponent,
    SearchBarComponent,
    ProductAboutComponent,
    LoginComponent,
    HasRoleDirective,
    AdminSidebarComponent,
    AdminLoginComponent,
    AdminProductComponent,
    AdminProductAddComponent,
    SafeurlPipe,
    AdminProductDetailComponent,
    AdminProductPhotoComponent,
    TextAreaInputComponent,
    ProductImageModalComponent,
    CarouselComponent,
    ImageViewerModalComponent,
    ProductImagesComponent,
    CarouselPreviewComponent,
    AdminCarouselAddComponent,
    AdminCarouselComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
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
