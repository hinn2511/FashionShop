import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from './customer/home-page/home-page.component';
import { ProductDetailComponent } from './customer/product-detail/product-detail.component';
import { ProductListComponent } from './customer/product-list/product-list.component';

const routes: Routes = [
  {path: '', component: HomePageComponent},
  {path: 'products', component: ProductListComponent},
  {path: 'product/:slug', component: ProductDetailComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
