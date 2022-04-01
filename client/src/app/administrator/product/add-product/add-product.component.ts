import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css']
})
export class AddProductComponent implements OnInit {

  addProductForm: FormGroup;
  validationErrors: string[] = [];

  constructor(private productService: ProductService,private fb: FormBuilder,  private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.addProductForm = this.fb.group ({
      productName: ['',Validators.required],
      categoryId: ['',Validators.required],
      productPrice: ['',Validators.required],
      gender: ['',Validators.required],
    })
  }

  addProduct() {
    this.productService.addProduct(this.addProductForm.value).subscribe( response =>{
      this.addProductForm.reset();
    }, error =>{
      this.validationErrors = error;
    })
  }

}
