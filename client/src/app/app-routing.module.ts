import { OrderDetailComponent } from './customer/order-detail/order-detail.component';
import { AdminProductOptionAddComponent } from './administrator/admin-product-option-add/admin-product-option-add.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from './home-page/home-page.component';
import { LoginComponent } from './customer/login/login.component';
import { ProductDetailComponent } from './customer/product-detail/product-detail.component';
import { ProductListComponent } from './customer/product-list/product-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { AdminLoginComponent } from './administrator/admin-login/admin-login.component';
import { AdminProductComponent } from './administrator/admin-product/admin-product.component';
import { AdminProductAddComponent } from './administrator/admin-product-add/admin-product-add.component';
import { AdminProductDetailComponent } from './administrator/admin-product-detail/admin-product-detail.component';
import { AdminProductPhotoComponent } from './administrator/admin-product-photo/admin-product-photo.component';
import { AdminCarouselAddComponent } from './administrator/admin-carousel-add/admin-carousel-add.component';
import { AdminCarouselComponent } from './administrator/admin-carousel/admin-carousel.component';
import { AdminProductOptionComponent } from './administrator/admin-product-option/admin-product-option.component';
import { AccountComponent } from './customer/account/account.component';
import { AdminOrderComponent } from './administrator/admin-order/admin-order.component';
import { AdminOrderDetailComponent } from './administrator/admin-order-detail/admin-order-detail.component';
import { AdminProductEditComponent } from './administrator/admin-product-edit/admin-product-edit.component';
import { AdminArticleComponent } from './administrator/admin-article/admin-article.component';
import { AdminArticleAddComponent } from './administrator/admin-article-add/admin-article-add.component';
import { AdminArticleDetailComponent } from './administrator/admin-article-detail/admin-article-detail.component';
import { NewsComponent } from './customer/news/news.component';
import { NewsDetailComponent } from './customer/news-detail/news-detail.component';

const routes: Routes = [
  {path: '', component: HomePageComponent},
  
  {path: 'login', component: LoginComponent },
  {path: 'administrator/login', component: AdminLoginComponent},

  {path: 'products', component: ProductListComponent},
  {path: 'product/:slug', component: ProductDetailComponent},

  {path: 'news', component: NewsComponent},
  {path: 'news/:slug', component: NewsDetailComponent},

  {path: 'order', component: OrderDetailComponent, canActivate: [AuthGuard]},

  {path: 'account', component: AccountComponent, canActivate: [AuthGuard] },

  {path: 'administrator/product-manager', component: AdminProductComponent, canActivate: [AuthGuard] },
  {path: 'administrator/product-manager/add', component: AdminProductAddComponent, canActivate: [AuthGuard] },
  {path: 'administrator/product-manager/edit/:id', component: AdminProductEditComponent, canActivate: [AuthGuard] },
  {path: 'administrator/product-manager/detail/:id', component: AdminProductDetailComponent, canActivate: [AuthGuard] },
  {path: 'administrator/product-manager/photos/:id', component: AdminProductPhotoComponent, canActivate: [AuthGuard] },

  {path: 'administrator/carousel-manager', component: AdminCarouselComponent, canActivate: [AuthGuard] },
  {path: 'administrator/carousel-manager/add', component: AdminCarouselAddComponent, canActivate: [AuthGuard] },

  {path: 'administrator/option-manager', component: AdminProductOptionComponent, canActivate: [AuthGuard] },
  {path: 'administrator/option-manager/add', component: AdminProductOptionAddComponent, canActivate: [AuthGuard] },

  {path: 'administrator/order-manager', component: AdminOrderComponent, canActivate: [AuthGuard] },
  {path: 'administrator/order-manager/detail/:id', component: AdminOrderDetailComponent, canActivate: [AuthGuard] },

  {path: 'administrator/article-manager', component: AdminArticleComponent, canActivate: [AuthGuard] },
  {path: 'administrator/article-manager/add', component: AdminArticleAddComponent, canActivate: [AuthGuard] },
  {path: 'administrator/article-manager/detail/:id', component: AdminArticleDetailComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
