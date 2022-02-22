import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

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
import { ProductRelatedComponent } from './customer/product-related/product-related.component';
import { ProductCarouselComponent } from './customer/product-carousel/product-carousel.component';
import { ProductRecentViewComponent } from './customer/product-recent-view/product-recent-view.component';

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
    ProductRelatedComponent,
    ProductCarouselComponent,
    ProductRecentViewComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    SharedModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
