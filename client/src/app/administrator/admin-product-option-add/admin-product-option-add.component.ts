import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ManagerProduct, Product } from 'src/app/_models/product';
import {
  ManagerOptionColor,
  ManagerOptionSize,
} from 'src/app/_models/productOptions';
import { ManagerProductParams } from 'src/app/_models/productParams';
import { OptionService } from 'src/app/_services/option.service';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-admin-product-option-add',
  templateUrl: './admin-product-option-add.component.html',
  styleUrls: ['./admin-product-option-add.component.css'],
})
export class AdminProductOptionAddComponent implements OnInit {
  newOptionForm: FormGroup;
  colors: ManagerOptionColor[] = [];
  sizes: ManagerOptionSize[] = [];
  products: ManagerProduct[] = [];
  validationErrors: string[] = [];

  constructor(
    private fb: FormBuilder,
    private optionService: OptionService,
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadColors();
    this.loadSizes();
    this.loadProducts();
  }

  initializeForm() {
    this.newOptionForm = this.fb.group({
      colorId: [0, [Validators.required, Validators.min(0)]],
      sizeId: [0, [Validators.required, Validators.min(0)]],
      productId: [0, [Validators.required, Validators.min(0)]],
      additionalPrice: [
        0,
        [Validators.required, Validators.min(0), Validators.max(9999)],
      ],
    });
  }

  addNewOption(event) {
    this.optionService.addOption(this.newOptionForm.value).subscribe(
      (response) => {
        if (event.submitter.name == 'saveAndContinue')
        {
          this.initializeForm(); 
          this.router.navigateByUrl('/administrator/option-manager/add');
        }
        else this.router.navigateByUrl('/administrator/option-manager');
      },
      (error) => {
        this.validationErrors = error;
      }
    );
  }

  loadColors() {
    this.optionService.getManagerColorOption().subscribe((response) => {
      this.colors = response;
    });
  }

  loadSizes() {
    this.optionService.getManagerSizeOption().subscribe((response) => {
      this.sizes = response;
    });
  }

  loadProducts() {

    var params: ManagerProductParams = 
    {
      category: "",
      pageNumber: 1,
      pageSize: 9999,
      orderBy: 0,
      field: "Name",
      gender: "",
      brand: "",
      query: "",
      productStatus: [0]
    };

    this.productService
      .getManagerProducts(params)
      .subscribe((response) => {
        this.products = response.result;
      });
  }

  getColor(color: ManagerOptionColor)
  {
    return 'color: ' + color.colorCode;
  }

  getColorStyle(color: ManagerOptionColor)
  {
    return 'background-color: ' + color.colorCode;
  }

  getColorValue(color: ManagerOptionColor)
  {
    return '&#xf111;&nbsp;' + color.colorName;
  }

  isColorSelected(color: ManagerOptionColor)
  {
    return this.newOptionForm.controls['colorId'].value == color.id;
  }

  selectColor(id: number)
  {
    this.newOptionForm.controls['colorId'].setValue(id);
  }
}
