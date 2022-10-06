import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddProductComponent } from './administrator/product/add-product/add-product.component';
import { EditProductComponent } from './administrator/product/edit-product/edit-product.component';
import { ProductManagementComponent } from './administrator/product/product-management/product-management.component';
import { ProductPhotoManagementComponent } from './administrator/product/product-photo-management/product-photo-management.component';
import { CartComponent } from './customer/cart/cart.component';
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

const routes: Routes = [
  {path: '', component: HomePageComponent},
  {path: 'login', component: LoginComponent },
  {path: 'products', component: ProductListComponent},
  {path: 'product/:slug', component: ProductDetailComponent},
  {path: 'my-cart', component: CartComponent},
  {path: 'administrator/login', component: AdminLoginComponent},
  {path: 'administrator/product-manager', component: AdminProductComponent, canActivate: [AuthGuard] },
  {path: 'administrator/product-manager/add', component: AdminProductAddComponent, canActivate: [AuthGuard] },
  {path: 'administrator/product-manager/detail/:id', component: AdminProductDetailComponent, canActivate: [AuthGuard] },
  {path: 'administrator/product-manager/photos/:id', component: AdminProductPhotoComponent, canActivate: [AuthGuard] },
  //{path: 'administrator/product-manager', component: AdminProductComponent},


  // {path: 'administrator/product', component: ProductManagementComponent, canActivate: [AuthGuard] },
  // {path: 'administrator/product/add', component: AddProductComponent},
  // {path: 'administrator/product/edit/:id', component: EditProductComponent},
  // {path: 'administrator/product-photo/:id', component: ProductPhotoManagementComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
