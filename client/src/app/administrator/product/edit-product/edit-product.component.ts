import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.css']
})
export class EditProductComponent implements OnInit {
  product: Product;
  editProductForm: FormGroup;
  validationErrors: string[] = [];

  constructor(private productService: ProductService,private fb: FormBuilder, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadProduct(+(this.route.snapshot.paramMap.get('id')));
    this.initializeForm();
  }

  initializeForm() {
    this.editProductForm = this.fb.group ({
      id: [this.product.id],
      productName: [this.product.productName,Validators.required],
      categoryId: [this.product.categoryId,Validators.required],
      productPrice: [this.product.price,Validators.required],
      brandId: [this.product.brandId,Validators.required],
    })
  }

  loadProduct(id: number) {
    this.productService.getProduct(id).subscribe(response => {
      this.product = response;
    })
  }

  editProduct() {
    this.productService.editProduct(this.editProductForm.value).subscribe(response =>{
      this.editProductForm.reset();
      this.productService.removeProductCache(this.product.id);
    }, error =>{
      this.validationErrors = error;
    })
  }

}
