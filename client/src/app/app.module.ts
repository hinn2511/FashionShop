import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from './_modules/shared.module';
import { HomePageComponent } from './customer/home-page/home-page.component';
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
import { ProductManagementComponent } from './administrator/product/product-management/product-management.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DateInputComponent } from './_forms/date-input/date-input.component';
import { NumberInputComponent } from './_forms/number-input/number-input.component';
import { ReactiveFormsModule,FormsModule } from '@angular/forms';
import { ProductPhotoManagementComponent } from './administrator/product/product-photo-management/product-photo-management.component';
import { AddProductComponent } from './administrator/product/add-product/add-product.component';
import { EditProductComponent } from './administrator/product/edit-product/edit-product.component';
import { CartComponent } from './customer/cart/cart.component';
import { ProductRelatedComponent } from './customer/product-related/product-related.component';
import { SearchBarComponent } from './customer/search-bar/search-bar.component';
import { ProductAboutComponent } from './customer/product-about/product-about.component';
import { LoginComponent } from './customer/login/login.component';
import { appInitializer } from './_helpers/app.initializer';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { AuthenticationService } from './_services/authentication.service';


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
    ProductManagementComponent,
    TextInputComponent,
    DateInputComponent,
    NumberInputComponent,
    ProductPhotoManagementComponent,
    AddProductComponent,
    EditProductComponent,
    CartComponent,
    ProductRelatedComponent,
    SearchBarComponent,
    ProductAboutComponent,
    LoginComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
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
