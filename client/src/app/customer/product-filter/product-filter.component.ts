import { CategoryService } from './../../_services/category.service';
import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { CustomerCatalogue } from 'src/app/_models/category';
import { CustomerColorFilter, CustomerPriceRange, CustomerSizeFilter, ProductParams } from 'src/app/_models/productParams';
import { fnConvertToSlug } from 'src/app/_common/function/function';

export class ProductFilter {
  productParams: ProductParams;
  selectedColor: CustomerColorFilter;
  selectedSize: CustomerSizeFilter;
  selectedPriceRange: CustomerPriceRange;
  constructor(productParams: ProductParams,
    selectedColor: CustomerColorFilter,
    selectedSize: CustomerSizeFilter,
    selectedPriceRange: CustomerPriceRange)
    {
      this.productParams = productParams;
      this.selectedColor = selectedColor;
      this.selectedSize = selectedSize;
      this.selectedPriceRange = selectedPriceRange;
    }
}

@Component({
  selector: 'app-product-filter',
  templateUrl: './product-filter.component.html',
  styleUrls: ['./product-filter.component.css']
})
export class ProductFilterComponent implements OnInit {
  @Input() productParams: ProductParams;
  @Input() selectedCategory: number;
  @Input() showCategory: boolean = false;
  catalogues: CustomerCatalogue[] = [];
  @Input() selectedGender: number;
  @Input()  colorFilters: CustomerColorFilter[] = [];
  @Output() apply = new EventEmitter<ProductFilter>();
  @Output() reset = new EventEmitter<ProductFilter>();
  @Output() close = new EventEmitter<boolean>();

  priceRanges: CustomerPriceRange[] = [
    new CustomerPriceRange(0, -1, 50),
    new CustomerPriceRange(1, 50, 100),
    new CustomerPriceRange(2, 100, 300),
    new CustomerPriceRange(3, 300, -1),
  ];

  sizeFilters: CustomerSizeFilter[] = [
    new CustomerSizeFilter(0, 'XS'),
    new CustomerSizeFilter(1, 'S'),
    new CustomerSizeFilter(2, 'M'),
    new CustomerSizeFilter(3, 'L'),
    new CustomerSizeFilter(4, 'XL'),
    new CustomerSizeFilter(5, 'XXL'),
  ];

  selectedColor: CustomerColorFilter;
  selectedSize: CustomerSizeFilter;

  selectedPriceRange: CustomerPriceRange;
  showResetPriceRangeButton: boolean = false;
  showResetSizeFilterButton: boolean = false;
  showResetCategoryFilterButton: boolean = false;
  showResetColorFilterButton: boolean = false;

  constructor(private categoryService: CategoryService) { }

  ngOnInit(): void {
    if(this.showCategory)
    {
      this.loadCategoryGroup();
    }
  }

  applyAndCloseFilter()
  {
      this.apply.emit(new ProductFilter(this.productParams, this.selectedColor, this.selectedSize, this.selectedPriceRange));
  }

  applyFilter()
  {
      this.apply.emit(new ProductFilter(this.productParams, this.selectedColor, this.selectedSize, this.selectedPriceRange));
  }

  resetFilter() {
    this.resetPriceRange();
    this.resetSize();
    this.resetColor();
    this.productParams.categoryId = this.selectedCategory;
    this.productParams.gender = this.selectedGender;
    this.applyFilter();
  }

  resetColor() {
    this.selectedColor = null;
    this.showResetColorFilterButton = false;
    this.productParams.colorCode = '';
    this.applyFilter();
  }

  resetPriceRange() {
    this.selectedPriceRange = null;
    this.showResetPriceRangeButton = false;
    this.productParams.minPrice = -1;
    this.productParams.maxPrice = -1;
    this.applyFilter();
  }

  resetSize() {
    this.selectedSize = null;
    this.showResetSizeFilterButton = false;
    this.productParams.size = '';
    this.applyFilter();
  }

  resetCategory() {
    this.selectedCategory = 0;
    this.showResetSizeFilterButton = false;
    this.productParams.categoryId = 0;
    this.applyFilter();
  }



  setPriceRange(priceRangeId: number) {
    if (this.selectedPriceRange != null) {
      this.selectedPriceRange = null;
      this.resetPriceRange();
      return;
    }
    let priceRange = this.priceRanges.find((x) => x.id == priceRangeId);
    this.productParams.minPrice = priceRange?.minPrice;
    this.productParams.maxPrice = priceRange?.maxPrice;
    this.selectedPriceRange = priceRange;
    this.showResetPriceRangeButton = true;
    this.applyFilter();
  }

  setSizeFilter(sizeFilterId: number) {
    if (this.selectedSize != null) {
      this.selectedSize = null;
      this.resetSize();
      return;
    }
    let size = this.sizeFilters.find((x) => x.id == sizeFilterId);
    this.selectedSize = size;
    this.productParams.size = size.sizeName;
    this.showResetSizeFilterButton = true;
    this.applyFilter();
  }

  setColorFilter(colorCode: string) {
    if (this.selectedColor != null) {
      this.selectedColor = null;
      this.resetColor();
      return;
    }
    let color = this.colorFilters.find((x) => x.colorCode == colorCode);
    this.selectedColor = color;
    this.productParams.colorCode = color.colorCode;
    this.showResetColorFilterButton = true;
    this.applyFilter();
  }

  setCategoryFilter(categoryId: number, gender: number) {
    console.log(`gender ${gender} categoryId ${categoryId}`);
    
    if (this.selectedCategory == categoryId)
    {
      this.selectedCategory = undefined;
      return;
    }
    else
      this.selectedCategory = categoryId;
    this.productParams.categoryId = categoryId;
    this.productParams.gender = gender;
    this.showResetCategoryFilterButton = true;

    this.applyFilter();
  }

  setGender(gender: number)
  {
    if (this.selectedGender == gender)
    this.selectedGender = undefined;
    else
    this.selectedGender = gender;

  }



  closeFilter()
  {
    this.close.emit(true);
  }

  loadCategoryGroup() {
    this.categoryService.getCatalogue().subscribe((result) => {
      this.catalogues = result;
    });
  }



}
