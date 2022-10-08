import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Brand, Category, SubCategory } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-admin-product-add',
  templateUrl: './admin-product-add.component.html',
  styleUrls: ['./admin-product-add.component.css']
})
export class AdminProductAddComponent implements OnInit {

  newProductForm: FormGroup;
  categories: Category[] = [];
  brands: Brand[] = [];
  subCategories: SubCategory[] = [];
  validationErrors: string[] = [];
  selectedCategory: number;

  constructor(private fb: FormBuilder, private productService: ProductService, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.loadCategory();
    this.selectCategory(0);
    this.loadBrand();
  }

  initializeForm() {
    this.newProductForm = this.fb.group ({
      productName: ['',Validators.required],
      categoryId: [0, [Validators.required,
        Validators.min(0)]],
      subCategoryId: [null],
      brandId: [0, [Validators.required,
        Validators.min(0)]],
      price: [0, [Validators.required,
        Validators.min(0),
        Validators.max(9999)]],
      description: [''],
    })
  }

  addNewProduct(event)
  {
    this.productService.addProduct(this.newProductForm.value).subscribe(response =>{
      if( event.submitter.name == "saveAndContinue" )
        this.router.navigateByUrl('/administrator/product-manager/add');
      else
        this.router.navigateByUrl('/administrator/product-manager');
      this.productService.removeCache();
    }
    ,error =>{
      this.validationErrors = error;
    }
    );
  }

  loadCategory() {
    this.productService.getCategories().subscribe(response =>{
      this.categories = response;
    })
  }

  loadSubCategories(categoryId: number) {
    this.selectCategory(categoryId);
    this.productService.getSubCategories(categoryId).subscribe(response =>{
      this.subCategories = response;
    })
  }

  selectCategory(categoryId: number) {
    this.selectedCategory = categoryId;
  }

  loadBrand() {
    this.productService.getBrands().subscribe(response =>{
      this.brands = response;
    })
  }


}