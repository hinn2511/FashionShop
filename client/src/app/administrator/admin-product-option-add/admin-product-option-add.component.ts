import {
  fnGetFormControlValue,
  fnUpdateFormControlStringValue,
} from 'src/app/_common/function/function';
import { AdminProductOptionComponent } from './../admin-product-option/admin-product-option.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ManagerProduct } from 'src/app/_models/product';
import {
  ManagerOptionColor,
  ManagerOptionSize,
} from 'src/app/_models/productOptions';
import { ManagerProductParams } from 'src/app/_models/productParams';
import { OptionService } from 'src/app/_services/option.service';
import { ProductService } from 'src/app/_services/product.service';
import { ColorPickerControl } from '@iplab/ngx-color-picker';

@Component({
  selector: 'app-admin-product-option-add',
  templateUrl: './admin-product-option-add.component.html',
  styleUrls: ['./admin-product-option-add.component.css'],
})
export class AdminProductOptionAddComponent implements OnInit {
  newOptionForm: FormGroup;

  @ViewChild('productOptionList')  productOptionList: AdminProductOptionComponent;
  colors: ManagerOptionColor[] = [];
  sizes: ManagerOptionSize[] = [];
  products: ManagerProduct[] = [];
  validationErrors: string[] = [];
  sketchControl = new ColorPickerControl().setValueFrom('#A6771C').hideAlphaChannel();
  hex: string = '#A6771C';
  constructor(
    private fb: FormBuilder,
    private optionService: OptionService,
    private productService: ProductService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadProducts();
  }

  initializeForm() {
    this.newOptionForm = this.fb.group({
      colorCode: [ '', [Validators.required]],
      colorName: ['', [Validators.required]],
      sizeName: ['', [Validators.required]],
      productId: [0, [Validators.required, Validators.min(0)]],
      additionalPrice: [
        0,
        [Validators.required, Validators.min(0), Validators.max(9999)],
      ],
    });

    this.newOptionForm.controls.productId.valueChanges.subscribe(
      (valueChange) => {
        this.productOptionList.filterByProductIds([
          fnGetFormControlValue(this.newOptionForm, 'productId'),
        ]);
      }
    );
  }

  addNewOption(event) {
    
    this.optionService.addOption(this.newOptionForm.value).subscribe(
      (response) => {
        this.toastr.success('Product option have been added', 'Success');
        if (event.submitter.name == 'saveAndContinue') {
          // this.initializeForm();
          this.productOptionList.loadOptions();
          this.router.navigateByUrl('/administrator/option-manager/add');
        } else this.router.navigateByUrl('/administrator/option-manager');
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
        this.validationErrors = error;
      }
    );
  }

  loadProducts() {
    var params: ManagerProductParams = {
      category: '',
      pageNumber: 1,
      pageSize: 9999,
      orderBy: 0,
      field: 'Name',
      gender: '',
      brand: '',
      query: '',
      productStatus: [0],
    };

    this.productService.getManagerProducts(params).subscribe((response) => {
      this.products = response.result;
    });
  }

  updateColorHex() {
    console.log(this.hex);
    
    fnUpdateFormControlStringValue(
      this.newOptionForm,
      'colorCode',
      this.hex,
      false
    );
  }
}
