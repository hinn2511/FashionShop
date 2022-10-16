import { Component, OnInit } from '@angular/core';
import { ManagerProduct, Product } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-admin-product-detail',
  templateUrl: './admin-product-detail.component.html',
  styleUrls: ['./admin-product-detail.component.css']
})
export class AdminProductDetailComponent implements OnInit {
  product: ManagerProduct;
  descriptionReview: boolean;

  constructor(private productService: ProductService) { }

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

}
