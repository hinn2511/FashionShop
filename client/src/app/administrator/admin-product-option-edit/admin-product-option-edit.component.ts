import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ColorPickerControl } from '@iplab/ngx-color-picker';
import { ToastrService } from 'ngx-toastr';
import { fnUpdateFormControlStringValue } from 'src/app/_common/function/function';
import { ManagerProduct } from 'src/app/_models/product';
import {
  ManagerOption,
  ManagerOptionColor,
  ManagerOptionSize,
} from 'src/app/_models/productOptions';
import { OptionService } from 'src/app/_services/option.service';
import { ProductService } from 'src/app/_services/product.service';
import { AdminProductOptionComponent } from '../admin-product-option/admin-product-option.component';

@Component({
  selector: 'app-admin-product-option-edit',
  templateUrl: './admin-product-option-edit.component.html',
  styleUrls: ['./admin-product-option-edit.component.css'],
})
export class AdminProductOptionEditComponent implements OnInit {
  updateOptionForm: FormGroup;
  @ViewChild('productOptionList')
  productOptionList: AdminProductOptionComponent;
  colors: ManagerOptionColor[] = [];
  sizes: ManagerOptionSize[] = [];
  products: ManagerProduct[] = [];
  option: ManagerOption;
  validationErrors: string[] = [];
  sketchControl = new ColorPickerControl()
    .setValueFrom('#A6771C')
    .hideAlphaChannel();
  hex: string = '#A6771C';
  constructor(
    private fb: FormBuilder,
    private optionService: OptionService,
    private productService: ProductService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    const optionId = this.route.snapshot.paramMap.get('id');
    this.loadProductOptionDetail(+optionId);
  }

  loadProductOptionDetail(optionId: number) {
    this.optionService.getManagerOption(optionId).subscribe((response) => {
      this.option = response;
      this.initializeForm();
      this.hex = this.option.colorCode;
      setTimeout(() => {
        this.productOptionList.filterByProductIds([this.option.product.id]);
      }, 500);
    });
  }

  initializeForm() {
    this.updateOptionForm = this.fb.group({
      colorCode: [
        { value: this.option.colorCode, disabled: true },
        [Validators.required],
      ],
      colorName: [this.option.colorName, [Validators.required]],
      sizeName: [this.option.sizeName, [Validators.required]],
      productId: [
        { value: this.option.product.id, disabled: true },
        [Validators.required, Validators.min(0)],
      ],
      additionalPrice: [
        this.option.additionalPrice,
        [Validators.required, Validators.min(0), Validators.max(9999)],
      ],
    });
  }

  updateOption(event) {
    console.log(this.updateOptionForm.getRawValue());
    
    this.optionService
      .editOption(this.option.id, this.updateOptionForm.getRawValue())
      .subscribe(
        (response) => {
          this.toastr.success('Product option have been updated', 'Success');
          if (event.submitter.name == 'saveAndContinue') {
            // this.initializeForm();
            this.productOptionList.loadOptions();
            this.productOptionList.filterByProductIds([this.option.product.id]);
            this.router.navigateByUrl(`/administrator/option-manager/edit/${this.option.product.id}`);
          } else this.router.navigateByUrl('/administrator/option-manager');
        },
        (error) => {
          this.toastr.error(error, 'Error');
          this.validationErrors = error;
        }
      );
  }

  updateColorHex() {
    fnUpdateFormControlStringValue(
      this.updateOptionForm,
      'colorCode',
      this.hex,
      false
    );
  }
}
