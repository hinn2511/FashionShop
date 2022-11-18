import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { ManagerProduct, Product } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';
import { IdArray } from 'src/app/_models/adminRequest';
import { ToastrService } from 'ngx-toastr';

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

  getStateStyle() {
    switch (this.product.status) {
      case 0:
        return 'width: 100px ;background-color: rgb(51, 155, 51)';
      case 1:
        return 'width: 100px ;background-color: rgb(124, 124, 124)';
      default:
        return 'width: 100px ;background-color: rgb(155, 51, 51)';
    }
  }

  getProductState() {
    switch (this.product.status) {
      case 0:
        return 'Active';
      case 1:
        return 'Hidden';
      default:
        return 'Deleted';
    }
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
