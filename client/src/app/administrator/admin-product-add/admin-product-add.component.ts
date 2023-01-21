import { DialogService } from 'src/app/_services/dialog.service';

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Brand } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';
import {
  Category,
  ManagerCatalogue,
  ManagerCategoryParams,
} from 'src/app/_models/category';
import { CategoryService } from 'src/app/_services/category.service';
import { concatMap } from 'rxjs/operators';
import { fnUpdateFormControlNumberValue } from 'src/app/_common/function/function';

@Component({
  selector: 'app-admin-product-add',
  templateUrl: './admin-product-add.component.html',
  styleUrls: ['./admin-product-add.component.css'],
})
export class AdminProductAddComponent implements OnInit {
  newProductForm: FormGroup;
  categories: Category[] = [];
  brands: Brand[] = [];
  validationErrors: string[] = [];
  selectedCategoryName: string = "";
  catalogue: ManagerCatalogue[] = [];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private router: Router,
    private toastr: ToastrService,
    private dialogService: DialogService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadCategory();
    this.loadBrand();
    this.loadCatalogue();
    
  }

  initializeForm() {
    this.newProductForm = this.fb.group({
      productName: ['', Validators.required],
      categoryId: [0, [Validators.required, Validators.min(1)]],
      subCategoryId: [null],
      brandId: [0, [Validators.required, Validators.min(0)]],
      price: [
        0,
        [Validators.required, Validators.min(0), Validators.max(9999)],
      ],
      description: [''],
    });
  }

  addNewProduct(event) {
    this.productService.addProduct(this.newProductForm.value).subscribe(
      (response) => {
        if (event.submitter.name == 'saveAndContinue')
          this.router.navigateByUrl('/administrator/product-manager/add');
        else this.router.navigateByUrl('/administrator/product-manager');

        //remove in production
        this.productService.removeCache();
        //
        this.toastr.success('Products have been added', 'Success');
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
        this.validationErrors = error;
      }
    );
  }

  loadCategory() {
    let categoryParams: ManagerCategoryParams = {
      pageNumber: 1,
      pageSize: 9999,
      orderBy: 0,
      field: 'CategoryName',
      query: '',
      categoryStatus: [0],
      genders: [],
    };
    this.categoryService
      .getManagerCategories(categoryParams)
      .subscribe((response) => {
        this.categories = response.result;
      });
  }


  loadBrand() {
    this.productService.getBrands().subscribe((response) => {
      this.brands = response;
    });
  }

  updateDescription($event: string) {
    this.newProductForm.controls['description'].setValue($event, {
      emitEvent: false,
    });
  }

  backToList() {
    this.dialogService
      .openConfirmDialog(
        'Confirm',
        'Are you sure to cancel? Every change will be lost.',
        false,
        'Yes',
        'No'
      )
      .subscribe((result) => {
        if (result.result) {
          this.router.navigateByUrl('/administrator/product-manager');
        }
      });
  }

  loadCatalogue() {
    this.categoryService.getManagerCatalogue().subscribe((result) => {
      this.catalogue = result;
    });
  }

  openCatalogue() {    
    this.dialogService
      .openCategorySingleSelectorDialog(this.catalogue)
      .subscribe((selectedResult) => {
        if (selectedResult.result)
        {
          this.selectedCategoryName = selectedResult.selectedValue;
          fnUpdateFormControlNumberValue(this.newProductForm, "categoryId", selectedResult.selectedId, false);
        }
      });
  }
}
