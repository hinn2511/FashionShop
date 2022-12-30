import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { ManagerProduct } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';
import { IdArray } from 'src/app/_models/adminRequest';
import { ToastrService } from 'ngx-toastr';
import { fnGetObjectStateString, fnGetObjectStateStyle } from 'src/app/_common/function/global';

@Component({
  selector: 'app-admin-product-detail',
  templateUrl: './admin-product-detail.component.html',
  styleUrls: ['./admin-product-detail.component.css']
})
export class AdminProductDetailComponent implements OnInit {
  product: ManagerProduct;
  descriptionReview: boolean;

  id: IdArray = {
    ids: [this.productService.getSelectedProductId()],
  };

  constructor(private productService: ProductService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.loadProductDetail(this.productService.getSelectedProductId());
    this.descriptionReview = false;
  
  }

  loadProductDetail(productId: number) {
    this.productService.getManagerProduct(productId).subscribe(result => {
      this.product = result;
    })
  }

  descriptionPreviewToggle()
  {
    this.descriptionReview = !this.descriptionReview;
  }

  getProductState() {
    return fnGetObjectStateString(this.product.status);
  }

  getProductStateStyle() {
    return fnGetObjectStateStyle(this.product.status);
  }

  editProduct() {
    this.router.navigateByUrl(
      '/administrator/product-manager/edit/' + this.productService.getSelectedProductId()
    );
  }

  hideProduct() {    
    this.productService.hideProducts(this.id).subscribe((result) => {
      this.loadProductDetail(this.productService.getSelectedProductId());
      this.toastr.success('Product have been hidden or unhidden', 'Success');
    }, 
    error => 
    {
      this.toastr.error("Something wrong happen!", 'Error');
    });
  }

  deleteProduct() {
    this.productService.deleteProduct(this.id.ids).subscribe((result) => {
      this.loadProductDetail(this.productService.getSelectedProductId());
      this.toastr.success('Product have been deleted', 'Success');
    }, 
    error => 
    {
      this.toastr.error("Something wrong happen!", 'Error');
    });
  }
}
