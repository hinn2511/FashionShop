import { AuthenticationService } from 'src/app/_services/authentication.service';
import { FileService } from 'src/app/_services/file.service';
import { IdArray } from 'src/app/_models/adminRequest';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Pagination } from 'src/app/_models/pagination';
import { ManagerProduct } from 'src/app/_models/product';
import { animate, state, style, transition, trigger } from '@angular/animations';
import {
  ManagerProductParams,
} from 'src/app/_models/productParams';
import { ProductService } from 'src/app/_services/product.service';
import { environment } from 'src/environments/environment';
import { ToastrService } from 'ngx-toastr';
import { fnGetObjectStateString, fnGetObjectStateStyle } from 'src/app/_common/function/global';

@Component({
  selector: 'app-admin-product',
  templateUrl: './admin-product.component.html',
  styleUrls: ['./admin-product.component.css'],
  animations: [
    trigger('rotatedState', [
        state('default', style({ transform: 'rotate(0)' })),
        state('rotated', style({ transform: 'rotate(-180deg)' })),
        transition('rotated => default', animate('500ms ease-out')),
        transition('default => rotated', animate('500ms ease-in'))
      ])
  ]
})
export class AdminProductComponent implements OnInit {
  products: ManagerProduct[];
  pagination: Pagination;
  productParams: ManagerProductParams;
  selectAllProduct: boolean;
  state: string = 'default';

  showStatusFilter: boolean;

  selectedIds: number[] = [];
  query: string;
  baseUrl = environment.apiUrl;

  constructor(
    private productService: ProductService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private fileService: FileService,
    private toastr: ToastrService
  ) {
    this.productParams = this.productService.getManagerProductParams();
  }

  ngOnInit(): void {
    this.productParams.field = 'Id';
    this.productParams.orderBy = 0;
    this.productParams.productStatus = [0, 1];
    this.selectAllProduct = false;
    this.showStatusFilter = false;
    this.loadProducts();
  }

  rotate() {
    this.state = (this.state === 'default' ? 'rotated' : 'default');
  }

  loadProducts() {
    this.productService.setManagerProductParams(this.productParams);

    this.productService
      .getManagerProducts(this.productParams)
      .subscribe((response) => {
        this.products = response.result;
        this.pagination = response.pagination;
      });
  }

  resetSelectedIds() {
    this.selectedIds = [];
  }

  viewDetail(productId: number) {
    this.productService.setSelectedProductId(productId);
    this.router.navigateByUrl(
      '/administrator/product-manager/detail/' + productId
    );
  }

  editProduct() {
    if (!this.isSingleSelected()) return;
    this.productService.setSelectedProductId(this.selectedIds[0]);
    this.router.navigateByUrl(
      `/administrator/product-manager/edit/${this.selectedIds[0]}`
    );
  }

  hideProducts() {
    if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.productService.hideProducts(ids).subscribe((result) => {
      this.loadProducts();
      this.resetSelectedIds();
      this.toastr.success('Products have been hidden or unhidden', 'Success');
    }, 
    error => 
    {
      this.toastr.error("Something wrong happen!", 'Error');
    });
  }

  deleteProducts() {
    if (!this.isMultipleSelected()) return;
    this.productService.deleteProduct(this.selectedIds).subscribe((result) => {
      this.loadProducts();
      this.resetSelectedIds();
      this.toastr.success('Products have been deleted', 'Success');
    }, 
    error => 
    {
      this.toastr.error("Something wrong happen!", 'Error');
    });
  }

  pageChanged(event: any) {
    if (this.productParams.pageNumber !== event.page) {
      this.productParams.pageNumber = event.page;
      this.productService.setManagerProductParams(this.productParams);
      this.loadProducts();
    }
  }

  sort(type: number) {
    this.productParams.orderBy = type;
    this.loadProducts();
  }

  filter(params: ManagerProductParams) {
    this.productParams = params;
    this.loadProducts();
  }

  resetFilter() {
    this.productParams = this.productService.resetManagerProductParams();
    this.loadProducts();
  }

  orderBy(field: string) {
    switch (field) {
      case 'id':
        this.productParams.field = 'Id';
        break;
      case 'name':
        this.productParams.field = 'Name';
        break;
      case 'sold':
        this.productParams.field = 'Sold';
        break;
      case 'price':
        this.productParams.field = 'Price';
        break;
      case 'status':
        this.productParams.field = 'Status';
        break;
      default:
        this.productParams.field = 'Date';
        break;
    }
    if (this.productParams.orderBy == 0) this.productParams.orderBy = 1;
    else this.productParams.orderBy = 0;
    this.rotate();
    this.loadProducts();
  }

  selectAllProducts() {
    if (this.selectAllProduct) {
      this.selectedIds = [];
    } else {
      this.selectedIds = this.products.map(({ id }) => id);
    }
    this.selectAllProduct = !this.selectAllProduct;
  }

  selectProduct(id: number) {
    if (this.selectedIds.includes(id)) {
      this.selectedIds.splice(this.selectedIds.indexOf(id), 1);
    } else {
      this.selectedIds.push(id);
    }
  }

  isProductSelected(id: number) {
    if (this.selectedIds.indexOf(id) >= 0) return true;
    return false;
  }

  getProductState(product: ManagerProduct) {
    return fnGetObjectStateString(product.status);
  }

  getStateStyle(product: ManagerProduct) {
    return fnGetObjectStateStyle(product.status);
  }

  isStatusIncluded(status: number) {
    return this.productParams.productStatus.indexOf(status) > -1;
  }

  selectStatus(status: number) {
    if (this.isStatusIncluded(status))
      this.productParams.productStatus =
        this.productParams.productStatus.filter((x) => x !== status);
    else this.productParams.productStatus.push(status);
    this.productParams.productStatus = [...this.productParams.productStatus].sort((a, b) => a - b);
    this.loadProducts();
  }

  selectAllProductStatus() {
    if(this.productParams.productStatus.length == 3)
      this.productParams.productStatus = [];
    else
      this.productParams.productStatus = [0, 1, 2];
    this.loadProducts();
  }

  statusFilterToggle() {
    this.showStatusFilter = !this.showStatusFilter;
  }

  isSingleSelected() {
    return this.selectedIds.length == 1;
  }

  isMultipleSelected() {
    return this.selectedIds.length >= 1;
  }

  onFileSelected(event: any) {
    if (event.target.files && event.target.files[0]) {
      let file = event.target.files[0];
      this.fileService.uploadFile(file, this.baseUrl + 'product/import', this.authenticationService.userValue.jwtToken).subscribe(result => {
        this.loadProducts();
        this.toastr.success('Products have been imported', 'Success');
      }, 
      error => 
      {
        this.toastr.error("Something wrong happen!", 'Error');
      });
    }
  }
}
