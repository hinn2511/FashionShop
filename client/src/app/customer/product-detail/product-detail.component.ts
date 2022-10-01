import { Component, HostListener, Input, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Params } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { BreadCrumb } from 'src/app/_models/breadcrum';
import { Product } from 'src/app/_models/product';
import sizeList, { Color, Size } from 'src/app/_models/productOptions';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  product: Product;
  breadCrumb: BreadCrumb[];
  quantity: number;
  
  sizes: Size[] = [
    {
      sizeName: 'XS'
    },
    {
      sizeName: 'S'
    },
    {
      sizeName: 'M'
    },
    {
      sizeName: 'L'
    },
    {
      sizeName: 'XL'
    },
    {
      sizeName: 'XXL'
    },
  ];

  colors: Color[] = [
    {
      colorCode: '000000',
      colorName: 'Black'
    },
    {
      colorCode: 'e8d3b9',
      colorName: 'Beige'
    },
    {
      colorCode: 'ffffff',
      colorName: 'White'
    },
    {
      colorCode: '808080',
      colorName: 'Gray'
    }
  ]

  selectedSize: Size;
  selectedColor: Color;

  constructor(private productService: ProductService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadProduct(this.route.snapshot.queryParams['id']);
    this.selectedSize = this.sizes[0];
    this.selectedColor = this.colors[0];
    this.quantity = 1;

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
        name: this.product.productName,
        route: ''
      },
      {
        name: this.product.productName,
        route: ''
      },
    ];
  }

  chooseColor(color: Color) {
    this.selectedColor = color;
  }

  onSizeChange(size: string){
    this.selectedSize.sizeName = size;
  }

 
}
