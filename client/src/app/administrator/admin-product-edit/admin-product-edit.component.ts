import { SubCategory } from './../../_models/product';
import { CategoryService } from 'src/app/_services/category.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Brand, ManagerProduct } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';
import { Category, ManagerCatalogue, ManagerCategoryParams, fnGetGenderName } from 'src/app/_models/category';
import { DialogService } from 'src/app/_services/dialog.service';
import { fnFlattenArray, fnUpdateFormControlNumberValue } from 'src/app/_common/function/global';

@Component({
  selector: 'app-admin-product-edit',
  templateUrl: './admin-product-edit.component.html',
  styleUrls: ['./admin-product-edit.component.css'],
})
export class AdminProductEditComponent implements OnInit {
  product: ManagerProduct;
  editProductForm: FormGroup;
  brands: Brand[] = [];
  selectedCategory: number;
  descriptionUpdated = false;
  selectedCategoryName: string = "";
  catalogue: ManagerCatalogue[] = [];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private dialogService: DialogService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    this.loadProductDetail(+productId);
    this.loadCatalogue();
    this.loadBrand();
  }

  initializeForm() {
    this.editProductForm = this.fb.group({
      productName: [this.product.productName, Validators.required],
      categoryId: [
        this.product.categoryId,
        [Validators.required, Validators.min(0)],
      ],
      brandId: [this.product.brandId, [Validators.required, Validators.min(0)]],
      price: [
        this.product.price,
        [Validators.required, Validators.min(0), Validators.max(9999)],
      ],
      description: [this.product.description],
    });
  }

  editProduct(event) {
    this.productService
      .editProduct(this.product.id, this.editProductForm.value)
      .subscribe(
        (response) => {
          // this.router.navigateByUrl('/administrator/product-manager/edit/' + );
          // else
          if (event.submitter.name != 'saveAndContinue')
            this.router.navigateByUrl('/administrator/product-manager');

          //remove in production
          this.productService.removeCache();
          //
          this.toastr.success('Products have been added', 'Success');
          this.product = this.editProductForm.value;
        },
        (error) => {
          this.toastr.error('Something wrong happen!', 'Error');
        }
      );
  }

  loadProductDetail(productId: number) {
    this.productService.getManagerProduct(productId).subscribe((response) => {
      this.product = response;
      this.initializeForm();
      this.descriptionUpdated = true;
      this.selectedCategoryName = this.product.categoryGender + " - " + this.product.categoryName
    });
  }

  loadBrand() {
    this.productService.getBrands().subscribe((response) => {
      this.brands = response;
    });
  }

  updateDescription($event: string) {
    this.editProductForm.controls['description'].setValue($event, {
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
          fnUpdateFormControlNumberValue(this.editProductForm, "categoryId", selectedResult.selectedId, false);
        }
      });
  }
}
