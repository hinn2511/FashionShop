import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Params } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { BreadCrumb } from 'src/app/_models/breadcrum';
import { Product } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  product: Product;
  breadCrumb: BreadCrumb[]; 

  constructor(private productService: ProductService,private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadProduct(this.route.snapshot.queryParams['id']);
    
  }

  loadProduct(id: number) {
    this.productService.getProduct(id).subscribe(response => {
      this.product = response;
      this.setBreadcrumb();
    })
  }

  setBreadcrumb() {
    this.breadCrumb = [
      {
        name: 'Home',
        route: '/'
      },
      {
        name: this.product.gender,
        route: ''
      },
      {
        name: this.product.productName,
        route: ''
      },
    ];
  }
}
