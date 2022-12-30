import { ConfirmService } from 'src/app/_services/confirm.service';
import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Brand, Category, SubCategory, ManagerProduct } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-admin-product-edit',
  templateUrl: './admin-product-edit.component.html',
  styleUrls: ['./admin-product-edit.component.css']
})
export class AdminProductEditComponent implements OnInit {
  product: ManagerProduct;
  editProductForm: FormGroup;
  categories: Category[] = [];
  brands: Brand[] = [];
  subCategories: SubCategory[] = [];
  selectedCategory: number;

  descriptionUpdated = false;


  constructor(private fb: FormBuilder, private productService: ProductService, private router: Router, private route: ActivatedRoute, private toastr: ToastrService, private confirmService: ConfirmService) { }

  ngOnInit(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    this.loadProductDetail(+productId);
    this.loadCategory();
    this.selectCategory(0);
    this.loadBrand();
  }

  initializeForm() {
    this.editProductForm = this.fb.group ({
      productName: [this.product.productName,Validators.required],
      categoryId: [this.product.categoryId, [Validators.required,
        Validators.min(0)]],
      subCategoryId: [this.product.subCategoryId],
      brandId: [this.product.brandId, [Validators.required,
        Validators.min(0)]],
      price: [this.product.price, [Validators.required,
        Validators.min(0),
        Validators.max(9999)]],
      description: [this.product.description],
    })
  }

  editProduct(event)
  {
    this.productService.editProduct(this.product.id, this.editProductForm.value).subscribe(response =>{
      // this.router.navigateByUrl('/administrator/product-manager/edit/' + );
      // else
      if( event.submitter.name != "saveAndContinue" )
        this.router.navigateByUrl('/administrator/product-manager');

      //remove in production
      this.productService.removeCache();
      //
      this.toastr.success('Products have been added', 'Success');
      this.product = this.editProductForm.value;
    }, 
    error => 
    {
      this.toastr.error('Something wrong happen!', 'Error');
    });
    
  }

  loadProductDetail(productId: number)
  {
    this.productService.getManagerProduct(productId).subscribe(response => {
      this.product = response;    
      this.initializeForm();
      this.descriptionUpdated = true;  
    })
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

  updateDescription($event: string)
  {
    this.editProductForm.controls['description'].setValue($event, { emitEvent: false });
  }

  backToList()
  {
    this.confirmService.confirm("Confirm", "Are you sure to cancel? Every change will be lost.", false, "Yes", "No").subscribe(result => {
      if(result.result)
        {
          this.router.navigateByUrl("/administrator/product-manager");
        }
    })
  }

}
