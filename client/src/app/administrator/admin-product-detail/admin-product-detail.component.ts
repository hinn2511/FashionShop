import { Component, OnInit } from '@angular/core';
import { Product } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-admin-product-detail',
  templateUrl: './admin-product-detail.component.html',
  styleUrls: ['./admin-product-detail.component.css']
})
export class AdminProductDetailComponent implements OnInit {
  product: Product;

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProductDetail(this.productService.getSelectedProductId());
  }

  loadProductDetail(productId: number) {
    this.productService.getProduct(productId).subscribe(result => {
      this.product = result;
    })
  }

}
