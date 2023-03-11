import { CategorySummaryComponent } from './customer/category-summary/category-summary.component';
import { ProductSaleComponent } from './customer/product-sale/product-sale.component';
import { AdminDashboardComponent } from './administrator/dashboard/admin-dashboard/admin-dashboard.component';
import { OrderReviewComponent } from './customer/order-review/order-review.component';
import { RegisterComponent } from './customer/register/register.component';
import { SearchResultComponent } from './customer/search-result/search-result.component';
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
import { AdminCategoryComponent } from './administrator/admin-category/admin-category.component';
import { AdminCategoryAddComponent } from './administrator/admin-category-add/admin-category-add.component';
import { AdminCategoryEditComponent } from './administrator/admin-category-edit/admin-category-edit.component';
import { AdminProductOptionEditComponent } from './administrator/admin-product-option-edit/admin-product-option-edit.component';
import { AdminCategoryDetailComponent } from './administrator/admin-category-detail/admin-category-detail.component';
import { AdminRoleComponent } from './administrator/admin-role/admin-role.component';
import { AdminRoleAddComponent } from './administrator/admin-role-add/admin-role-add.component';
import { AdminRoleDetailComponent } from './administrator/admin-role-detail/admin-role-detail.component';
import { AdminUserComponent } from './administrator/admin-user/admin-user.component';
import { AdminUserDetailComponent } from './administrator/admin-user-detail/admin-user-detail.component';
import { AdminSettingComponent } from './administrator/admin-setting/admin-setting.component';

export const routes: Routes = [
  {path: '', component: HomePageComponent, pathMatch: 'full', data: { state: 'home' }},

  {path: 'register', component: RegisterComponent, data: { state: 'register' } },
  
  {path: 'login', component: LoginComponent, data: { state: 'login' } },
  {path: 'administrator/login', component: AdminLoginComponent},

  {
    path: 'product',
    children: [
      {
        path: '',
        component: ProductListComponent,
        data: { state: 'product' }
      },
      {
        path: ':slug',
        component: ProductDetailComponent,
        data: { state: 'product-detail' }
      }     
    ],
  },

  {
    path: 'categories',
    children: [
      {
        path: '',
        component: CategorySummaryComponent,
        data: { state: 'categories' }
      }
    ],
  },

  { path: 'search', component: SearchResultComponent, data: { state: 'search' } },

  { path: 'sale-off', component: ProductSaleComponent, data: { state: 'product-sale' }},

  {
    path: 'news',
    children: [
      {
        path: '',
        component: NewsComponent,
        data: { state: 'news' }
      },
      {
        path: ':slug',
        component: NewsDetailComponent,
        data: { state: 'new-detail' }
      },
    ],
  },

  {
    path: 'order',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: OrderDetailComponent,
        canActivate: [AuthGuard],
        data: { state: 'order' }
      },
      {
        path: 'review',
        component: OrderReviewComponent,
        canActivate: [AuthGuard],
        data: { state: 'order-preview' }
      }
    ],
  },

  { path: 'account', component: AccountComponent, canActivate: [AuthGuard], data: { state: 'account' }},
  
  {
    path: 'administrator/dashboard',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminDashboardComponent,
        canActivate: [AuthGuard],
      }
    ],
  },

  
  {
    path: 'administrator/setting-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminSettingComponent,
        canActivate: [AuthGuard],
      },
    ],
  },

  {
    path: 'administrator/user-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminUserComponent,
        canActivate: [AuthGuard],
      },
      {
        path: ':id',
        component: AdminUserComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'detail/:id',
        component: AdminUserDetailComponent,
        canActivate: [AuthGuard],
      },
    ],
  },

  {
    path: 'administrator/role-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminRoleComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'detail/:id',
        component: AdminRoleDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'add',
        component: AdminRoleAddComponent,
        canActivate: [AuthGuard],
      }
    ],
  },

  {
    path: 'administrator/product-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminProductComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'add',
        component: AdminProductAddComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'edit/:id',
        component: AdminProductEditComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'detail/:id',
        component: AdminProductDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'photos/:id',
        component: AdminProductPhotoComponent,
        canActivate: [AuthGuard],
      },
    ],
  },

  {
    path: 'administrator/category-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminCategoryComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'detail/:id',
        component: AdminCategoryDetailComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'add',
        component: AdminCategoryAddComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'edit/:id',
        component: AdminCategoryEditComponent,
        canActivate: [AuthGuard],
      },
    ],
  },

  {
    path: 'administrator/carousel-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminCarouselComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'add',
        component: AdminCarouselAddComponent,
        canActivate: [AuthGuard],
      },
    ],
  },

  {
    path: 'administrator/option-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminProductOptionComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'add',
        component: AdminProductOptionAddComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'edit/:id',
        component: AdminProductOptionEditComponent,
        canActivate: [AuthGuard],
      },
    ],
  },

  {
    path: 'administrator/order-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminOrderComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'detail/:id',
        component: AdminOrderDetailComponent,
        canActivate: [AuthGuard],
      },
    ],
  },

  {
    path: 'administrator/article-manager',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: AdminArticleComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'add',
        component: AdminArticleAddComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'detail/:id',
        component: AdminArticleDetailComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
