import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddProductComponent } from './administrator/product/add-product/add-product.component';
import { EditProductComponent } from './administrator/product/edit-product/edit-product.component';
import { ProductManagementComponent } from './administrator/product/product-management/product-management.component';
import { ProductPhotoManagementComponent } from './administrator/product/product-photo-management/product-photo-management.component';
import { CartComponent } from './customer/cart/cart.component';
import { HomePageComponent } from './customer/home-page/home-page.component';
import { ProductDetailComponent } from './customer/product-detail/product-detail.component';
import { ProductListComponent } from './customer/product-list/product-list.component';

const routes: Routes = [
  {path: '', component: HomePageComponent},
  {path: 'products', component: ProductListComponent},
  {path: 'product/:slug', component: ProductDetailComponent},
  {path: 'my-cart', component: CartComponent},
  {path: 'administrator/product', component: ProductManagementComponent},
  {path: 'administrator/product/add', component: AddProductComponent},
  {path: 'administrator/product/edit/:id', component: EditProductComponent},
  {path: 'administrator/product-photo/:id', component: ProductPhotoManagementComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
